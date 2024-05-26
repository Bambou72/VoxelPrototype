using OpenTK.Graphics.OpenGL4;

namespace VoxelPrototype.client.Render.Components
{
    public interface ITexture
    {
        public void Use(TextureUnit unit);
        public void Clean();

    }
}
