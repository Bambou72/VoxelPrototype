using OpenTK.Mathematics;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.GUI.Prototype
{
    public class UIElement
    {
        public Vector2 Position = Vector2i.Zero;
        public Vector2 Size = Vector2i.Zero;
        public bool Active = true;

        public virtual void Render(UIRenderer Renderer)
        {
        }
        public virtual void Update(Vector2 MousePos)
        {
        }
        public virtual bool OnMouseMove(MouseMovedEvent Event)
        {
            return false;
        }
        public virtual bool OnWheel(MouseWheelEvent Event)
        {
            return false;
        }
        public virtual bool OnMouseDown(MouseDownEvent Event)
        {
            return false;
        }
        public virtual bool OnMouseUp(MouseUpEvent Event)
        {
            return false;
        }
        public virtual bool OnMouseClicked(MouseClickedEvent Event)
        {
            return false;
        }
        public virtual bool OnKeyDown(KeyDownEvent Event)
        {
            return false;
        }
        public virtual bool OnKeyPressed(KeyPressedEvent Event)
        {
            return false;
        }

    }
}
