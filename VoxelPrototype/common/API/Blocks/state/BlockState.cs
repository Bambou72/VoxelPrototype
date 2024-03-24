using System.Collections.Immutable;
namespace VoxelPrototype.common.API.Blocks.state
{
    public class BlockState
    {
        private readonly Block Parent;
        private readonly ImmutableDictionary<BlockProperty<object>, object> Properties;
        private Dictionary<(BlockProperty<object>, object), BlockState> StatesByValues;
        private ushort Id;
        public BlockState(Block owner, ImmutableDictionary<BlockProperty<object>, object> properties,ushort Id)
        {
            Parent = owner;
            Properties = properties;
            this.Id = Id;
        }
        public Block GetBlock()
        {
            return Parent;
        }
        public bool IsBlock(Block block)
        {
            return Parent == block;
        }
        public ushort GetId()
        {
            return Id;
        }
        public int GetPropertiesCount()
        {
            return Properties.Count;
        }
        public ImmutableDictionary<BlockProperty<object>, object> GetProperties()
        {
            return Properties;
        }
        public bool Has(BlockProperty<object> property)
        {
            return Properties.ContainsKey(property);
        }
        public T Get<T>(BlockProperty<T> property)
        {
            if (Properties.TryGetValue(property as BlockProperty<object>, out var value))
            {
                return (T)value;
            }
            throw new InvalidOperationException($"Block state for {this.Parent.Id} doesn't define the property {property.GetName()}.");
        }
        public BlockState With<T>(BlockProperty<T> property, T value)
        {
            return WithRaw(property as BlockProperty<object>, value);
        }
        public BlockState WithRaw(BlockProperty<object> property, object value)
        {
            object v = Get(property);
            if (v == value)
                return this;
            BlockState state = StatesByValues[(property, value)];
            if (state == null)
                throw new InvalidOperationException($"Block state for {Parent.Id} doesn't allow the value {value} for property {property.GetName()}.");
            return state;
        }
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = (BlockState)obj;
            return this.Parent.Id == that.GetBlock().Id &&  Id == that.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return $"BlockState:{{block={Parent.Id}, properties={Properties}, uid={Id}}}";
        }
        public void SetAllStates(Dictionary<ImmutableDictionary<BlockProperty<object>, object>, BlockState> states)
        {
            if (StatesByValues != null)
            {
                throw new InvalidOperationException("States by values are already defined for this block state.");
            }
            var statesByValues = new Dictionary<(BlockProperty<object>,object), BlockState>();
            var tempMapForGet = new Dictionary<BlockProperty<object>, object>(Properties);
            foreach (var (prop, value) in Properties)
            {
                foreach (var allowedValue in prop.GetAllPossibleValues())
                {
                    if (!ReferenceEquals(allowedValue, value))
                    {
                        if (tempMapForGet.TryGetValue(prop, out var oldValue))
                        {
                            tempMapForGet[prop] = allowedValue;
                            statesByValues.Add((prop, allowedValue), states[tempMapForGet.ToImmutableDictionary()]);
                            tempMapForGet[prop] = oldValue;
                        }
                    }
                }
            }
            StatesByValues = statesByValues.Count == 0
                ? new Dictionary<(BlockProperty<object>, object), BlockState>(statesByValues)
                : new Dictionary<(BlockProperty<object>, object), BlockState>();
        }
    }
}
