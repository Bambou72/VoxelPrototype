using System.Collections;
using System.Collections.Immutable;
using VoxelPrototype.api.Blocks.Properties;
namespace VoxelPrototype.api.Blocks.State
{
    public class BlockStateHolder
    {
        private Block Block;
        private ImmutableDictionary<string, IProperty> Properties;
        private List<BlockState> States = new();
        internal BlockStateHolder(BlockStateBuilder Builder)
        {
            Block = Builder.Block;
            Properties = Builder.Properties.ToImmutableDictionary();
            var AllValues = new List<List<object>> { new List<object>() };
            foreach (var property in Properties.Values)
            {
                var values = property.GetAllValues();
                List<List<object>> newAllValues = new List<List<object>>();
                foreach (var lst in AllValues)
                {
                    foreach (var allowedValue in values)
                    {
                        var newList = new List<object>(lst)
                        {
                            allowedValue
                        };
                        newAllValues.Add(newList);
                    }
                }
                AllValues = newAllValues;
            }
            Dictionary<(IProperty, object), BlockState> statesByProperties = new();
            uint ID = 0;
            foreach (var uniqueGroup in AllValues)
            {
                var propertiesValues = new Dictionary<IProperty, object>(Properties.Values.Zip(uniqueGroup, (k, v) => new KeyValuePair<IProperty, object>(k, v)));
                BlockState state = new BlockState(Block, propertiesValues, ID);
                foreach (var pair in propertiesValues)
                {
                    statesByProperties.Add((pair.Key, pair.Value), state);
                }
                States.Add(state);
                ID++;
            }
            var immstates = statesByProperties.ToImmutableDictionary();
            foreach (var state in States)
            {
                state.SetAllStates(immstates);
            }
        }
        public BlockState GetBaseState()
        {
            return States[0];
        }
        public List<BlockState> GetStates()
        {
            return States;
        }
        public IProperty GetPropertiesByName(string name)
        {
            return Properties[name];
        }
    }
}
