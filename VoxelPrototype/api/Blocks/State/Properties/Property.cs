namespace VoxelPrototype.api.Blocks.Properties
{
    public interface IProperty
    {
        public string Name { get; }
        public List<object> GetAllValues();
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

        public virtual T GetValueFromString(string value)
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
    }
}
