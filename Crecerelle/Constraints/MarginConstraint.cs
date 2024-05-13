using Crecerelle.Elements;
using OpenTK.Mathematics;

namespace Crecerelle.Constraints
{
    public enum MarginDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public struct MarginConstraint : IConstraint
    {
        MarginDirection MarginDirection;
        float Value;
        bool Relative;

        public MarginConstraint(MarginDirection MarginDirection, float Value, bool Relative)
        {
            this.MarginDirection = MarginDirection;
            this.Value = Value;
            this.Relative = Relative;
        }

        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            Vector2i ParentSize = Parent != null ? Parent.Size : UIManager.ClientSize;
            int ExactValue = (int)Value;
            if (Relative)
            {
                if (Location == ConstraintLocation.X || Location == ConstraintLocation.Width)
                {
                    ExactValue = (int)(ParentSize.X * Value);

                }
                else
                {
                    ExactValue = (int)(ParentSize.Y * Value);
                }
            }
            if (Location == ConstraintLocation.X)
            {
                if (MarginDirection == MarginDirection.Left)
                {
                    return ExactValue;
                }
                else if (MarginDirection == MarginDirection.Right)
                {
                    return ParentSize.X - Current.Size.X - ExactValue;
                }
            }
            else if (Location == ConstraintLocation.Y)
            {
                if (MarginDirection == MarginDirection.Top)
                {
                    return ExactValue;
                }
                else if (MarginDirection == MarginDirection.Bottom)
                {
                    return ParentSize.Y - Current.Size.Y - ExactValue;
                }
            }
            else if (Location == ConstraintLocation.Width)
            {
                if (MarginDirection == MarginDirection.Right)
                {
                    return ParentSize.X - ExactValue;
                }
            }
            else if (Location == ConstraintLocation.Y)
            {
                if (MarginDirection == MarginDirection.Bottom)
                {
                    return ParentSize.Y - ExactValue;
                }
            }
            return 0;
        }
    }
}
