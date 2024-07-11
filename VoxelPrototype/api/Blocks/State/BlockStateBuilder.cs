using VoxelPrototype.api.Blocks.Properties;
namespace VoxelPrototype.api.Blocks.State
{
    public class BlockStateBuilder
    {
        internal Block Block;
        internal Dictionary<string, IProperty> Properties = new();
        internal BlockStateBuilder(Block Block)
        {
            this.Block = Block;
        }
        public void Register(params IProperty[] properties)
        {
            foreach (var property in properties)
            {
                Properties[property.Name] = property;
            }
        }
        internal BlockStateHolder Build()
        {
            return new BlockStateHolder(this);
        }
    }
}
