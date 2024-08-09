namespace VoxelPrototype.api.block.state.properties
{
    public class BooleanProperty : Property<bool>
    {
        public BooleanProperty(string name) : base(name)
        {
        }
        public override bool DefaultValue()
        {
            return false;
        }
        public override string GetValueString(bool value)
        {
            return value ? "true" : "false";
        }
        public override object GetValueFromString(string name)
        {
            return name.ToLower() == "true";
        }
        public override List<object> GetAllValues()
        {
            return new List<object> { false, true };
        }
    }
}
