using OpenTK.Graphics.OpenGL4;
namespace VoxelPrototype.client.Render
{
    internal static class Renderer
    {
        internal static void Init()
        {
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Multisample);
        }
    }
}
