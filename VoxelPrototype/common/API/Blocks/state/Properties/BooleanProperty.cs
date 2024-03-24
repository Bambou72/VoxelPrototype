using System.Collections.ObjectModel;
using VoxelPrototype.common.Utils;
namespace VoxelPrototype.common.API.Blocks.state.Properties
{
    internal class BooleanProperty : BlockProperty<bool>
    {
        public BooleanProperty(string name) : base(name)
        {
        }
        public override bool GetDefaultValue()
        {
            return false;
        }
        public override string GetValueName(bool value)
        {
            return value ? "true" : "false";
        }
        public override bool GetValueFromName(string name)
        {
            return name == "true";
        }
        public override IReadOnlyCollection<bool> GetAllPossibleValues()
        {
            return (IReadOnlyCollection<bool>)BooleanSet.IMMUTABLE;
        }
    }
}
