using System.Collections.Immutable;
namespace VoxelPrototype.common.API.Blocks.state
{
    public class BlockStateHolder
    {
        private  Block Parent;
	    private ImmutableDictionary<string, BlockProperty<object>> Properties;
        private ImmutableList<BlockState> States;
        internal BlockStateHolder(Block parent, Dictionary<string, BlockProperty<object>> properties)
        {
            Parent = parent;
            Properties = ImmutableDictionary.CreateRange(properties);
            var stream = new List<List<object>> { new List<object>() };
            Dictionary<ImmutableDictionary<BlockProperty<object>, object>, BlockState > statesByProperties = new();
            List<BlockState> states = new();
            foreach (var property in properties.Values)
            {
                stream = stream.SelectMany(lst =>
                    property.GetAllPossibleValues().SelectMany(allowedValue =>
                    {
                        var newList = new List<object>(lst);
                        newList.Add(allowedValue);
                        return new List<List<object>> { newList };
                    })
                ).ToList();
            }
            ushort Id =0;
            foreach (var uniqueGroup in stream)
            {
                var propertiesValues = properties.ToImmutableDictionary(kv => kv.Value, kv => uniqueGroup.ElementAt(properties.Values.ToList().IndexOf(kv.Value)));
                var state = new BlockState(Parent, propertiesValues,Id);
                Id++;
                statesByProperties.Add(propertiesValues, state);
                states.Add(state);
            }
            foreach (var state in states)
            {
                state.SetAllStates(statesByProperties);
                //state.SetupBoundingBoxes();
            }
            States = states.ToImmutableList();
        }
        public BlockState GetBaseState()
        {
            return States[0];
        }
        public ImmutableList<BlockState> GetStates()
        {
            return States;
        }
        public BlockProperty<object> GetPropertiesByName(string name)
        {
            return Properties[name];
        }
    }
}
