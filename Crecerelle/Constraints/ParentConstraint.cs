using Crecerelle;
using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public struct ParentConstraint : IConstraint
    {
        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            if (Location == ConstraintLocation.X)
            {
                return 0;
            }
            else if (Location == ConstraintLocation.Y)
            {
                return 0;
            }
            else if (Location == ConstraintLocation.Width)
            {
                return Parent != null ? Parent.Size.X : UIManager.ClientSize.X;
            }
            else if (Location == ConstraintLocation.Height)
            {
                return Parent != null ? Parent.Size.Y : UIManager.ClientSize.Y;
            }
            return 0;
        }
    }
}
