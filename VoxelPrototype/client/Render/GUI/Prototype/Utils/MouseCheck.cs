using OpenTK.Mathematics;

namespace VoxelPrototype.client.Render.GUI.Prototype.Utils
{
    public static class MouseCheck
    {
        public static bool IsHovering(Vector2 MousePos, Vector2 Start, Vector2 End)
        {
            if (MousePos.X < Start.X || MousePos.X > End.X ||
                MousePos.Y < Start.Y || MousePos.Y > End.Y)
            {
                return false;
            }
            return true;
        }
    }
}
