using System.Collections;
using System.IO;
using VoxelPrototype.common.API.Blocks.Properties;
using VoxelPrototype.common.API.Blocks.State;
namespace VoxelPrototype.common.API.Blocks.state
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
            
            /*
            var AllValues = new List<List<object>> { new List<object>() };
            foreach (var property in Properties.Values)
            {
                var newList = new List<object>();
                var genericType = property.GetType().BaseType.GetGenericArguments()[0];
                var getAllValuesMethod = property.GetType().GetMethod("GetAllValues");
                if (getAllValuesMethod != null && getAllValuesMethod.IsPublic && !getAllValuesMethod.IsAbstract)
                {
                    var values = getAllValuesMethod.Invoke(property, null);
                    // Ajoutez les valeurs obtenues à la nouvelle liste
                    if (values != null)
                    {
                        
                        foreach (var allowedValue in (IEnumerable)values)
                        {
                            newList.Add(allowedValue);
                        }
                    }
                }
                AllValues.Add(newList);
            }*/

            /*
            foreach (var property in Properties.Values)
            {
                AllValues = AllValues.SelectMany(lst =>
                    property.GetAllPossibleValues().Select(allowedValue =>
                        new List<object>(lst) { allowedValue }
                    )
                ).ToList();
            }*/
            Dictionary<(IProperty, object), BlockState> statesByProperties = new();
            uint ID=0;
            foreach (var uniqueGroup in AllValues)
            {
                var propertiesValues = new Dictionary<IProperty, object>(Properties.Values.Zip(uniqueGroup, (k, v) => new KeyValuePair<IProperty, object>(k, v)));
                //Dictionary<IProperty, object> propertiesValues = Properties.ToDictionary(kv => kv.Value, kv => uniqueGroup.ElementAt(Properties.Values.ToList().IndexOf(kv.Value)));
                
                /*Dictionary<IProperty, object> propertiesValues = new();
                foreach (var kvp in Properties)
                {
                    var value = kvp.Value;
                    var index = Properties.Values.ToList().IndexOf(value);
                    var uniqueGroupValue = uniqueGroup.ElementAt(index);
                    propertiesValues[value] = uniqueGroupValue;
                }*/


                BlockState state = new BlockState(this.Block, propertiesValues,ID);
                foreach(var pair in propertiesValues)
                {
                    statesByProperties.Add((pair.Key, pair.Value),state);
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
