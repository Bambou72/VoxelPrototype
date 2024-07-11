namespace VoxelPrototype.utils
{
    public struct Identifier : IEquatable<Identifier>
    {
        const char Separator = '/';
        const char NamespaceSeparator = ':';
        const string BaseNamespace = "voxelprototype";

        public string Namespace;
        public string Path;

        public Identifier(string @Path)
        {
            Namespace = BaseNamespace;
            this.Path = @Path;
        }
        public Identifier(string @namespace, string path)
        {
            Namespace = @namespace;
            Path = path;
        }
        public static Identifier FromString(string String)
        {
            return new(String.Split(NamespaceSeparator)[0], String.Split(NamespaceSeparator)[1]);
        }

        public bool Equals(Identifier other)
        {
            if (Namespace != other.Namespace) return false;
            if (Path != other.Path) return false;
            return true;
        }
        public override bool Equals(object? obj)
        {
            if (obj is Identifier) return Equals((Identifier)obj);
            return false;
        }
        public override string ToString()
        {
            return Namespace +":"+ Path;
        }
    }
}

