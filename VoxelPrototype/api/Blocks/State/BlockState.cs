using System.Collections.Immutable;
using System.Reflection;
using VoxelPrototype.api.Blocks.Properties;
using VoxelPrototype.VBF;
namespace VoxelPrototype.api.Blocks.State
{
    public class BlockState : IVBFSerializable<BlockState>
    {
        public Block Block { get; }
        public uint ID { get; }
        private ImmutableDictionary<IProperty, object> Properties { get; }
        private ImmutableDictionary<(IProperty, object), BlockState> StatesByValues;
        public BlockState() { }
        public BlockState(Block Block, Dictionary<IProperty, object> Properties, uint ID)
        {
            this.Block = Block;
            this.Properties = Properties.ToImmutableDictionary();
            this.ID = ID;
        }
        public bool Has(IProperty Property)
        {
            return Properties.ContainsKey(Property);
        }
        public T Get<T>(Property<T> Property)
        {
            if (Properties.TryGetValue(Property, out var value))
            {
                return (T)value;
            }
            else
            {
                throw new Exception("Can't find property");
            }
        }
        public BlockState With<T>(Property<T> property, T value)
        {
            T curvalue = Get(property);
            if (curvalue.Equals(value))
            {
                return this;
            }
            else
            {

                BlockState state = StatesByValues[(property, value)];
                if (state == null)
                {
                    throw new InvalidOperationException($"Block state for {Block.ID} doesn't allow the value {value} for property {property.Name}.");
                }
                return state;
            }
        }
        public BlockState With(string property, string value)
        {
            var Curpair = Properties.FirstOrDefault(pair => pair.Key.Name == property);
            if (Curpair.Key == null)
            {
                throw new KeyNotFoundException($"Property '{property}' not found.");
            }
            MethodInfo getValueFromStringMethod = Curpair.Key.GetType().GetMethod("GetValueFromString");
            object parsedValue = getValueFromStringMethod.Invoke(Curpair.Key, [value]);
            if (!Curpair.Value.Equals(parsedValue))
            {
                BlockState state = StatesByValues[(Curpair.Key, parsedValue)];
                if (state == null)
                {
                    throw new InvalidOperationException($"Block state for {Block.ID} doesn't allow the value {value} for property {Curpair.Key.Name}.");
                }
                return state;
            }
            return this;
        }
        public override string ToString()
        {
            string Base = "";
            foreach (var property in Properties.Keys)
            {
                string Name = property.Name;
                string Value = Properties[property].ToString();
                if (Base != "")
                {
                    Base += ";";
                }
                Base += Name + ":" + Value;
            }
            return Base;
        }
        public void SetAllStates(ImmutableDictionary<(IProperty, object), BlockState> states)
        {
            StatesByValues = states;
        }
        public VBFCompound Serialize()
        {
            VBFCompound BlockState = new();
            BlockState.AddString("ID", Block.ID);
            VBFCompound Properties = new();
            foreach (var prop in this.Properties)
            {
                Properties.AddString(prop.Key.Name, prop.Value.ToString());
            }
            BlockState.Add("Prop", Properties);
            return BlockState;
        }
        public BlockState Deserialize(VBFCompound compound)
        {
            var Block = BlockRegistry.GetInstance().GetBlock(compound.GetString("ID").Value);
            var CurrentBlockState = Block.GetDefaultState();

            foreach (var prop in compound.Get<VBFCompound>("Prop").Tags)
            {
                var vbfval = (VBFString)prop.Value;
                CurrentBlockState = CurrentBlockState.With(prop.Key, vbfval.Value);
            }
            return CurrentBlockState;
        }
    }
}
