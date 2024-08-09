namespace VoxelPrototype.api.block.state.properties
{

    public class EnumProperty<T> : Property<T> where T : struct, Enum
    {
        private static readonly T[] EnumValues = (T[])Enum.GetValues(typeof(T));
        public EnumProperty(string name) : base(name)
        {
        }
        public override List<object> GetAllValues()
        {
            var Values = Enum.GetValues(typeof(T));
            // Convert to list
            List<object> FinalValues = [.. Values];
            return FinalValues;
        }
        public override T DefaultValue()
        {
            return EnumValues[0];
        }
        public override object GetValueFromString(string name)
        {

            if (Enum.TryParse(name, true, out T value))
            {
                return value;
            }
            return DefaultValue();
        }
        public override string GetValueString(T value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}
