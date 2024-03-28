namespace VoxelPrototype.common.API.Blocks.Properties
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
        public override bool GetValueFromString(string name)
        {
            return name == "true";
        }
        public override List<bool> GetAllValues()
        {
            return new List<bool> {false, true };
        }
    }
}
