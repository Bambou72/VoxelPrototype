namespace VoxelPrototype.api.block.state.properties
{
    internal class IntegerProperty : Property<int>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Property");

        int Default;
        int Min, Max;
        public IntegerProperty(string Name , int Min , int Max , int Default) : base(Name)
        {
            if(Math.Abs(Max-Min) > 4096)
            {
                Logger.Warn("Possible values may be to big, this can lead to an increase of ram and cpu usage");
            }
            this.Min = Min;
            this.Max = Max;
            this.Default = Default;
        }
        public override int DefaultValue()
        {
            return Default;
        }
        public override string GetValueString(int value)
        {
            return value.ToString();
        }
        public override object GetValueFromString(string name)
        {
            return int.Parse(name);
        }
        public override List<object> GetAllValues()
        {
            List<object> values = new List<object>();
            for(int i = Min;i<= Max; i++)
            {
                values.Add(i);
            }
            return values;
        }
    }
}
