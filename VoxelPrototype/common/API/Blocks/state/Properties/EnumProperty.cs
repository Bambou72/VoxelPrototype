namespace VoxelPrototype.common.API.Blocks.state.Properties
{
    public class EnumProperty<T> : BlockProperty<T> where T : Enum
    {
        private readonly Type enumClass;
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
            this.defaultValue = values[0];
            this.valuesSet = new HashSet<T>(values);
            this.names = values.Select(v => v.ToString().ToLower()).ToArray();
        }
        public override IReadOnlyCollection<T> GetAllPossibleValues()
        {
            return valuesSet;
        }
        public override T GetDefaultValue()
        {
            return defaultValue;
        }
        public override T GetValueFromName(string name)
        {
            name = name.ToUpper();
            foreach (T value in this.valuesSet)
            {
                if (value.ToString().ToUpper() == name)
                {
                    return value;
                }
            }
            return this.defaultValue;
        }
        public override string GetValueName(T value)
        {
            int ordinal = Convert.ToInt32(value);
            return names[ordinal];
        }
    }
}
