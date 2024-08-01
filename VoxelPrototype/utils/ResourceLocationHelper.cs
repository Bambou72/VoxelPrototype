namespace VoxelPrototype.utils
{
    public static class ResourceLocationHelper 
    {
        const string Separator = ":";
        const string BaseNamespace = "voxelprototype";
        public static string GetNamespace(string ResourceLocation)
        {
            return ResourceLocation.Split(Separator)[0];
        }
        public static string GetPath(string ResourceLocation)
        {
            return ResourceLocation.Split(Separator)[1];
        }
        public static string GetPathWithoutLast(string ResourceLocation)
        {
            int LastIndexOfSlash  = ResourceLocation.LastIndexOf('/');
            return ResourceLocation.Substring(0, LastIndexOfSlash);
        }
        public static string GetResourceName(string ResourceLocation)
        {
            int LastIndexOfSlash = ResourceLocation.LastIndexOf('/');
            return ResourceLocation.Substring(LastIndexOfSlash+ 1, ResourceLocation.Length);
        }
    }
}
