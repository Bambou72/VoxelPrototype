using System.Collections.ObjectModel;
namespace VoxelPrototype.common.API.Blocks.state
{
    public class BlockStateBuilder
    {
        private readonly Block Parent;
	    private readonly Dictionary<string, BlockProperty<object>> Properties;
        public BlockStateBuilder(Block owner)
        {
            Parent = owner;
            Properties= new();
        }
        public void Register<T>(BlockProperty<T> property)
        {
            string name =property.GetName();
            IReadOnlyCollection<T> allowedValues = property.GetAllPossibleValues();
            if (allowedValues.Count ==0)
                throw new Exception("Given property '" + name + "' returns empty collection for allowed values.");
            if (Properties.ContainsKey(name))
            {
                throw new Exception("Given property is already registered in this state container builder.");
            }
            Properties.Add(name, property as BlockProperty<object>);
        }
        public BlockStateHolder build()
        {
            return new BlockStateHolder(Parent, Properties);
        }
    }
}
