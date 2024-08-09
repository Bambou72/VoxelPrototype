namespace VoxelPrototype.api.block.state.properties
{
    public interface IProperty
    {
        public string Name { get; }
        public List<object> GetAllValues();

        public object GetValueFromString(string value);

    }
    public class Property<T> : IProperty
    {
        public string Name { get; }

        public Property(string Name)
        {
            this.Name = Name;
        }

        public virtual string GetValueString(T value)
        {
            throw new NotImplementedException();
        }
        public virtual T DefaultValue()
        {
            throw new NotImplementedException();
        }

        public virtual List<object> GetAllValues()
        {
            throw new NotImplementedException();
        }

        public virtual object GetValueFromString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
