namespace VoxelPrototype.client.Resources
{
    public class Resource
    {
        Stream Stream;
        string path;
        public Resource( string path)
        {
            this.path = path;
        }

        public Stream GetStream()
        {
            return Stream;
        }
        public string GetPath()
        {
            return path;
        }
        public void Open()
        {
            Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }
        public void Close()
        {
            Stream.Close();
        }
    }
}
