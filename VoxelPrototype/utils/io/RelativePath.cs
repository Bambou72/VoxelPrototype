namespace VoxelPrototype.utils.io
{
    public static class RelativePath
    {
        public static string[] GetRelativePathsDirectories(string baseDir)
        {
            string[] directories = Directory.GetDirectories(baseDir);
            string[] relativePaths = new string[directories.Length];

            for (int i = 0; i < directories.Length; i++)
            {
                relativePaths[i] = GetRelativePath(baseDir, directories[i]);
            }

            return relativePaths;
        }
        public static string GetRelativePathFile(string baseDir, string fullPath)
        {
            string absoluteBaseDir = Path.GetFullPath(baseDir);
            string absoluteFullPath = Path.GetFullPath(fullPath);
            if (!absoluteFullPath.StartsWith(absoluteBaseDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Le chemin complet ne commence pas par le répertoire de base.");
            }
            string relativePath = absoluteFullPath.Substring(absoluteBaseDir.Length).TrimStart(Path.DirectorySeparatorChar);
            string directory = Path.GetDirectoryName(relativePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativePath);
            string specificPart = Path.Combine(directory, fileNameWithoutExtension);
            return specificPart.Replace(Path.DirectorySeparatorChar, '/');
        }
        public static string GetRelativePath(string baseDir, string fullPath)
        {
            if (!fullPath.StartsWith(baseDir))
            {
                throw new InvalidOperationException();
            }

            string relativePath = fullPath.Replace(baseDir, "");
            relativePath = relativePath.Replace("\\", "");
            return relativePath;
        }
    }
}
