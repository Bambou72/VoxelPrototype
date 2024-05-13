using Crecerelle.Constraints;
using Crecerelle.Renderer;
namespace Crecerelle.Elements
{
    public class UIElementHolder : UIElement
    {
        public List<UIElement> Children = new();
        public List<ConstraintHolder> ChildrenConstraint = new();
        public override void Render(IUIRenderer Renderer)
        {
            foreach (UIElement Child in Children)
            {
                if(Show)
                {
                    Child.Render(Renderer);
                }
            }
            base.Render(Renderer);
        }

        public virtual void AddChild(UIElement UIElement , ConstraintHolder Holder)
        {
            Children.Add(UIElement);
            ChildrenConstraint.Add(Holder);
        }
        public virtual void ChildUpadate(UIManager Manager,ConstraintHolder Holder,UIElement Child)
        {
            if (Holder.Constrained)
            {
                if (Holder.WidthConstraint != null)
                {
                    Child.Size.X = Holder.WidthConstraint.Get(ConstraintLocation.Width, this, Child);
                }
                if (Holder.HeightConstraint != null)
                {
                    Child.Size.Y = Holder.HeightConstraint.Get(ConstraintLocation.Height, this, Child);
                }
                if (Holder.XConstraint != null)
                {
                    Child.Position.X = Position.X + Holder.XConstraint.Get(ConstraintLocation.X, this, Child);
                }
                if (Holder.YConstraint != null)
                {
                    Child.Position.Y = Position.Y + Holder.YConstraint.Get(ConstraintLocation.Y, this, Child);
                }
            }
        }
        public override void Update(UIManager Manager)
        {
            float zorder = ZOrder + 0.0001f;
            for(int i = 0;i< ChildrenConstraint.Count; i++)
            {
                var Child = Children[i];
                Child.ZOrder = zorder;
                ChildUpadate(Manager, ChildrenConstraint[i], Child);
                Child.Update(Manager);                
            }
            base.Update(Manager);
        }
        
    }
}
