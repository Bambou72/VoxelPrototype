namespace VoxelPrototype.client.Resources
{
    public class Resource
    {
        Stream Stream;
        string path;
        public Resource(Stream stream,string path)
        {
            Stream = stream;
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
        public void Close()
        {
            Stream.Close();
        }
    }
}
