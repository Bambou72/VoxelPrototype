using OpenTK.Graphics.OpenGL;

namespace VoxelPrototype.client.Render.Components
{
    public interface ITexture
    {
        public void Use(TextureUnit unit);
        public void Clean();

    }
}
