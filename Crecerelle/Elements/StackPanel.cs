using Crecerelle.Constraints;
using Crecerelle.Renderer;
namespace Crecerelle.Elements
{
    public class StackPanel : UIElementHolder
    {
        public int Padding;
        public int Pos;
        public StackPanel(int padding)
        {
            LockInput = false;
            Padding = padding;
        }
        public override void Render(IUIRenderer Renderer)
        {
            base.Render(Renderer);
            //Renderer.RenderQuad(new OpenTK.Mathematics.Vector3(Position.X, Position.Y, ZOrder), Size, new Utils.Color(1, 0, 0, 1));
        }
        public override void Update(UIManager Manager)
        {
            Pos = 0;
            base.Update(Manager);
        }
        public override void ChildUpadate(UIManager Manager,ConstraintHolder Holder,  UIElement Child)
        {
            base.ChildUpadate(Manager, Holder, Child);
            Child.Position.Y = Position.Y+ Pos;
            Pos += Child.Size.Y + Padding;
        }
    }
}
