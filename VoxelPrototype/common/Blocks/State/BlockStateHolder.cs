using System.Collections;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.common.Blocks.Properties;

namespace VoxelPrototype.common.Blocks.State
{
    public class BlockStateHolder
    {
        private Block Block;
        private readonly Dictionary<string, IProperty> Properties;
        private readonly List<BlockState> States = new();
        internal BlockStateHolder(Block Block, Dictionary<string, IProperty> Properties)
        {
            this.Block = Block;
            this.Properties = Properties;
            var AllValues = new List<List<object>> { new List<object>() };
            foreach (var property in Properties.Values)
            {
                var getAllValuesMethod = property.GetType().GetMethod("GetAllValues");
                var values = (IEnumerable)getAllValuesMethod.Invoke(property, null);
                List<List<object>> newAllValues = new List<List<object>>();
                foreach (var lst in AllValues)
                {
                    foreach (var allowedValue in values)
                    {
                        var newList = new List<object>(lst);
                        newList.Add(allowedValue);
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
                BlockState state = new BlockState(this.Block, propertiesValues, ID);
                foreach (var pair in propertiesValues)
                {
                    statesByProperties.Add((pair.Key, pair.Value), state);
                }
                States.Add(state);
                ID++;
            }
            foreach (var state in States)
            {
                state.SetAllStates(statesByProperties);
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
