using Crecerelle.Renderer;
using Crecerelle.Utils;
using OpenTK.Mathematics;
namespace Crecerelle.Elements
{
    public class UIElement
    {
        public Vector2i Position = Vector2i.Zero;
        public Vector2i Size = Vector2i.Zero;
        public float ZOrder;
        public bool LockInput = true;
        public bool Show = true;
        public bool Hovered;
        public virtual void Render(IUIRenderer Renderer){}
        public virtual void Update(UIManager Manager)
        {
            if (LockInput && !Manager.InputCaptured && MouseCheck.IsHovering(Position,Position+Size))
            {
                Manager.InputCaptured = true;
                OnHovered(Manager);
            }
        }
        public virtual void OnHovered(UIManager Manager){}
    }
}
