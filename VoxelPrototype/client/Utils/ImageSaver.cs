using StbImageSharp;
using StbImageWriteSharp;
namespace VoxelPrototype.client.Utils
{
    public static class ImageSaver
    {
        static ImageWriter Writer = new();
        public static void SaveAsPNG(string Path, ImageResult Image)
        {
            using (var outputStream = File.OpenWrite(Path))
            {
                Writer.WritePng(Image.Data, Image.Width, Image.Height, (StbImageWriteSharp.ColorComponents)Image.Comp, outputStream);
                outputStream.Close();
            }
        }
    }
}
