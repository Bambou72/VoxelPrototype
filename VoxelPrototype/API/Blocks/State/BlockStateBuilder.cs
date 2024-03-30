using System.Collections.Generic;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.Blocks.Properties;
namespace VoxelPrototype.API.Blocks.State
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
            string name = property.Name;
            List<T> allowedValues = property.GetAllValues();
            if (allowedValues.Count == 0)
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
