using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui.elements
{
    public class Element
    {
        internal Vector2i Position;
        internal Vector2i Size;
        internal Margin Padding = new(20,20,10,10);

        public bool ParentSizing = true;
        public bool IsHovered
        {
            get
            {
                Vector2 MousePos = Client.TheClient.MousePosition;
                if (MousePos.X < Position.X || MousePos.X > Position.X + Size.X || MousePos.Y < Position.Y || MousePos.Y > Position.Y + Size.Y)
                {
                    return false;
                }
                return true;
            }
        }

        public virtual void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
        }
        public virtual void Update(MouseState MState, KeyboardState KSate)
        {
        }
    }
}
