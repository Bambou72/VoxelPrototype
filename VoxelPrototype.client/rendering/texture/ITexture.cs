using OpenTK.Graphics.OpenGL4;

namespace VoxelPrototype.client.rendering.texture
{
    public interface ITexture
    {
        public void Use(TextureUnit unit);
        public void Clean();

    }
}
