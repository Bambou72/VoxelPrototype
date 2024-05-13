using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public struct AspectConstraint : IConstraint
    {
        float Ratio;

        public AspectConstraint(float Ratio)
        {
            this.Ratio = Ratio;
        }

        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            if (Location == ConstraintLocation.Width)
            {
                return (int)(Current.Size.Y * Ratio);
            }
            else if (Location == ConstraintLocation.Height)
            {
                return (int)(Current.Size.X / Ratio);
            }
            return 0;
        }
    }
}
