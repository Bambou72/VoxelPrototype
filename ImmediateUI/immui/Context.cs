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
            BaseFont = FontLoader.LoadFromMemory(FontData.UncutSans_Regular),
            Colors = new()
            {
                { "Text", 0xFFFFFFFF },
                { "Frame", 0x304567FF },
                { "WindowHeader", 0x101527FF },
                { "WindowButton", 0xFF1527FF },
            }
        };
        internal ulong ActiveID;
        internal ulong HotID;
        internal ulong CurrentID;
        internal Vector2i MousePosition;
        internal  Vector2 ScreenSize;
        internal List<ulong> IDStack = new();
        internal ImmuiDrawList MainDrawList;
        internal DrawData DrawData = new DrawData();
        internal Window CurrentWindow;
        internal Dictionary<ulong, Window> Windows =new();
        internal bool MouseCaptured = false;
        public Context()
        {
            ImmuiDrawList.Init();
            MainDrawList = new ImmuiDrawList();
        }
    }
}
