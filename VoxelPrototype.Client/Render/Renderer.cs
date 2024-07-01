using OpenTK.Graphics.OpenGL;
namespace VoxelPrototype.client.Render
{
    internal class Renderer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private int MaxTextureSize;
        private int MaxTextureLayers;
        internal void Init()
        {
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.DepthTest);
            /*
            MaxTextureSize = GL.GetInteger(GetPName.MaxTextureSize);
            MaxTextureLayers = GL.GetInteger(GetPName.MaxArrayTextureLayers);*/
            Logger.Info($"Max texture size is {MaxTextureSize}");
            Logger.Info($"Max texture array layer size is {MaxTextureLayers}");
        }
    }
}
