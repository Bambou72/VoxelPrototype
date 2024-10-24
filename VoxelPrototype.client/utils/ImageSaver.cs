using VoxelPrototype.client.utils.StbImage;

namespace VoxelPrototype.client.utils
{
    public static class ImageSaver
    {
        static ImageWriter Writer = new();
        public static void SaveAsPNG(string Path, ImageResult Image)
        {
            using (var outputStream = File.OpenWrite(Path))
            {
                Writer.WritePng(Image.Data, Image.Width, Image.Height,Image.Comp, outputStream);
                outputStream.Close();
            }
        }
    }
}
