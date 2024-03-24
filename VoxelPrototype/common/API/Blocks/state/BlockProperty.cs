using System.Collections.ObjectModel;
using System.Xml.Linq;
namespace VoxelPrototype.common.API.Blocks.state
{
    public abstract class BlockProperty<T>
    {
        private string Name;
        public BlockProperty(string name)
        {
            Name = name;
        }
        public string GetName()
        {
            return Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public abstract T GetDefaultValue();
        public abstract string GetValueName(T value);
        public abstract T GetValueFromName(string name);
        public abstract IReadOnlyCollection<T> GetAllPossibleValues();
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            BlockProperty<T> other = (BlockProperty<T>)obj;
            return Name == other.Name;
        }
        public override string ToString()
        {
            return "BlockStateProperty:" + Name;
        }
    }
}
