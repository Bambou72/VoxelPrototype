namespace VoxelPrototype.utils
{
    public class ResourceID : IEquatable<ResourceID>
    {
        const string Separator = ":";
        const string BaseNamespace = "voxelprototype";
        string Namespace;
        string Path;
        public ResourceID(string path)
        {
            Namespace = BaseNamespace;
            Path = path;
        }
        public static ResourceID FromString(string Value)
        {
            var Split = Value.Split(Separator);
            return new ResourceID(Split[0], Split[1]);
        }
        public ResourceID(string @namespace, string path)
        {
            Namespace = @namespace;
            Path = path;
        }
        public override string ToString()
        {
            return Namespace + Separator + Path;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (!(obj is ResourceID)) return false;
            return Equals((ResourceID)obj);
        }
        public bool Equals(ResourceID? other)
        {
            if (Namespace != other.Namespace) return false;
            if (Path != other.Path) return false;
            return true;
        }
    }
}
