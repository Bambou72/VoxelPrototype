using ImmediateUI.immui.drawing;
using ImmediateUI.immui.math;
namespace ImmediateUI.immui
{
    internal class Window
    {
        internal ImmuiDrawList DrawList =new();
        internal string Name;
        internal ulong ID;
        internal Rect Rect = new(0,0,200,300);

        public Window()
        {
            DrawList.ResetForNewFrame();
            DrawList.PushClipRectFullScreen();
            DrawList.PushTextureID(ImmediateUI.Window.Blank.GetHandle());

        }
    }
}
