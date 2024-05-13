using Crecerelle;
using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public struct RelativeConstraint : IConstraint
    {
        float Value;
        public RelativeConstraint(float Value)
        {
            this.Value = Value;
        }
        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            if (Location == ConstraintLocation.X)
            {
                return (int)((Parent != null ? Parent.Size.X : UIManager.ClientSize.X) * Value);
            }
            else if (Location == ConstraintLocation.Y)
            {
                return (int)((Parent != null ? Parent.Size.Y : UIManager.ClientSize.Y) * Value);
            }
            else if (Location == ConstraintLocation.Width)
            {
                return (int)((Parent != null ? Parent.Size.X : UIManager.ClientSize.X) * Value);
            }
            else if (Location == ConstraintLocation.Height)
            {
                return (int)((Parent != null ? Parent.Size.Y : UIManager.ClientSize.Y) * Value);
            }
            return 0;
        }
    }
}
