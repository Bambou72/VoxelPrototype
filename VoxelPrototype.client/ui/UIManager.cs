using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui
{
    public class UIManager
    {
        internal UIRenderer Renderer;
        UIScreen Screen = null;
        public UIManager()
        {
            Renderer = new();
        }
        public void SetCurrentScreen(UIScreen? screen)
        {
            Screen = screen;
            if (screen != null)
            {
                Screen.ComputeLayout();
            }
        }

        public void Update()
        {
            if (Screen != null)
            {
                Screen.ComputeLayout();
                Screen.Update(Client.TheClient.MouseState, Client.TheClient.KeyboardState);
            }
        }
        public void OnResize()
        {
            Screen.ComputeLayout();
        }
        public void Render()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            if (Screen != null)
            {
                Screen.Render(Renderer, Client.TheClient.ClientSize, Matrix4.CreateOrthographicOffCenter(0, Client.TheClient.ClientSize.X, 0, Client.TheClient.ClientSize.Y, 0, 1));
            }
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);

        }
    }
}
