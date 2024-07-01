namespace VoxelPrototype.client.Resources
{
    internal class ResourcesPack : IEquatable<ResourcesPack>
    {
        internal string Name;
        internal string Version;
        internal string Description;
        internal string[] Namespaces;
        internal string Path;

        public bool Equals(ResourcesPack? other)
        {
            if(Name == other.Name && Version == other.Version) return true;
            return false;
        }
    }
}
