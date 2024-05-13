using Crecerelle;
using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public struct CenterConstraint : IConstraint
    {
        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            if (Location == ConstraintLocation.X)
            {
                return (Parent != null ? Parent.Size.X : UIManager.ClientSize.X) / 2 - Current.Size.X / 2;
            }
            else if (Location == ConstraintLocation.Y)
            {
                return (Parent != null ? Parent.Size.Y : UIManager.ClientSize.Y) / 2 - Current.Size.Y / 2;
            }
            return 0;
        }
    }
}
