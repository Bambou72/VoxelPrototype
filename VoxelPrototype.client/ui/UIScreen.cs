using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.elements.container;

namespace VoxelPrototype.client.ui
{
    public class UIScreen : Container
    {
        internal UIScreen Parent;
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            Position = Vector2i.Zero;
            Size = Client.TheClient.ClientSize;
            if(KSate.IsKeyDown(Keys.Escape))
            {
                if(Parent != null)
                {
                    Close();
                }
            }
            base.Update(MState, KSate);
        }
        internal void Close()
        {
            Client.TheClient.UIManager.SetCurrentScreen(null);
            Client.TheClient.UIManager.SetCurrentScreen(Parent);
        }
    }
}
