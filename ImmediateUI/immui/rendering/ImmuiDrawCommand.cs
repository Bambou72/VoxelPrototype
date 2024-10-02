using ImmediateUI.immui.math;
namespace ImmediateUI.immui.rendering
{
    public struct ImmuiDrawCommand
    {
        internal int TextureID;
        internal int Offset;
        internal int Count;
        internal Rect ClipRect;
        public bool Equals(ImmuiDrawCommand other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }
    }
    public struct ImmuiDrawCommandHeader :  IEquatable<ImmuiDrawCommand>
    {
        internal Rect ClipRect;
        internal int TextureID;

        public bool Equals(ImmuiDrawCommand other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }
    };
}
