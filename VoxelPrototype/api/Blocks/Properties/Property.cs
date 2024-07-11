namespace VoxelPrototype.api.Blocks.Properties
{
    public interface IProperty
    {
        public string Name { get; }
    }
    public class Property<T> : IProperty
    {
        public string Name { get; }

        public Property(string Name)
        {
            this.Name = Name;
        }
        public virtual List<T> GetAllValues()
        {
            throw new NotImplementedException();
        }

        public virtual string GetValueString(T value)
        {
            throw new NotImplementedException();
        }

        public virtual T GetValueFromString(string value)
        {
            throw new NotImplementedException();
        }

        public virtual T DefaultValue()
        {
            throw new NotImplementedException();
        }
    }
}
