
using OpenTK.Mathematics;

namespace Crecerelle.Utils
{
    public static class MouseCheck
    {
        public static bool IsHovering(Vector2i Start, Vector2i End)
        {
            if (UIManager.MousePos.X < Start.X || UIManager.MousePos.X > End.X ||
                UIManager.MousePos.Y < Start.Y || UIManager.MousePos.Y > End.Y)
            {
                return false;
            }
            return true;
        }
    }
}
