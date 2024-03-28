namespace VoxelPrototype.common.API.Blocks.Properties
{
    /*
    public class EnumProperty<T> : Property<T> where T : Enum
    {
        public readonly Type enumClass;
        private readonly T defaultValue;
        private readonly HashSet<T> valuesSet;
        private readonly string[] names;
        public EnumProperty(string name, Type enumClass, T[] values) : base(name)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            this.enumClass = enumClass ?? throw new ArgumentNullException(nameof(enumClass));
            defaultValue = values[0];
            valuesSet = new HashSet<T>(values);
            names = values.Select(v => v.ToString().ToLower()).ToArray();
        }
        
        public override IReadOnlyCollection<T> GetAllValues()
        {
            //return valuesSet;
        }
        public override T DefaultValue()
        {
            return defaultValue;
        }
        public override T GetValueFromString(string name)
        {
            name = name.ToUpper();
            foreach (T value in valuesSet)
            {
                if (value.ToString().ToUpper() == name)
                {
                    return value;
                }
            }
            return defaultValue;
        }
        public override string GetValueString(T value)
        {
            int ordinal = Convert.ToInt32(value);
            return names[ordinal];
        }
    }*/
}
