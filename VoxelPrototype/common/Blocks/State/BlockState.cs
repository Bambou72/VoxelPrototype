using VoxelPrototype.client;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.common.Blocks.Properties;
using VoxelPrototype.VBF;

namespace VoxelPrototype.common.Blocks.State
{
    public class BlockState : IVBFSerializable<BlockState>, IEquatable<BlockState>
    {
        public Block Block { get; }
        public uint ID { get; }
        private readonly Dictionary<IProperty, object> Properties;
        private Dictionary<(IProperty, object), BlockState> StatesByValues;
        public BlockState() { }
        public BlockState(Block Block, Dictionary<IProperty, object> Properties, uint ID)
        {
            this.Block = Block;
            this.Properties = Properties;
            this.ID = ID;
        }
        public bool Has(Property<object> property)
        {
            return Properties.ContainsKey(property);
        }
        public T Get<T>(Property<T> property)
        {
            foreach (var prop in Properties)
            {
                if (prop.Key.Equals(property))
                {
                    return (T)prop.Value;
                }
            }
            throw new Exception($"{property.Name} can't be found.");
        }
        public (T, IProperty) GetIntern<T>(Property<T> property)
        {
            foreach (var prop in Properties)
            {
                if (prop.Key.Equals(property))
                {
                    return ((T)prop.Value, prop.Key);
                }
            }
            throw new Exception($"{property.Name} can't be found.");
        }
        public BlockState With<T>(Property<T> property, T value)
        {
            return WithRaw(property, value);
        }
        public BlockState WithRaw<T>(Property<T> property, object value)
        {
            (object v, IProperty Prop) = GetIntern(property);
            if (v == value)
            {
                return this;

            }
            BlockState state = StatesByValues[(Prop, value)];
            if (state == null)
            {
                throw new InvalidOperationException($"Block state for {Block.ID} doesn't allow the value {value} for property {property.Name}.");
            }
            return state;
        }
        public bool Equals(BlockState? other)
        {
            if (other.ID != ID) return false;
            if (other.Block.ID != Block.ID) return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = (BlockState)obj;
            return Block.ID == that.Block.ID && ID == that.ID;
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
        public void SetAllStates(Dictionary<(IProperty, object), BlockState> states)
        {
            StatesByValues = states;
        }
        public VBFCompound Serialize()
        {
            VBFCompound BlockState = new();
            BlockState.AddString("ID", Block.ID);
            VBFCompound Properties = new();
            foreach (var prop in this.Properties.Keys)
            {
                object obj = this.Properties[prop];
                if (obj.GetType() == typeof(bool))
                {
                    Properties.AddBool(prop.Name, (bool)obj);
                }
                else if (obj.GetType().IsEnum)
                {
                    VBFCompound EnumProperty = new VBFCompound();
                    EnumProperty.AddString("Type", obj.GetType().Name);
                    EnumProperty.AddString("Value", obj.ToString());
                    Properties.Add(prop.Name, EnumProperty);
                }
            }
            BlockState.Add("Properties", Properties);
            return BlockState;
        }
        public BlockState Deserialize(VBFCompound compound)
        {
            var Block = Client.TheClient.ModManager.BlockRegister.GetBlock(compound.GetString("ID").Value);
            var CurrentBlockState = Block.GetDefaultState();

            foreach (var prop in compound.Get<VBFCompound>("Properties").Tags)
            {
                if (prop.Value.GetType() == typeof(VBFBool))
                {
                    var Value = (VBFBool)prop.Value;
                    CurrentBlockState = CurrentBlockState.With(new BooleanProperty(prop.Key), Value.Value);
                }
                else if (prop.Value.GetType() == typeof(VBFCompound))
                {
                    var Value = (VBFCompound)prop.Value;
                    Type enumType = Type.GetType(Value.GetString("Type").Value);
                    var parsed = Enum.Parse(enumType, Value.GetString("Value").Value);
                    var pro = Activator.CreateInstance(typeof(EnumProperty<>).MakeGenericType(enumType), prop.Key);
                    CurrentBlockState = CurrentBlockState.With((dynamic)pro, (dynamic)parsed);
                }
            }
            return CurrentBlockState;
        }


    }
}
