using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public struct PixelConstraint : IConstraint
    {
        int Pixel;
        public PixelConstraint(int Pixel)
        {
            this.Pixel = Pixel;
        }
        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current)
        {
            return Pixel;
        }
    }
}
