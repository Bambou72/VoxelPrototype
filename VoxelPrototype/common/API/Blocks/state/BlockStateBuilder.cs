using VoxelPrototype.common.API.Blocks.Properties;
using System.Collections.Generic;
namespace VoxelPrototype.common.API.Blocks.state
{
    public class BlockStateBuilder
    {
        private readonly Block Block;
	    private readonly Dictionary<string, IProperty> Properties = new();
        internal BlockStateBuilder(Block Block)
        {
            this.Block = Block;
        }
        public void Register<T>(Property<T> property)
        {
            string name =property.Name;
            List<T> allowedValues = property.GetAllValues();
            if (allowedValues.Count ==0)
            {
                throw new Exception("Given property '" + name + "' returns empty collection for allowed values.");
            }
            if (Properties.ContainsKey(name))
            {
                throw new Exception("Given property is already registered in this state container builder.");
            }
            Properties.Add(name, property);
        }
        internal BlockStateHolder Build()
        {
            return new BlockStateHolder(Block, Properties);
        }
    }
}
