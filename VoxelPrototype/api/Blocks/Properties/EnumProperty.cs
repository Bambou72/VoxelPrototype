namespace VoxelPrototype.api.Blocks.Properties
{

    public class EnumProperty<T> : Property<T> where T : struct, Enum
    {
        private static readonly T[] EnumValues = (T[])Enum.GetValues(typeof(T));
        public EnumProperty(string name) : base(name)
        {
        }
        public override List<T> GetAllValues()
        {
            return new List<T>((T[])Enum.GetValues(typeof(T)));
        }
        public override T DefaultValue()
        {
            return EnumValues[0];
        }
        public override T GetValueFromString(string name)
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
