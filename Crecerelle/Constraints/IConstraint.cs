using Crecerelle.Elements;

namespace Crecerelle.Constraints
{
    public interface IConstraint
    {
        /// <summary>
        /// Get value in pixel
        /// </summary>
        /// <param name="ThisSize">Is the size of the ui element</param>
        /// <param name="Size">Is the size of the parent</param>
        /// <returns></returns>
        public int Get(ConstraintLocation Location, UIElement Parent, UIElement Current);
    }
}
