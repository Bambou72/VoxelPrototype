using ImmediateUI.immui.math;
using OpenTK.Mathematics;
namespace ImmediateUI.immui
{
    public static partial class Immui
    {
        public static bool Button(string Label, Rect Rect)
        {
            bool result = false;
            ulong ID = GetID(Label);
            if (CheckMouse(Rect))
            {
                CurrentContext.HotID = ID;
                if (MouseDown(0) && CurrentContext.ActiveID != ID)
                {
                    CurrentContext.ActiveID = ID;
                }
            }
            else if (CurrentContext.ActiveID == ID)
            {
                CurrentContext.ActiveID = 0;
            }
            var DrawList = GetCurrentDrawList();
            var Style = GetCurrentStyle();
            if (CurrentContext.ActiveID == ID && !MouseDown(0) && CurrentContext.HotID == ID)
            {
                result = true;
                CurrentContext.ActiveID = 0;
            }
            if (CurrentContext.HotID == ID)
            {
                DrawList.AddRectFilled(new(Rect.X,Rect.Y), new(Rect.XW, Rect.YH), 0x102030FF);
            }
            else
            {
                DrawList.AddRectFilled(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), 0x051020FF);
            }
            Vector2 TSize = CalculateTextSize(Style.FontSize, Label);

            DrawList.AddText(new(Rect.CenterX - TSize.X/2,Rect.CenterY + TSize.Y / 2 ), Label, Style.FontSize, Style.Colors["Text"]);
            return result;
        }
        /*
        public static void TextInput(string Label, Rect Rect, ref string inputText)
        {
            ulong ID = GetID(Label);
            if (CheckMouse(Rect))
            {
                CurrentContext.HotID = ID;

                if (IO.IsMouseDown(0) && CurrentContext.ActiveID == 0)
                {
                    CurrentContext.ActiveID = ID;
                }
            }
            if (CurrentContext.ActiveID == ID)
            {
                if (inputText.Length > 0 && IO.BackspacePressed)
                {
                    inputText = inputText.Remove(inputText.Length - 1);
                }
                else
                {
                    foreach (char ch in IO.InputedChars)
                    {
                        inputText += ch;
                    }
                }
            }
            var DrawList = GetCurrentDrawList();
            var Style = GetCurrentStyle();
            Vector2 TSize = CalculateTextSize(Style.FontSize, Label);
            float Height = Math.Min(TSize.Y + Style.Padding.Y * 2, Rect.H);
            Vector2 Max = new(Rect.XW, Rect.Y + Height);
            if (CurrentContext.ActiveID == ID)
            {
                DrawList.AddRectFilled(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), 0x204060FF, 0);
            }
            else if (CurrentContext.HotID == ID)
            {
                DrawList.AddRectFilled(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), 0x102030FF, 0);
            }
            else
            {
                DrawList.AddRectFilled(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), 0x051020FF, 0);
            }
            DrawList.AddText(new Vector2(Rect.X + Style.Padding.X, (Rect.Y + Height / 2)) + new Vector2(0, TSize.Y) / 2, inputText, Style.FontSize, Style.Colors["Text"], CpuClip: new Rect(Rect.X,Rect.Y,Rect.W - Style.Padding.Z, (int)Height));
        }
        /*
        public static void BeginWindow(string Name,Vector2i Position =default,Vector2i Size = default)
        {
            ulong ID = GetID(Name);
            PushGeneratedID(ID);
            Window Wind;
            if(CurrentContext.Windows.ContainsKey(ID))
            {
                Wind = CurrentContext.Windows[ID];
            }else
            {
                Wind = new Window() { ID =ID,Name = Name};
                CurrentContext.Windows.Add(ID,Wind);
            }
            if(Position != default)
            {
                Wind.Rect.Position = Position;
            }
            CurrentContext.CurrentWindow = Wind;
            if(Size != default)
            {
                Wind.Rect.Size = Size;
            }
            if (CheckMouse(new(Wind.Rect.Position, new Vector2(Wind.Rect.Size.X, 30))))
            {
                if (IO.IsMouseDown(MouseButtons.Left))
                {
                    Wind.Rect.Position += IO.Drag;
                }
                //if(CheckMouse())
            }
            var DrawList = GetCurrentDrawList();
            var Style = GetCurrentStyle();
            DrawList.AddRectFilled(Wind.Rect.Position, Wind.Rect.Position+new Vector2(Wind.Rect.Size.X, 30), Style.Colors["WindowHeader"]);
            DrawList.AddRectFilled(Wind.Rect.Position + new Vector2(Wind.Rect.Size.X - 25,5), Wind.Rect.Position+new Vector2(Wind.Rect.Size.X -5, 25) , Style.Colors["WindowButton"]);
            DrawList.AddRectFilled(Wind.Rect.Position + new Vector2(0, 30), Wind.Rect.Max, Style.Colors["Frame"]);
        }
        public static void EndWindow()
        {
            CurrentContext.CurrentWindow = null;
            PopID();
        }*/
    }
}
