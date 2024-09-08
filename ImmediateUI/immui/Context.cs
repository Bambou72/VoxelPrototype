using ImmediateUI.immui.data;
using ImmediateUI.immui.drawing;
using ImmediateUI.immui.font;
using OpenTK.Mathematics;
namespace ImmediateUI.immui
{
    public class Context
    {
        internal Style Style = new Style()
        {
            BaseFont = FontLoader.LoadFromMemory(FontData.UncutSans_Regular)
        };
        internal ulong ActiveID;
        internal ulong HotID;
        internal ulong LastHotID;
        internal ulong CurrentID;
        internal Vector2i MousePosition;
        internal  Vector2 ScreenSize;
        internal List<ulong> IDStack = new();
        internal bool[] MouseButtons = new bool[3];
        internal List<char> InputedChars = new();
        internal ImmuiDrawListSharedData SharedData = new();
        internal ImmuiDrawList MainDrawList;
        internal DrawData DrawData = new DrawData();
        public Context()
        {
            MainDrawList = new(SharedData);
        }
    }
}
