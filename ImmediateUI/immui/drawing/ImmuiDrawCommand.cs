using ImmediateUI.immui.math;

namespace ImmediateUI.immui.drawing
{
    public struct ImmuiDrawCommand
    {
        internal int TextureID;
        internal int Offset;
        internal int Count;
        internal Rect ClipRect;

        public ImmuiDrawCommand()
        {
        }

        public bool Equals(ImmuiDrawCommand other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }
    }
    public struct ImmuiDrawCommandHeader : IEquatable<ImmuiDrawCommandHeader>, IEquatable<ImmuiDrawCommand>
    {
        internal Rect ClipRect;
        internal int TextureID;

        public bool Equals(ImmuiDrawCommandHeader other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }

        public bool Equals(ImmuiDrawCommand other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }
    };
}
