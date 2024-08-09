using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.client.utils.StbImageSharp;

namespace VoxelPrototype.client.rendering.texture
{
    public class StoreTexture : ITexture
    {
        public StoreTexture(ImageResult imageResult)
        {
            ImageResult = imageResult;
        }
        internal ImageResult ImageResult { get; set; }
        public void Clean()
        {
            ImageResult = null;
        }

        public int GetHandle()
        {
            return 0;
        }

        public void Use(TextureUnit unit)
        {
        }
    }
}
