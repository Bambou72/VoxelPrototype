using ImmediateUI.immui.math;
using OpenTK.Mathematics;
namespace ImmediateUI.immui
{
    public  static class Immui
    {
        public static bool Button(Context Ctx, string Label, Rect Rect)
        {
            bool result = false;
            ulong ID = Ctx.GetID(Label);
            if (Ctx.CheckMouse(Rect))
            {
                Ctx.HotID = ID;
                if (Ctx.MouseDown(0) && Ctx.ActiveID != ID)
                {
                    Ctx.ActiveID = ID;
                }
            }
            else if (Ctx.ActiveID == ID)
            {
                Ctx.ActiveID = 0;
            }
            var DrawList = Ctx.GetDrawList();
            if (Ctx.ActiveID == ID && !Ctx.MouseDown(0) && Ctx.HotID == ID)
            {
                result = true;
                Ctx.ActiveID = 0;
            }
            if (Ctx.HotID == ID)
            {
                DrawList.AddRect(Rect, 0x509940FF);
            }
            else
            {
                DrawList.AddRect(Rect, 0x357030FF);
            }
            Vector2 TSize = Ctx.CalculateTextSize(20, Label);

            DrawList.AddText(new(Rect.CenterX - TSize.X/2,Rect.CenterY + TSize.Y / 2 ), Label, 20, 0xFFFFFFFF,Ctx.font);
            return result;
        }
        /*
        public static void TextInput(string Label, Rect Rect, ref string inputText)
        {
            ulong ID = GetID(Label);
            if (CheckMouse(Rect))
            {
                Ctx.HotID = ID;

                if (IO.IsMouseDown(0) && Ctx.ActiveID == 0)
                {
                    Ctx.ActiveID = ID;
                }
            }
            if (Ctx.ActiveID == ID)
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
            if (Ctx.ActiveID == ID)
            {
                DrawList.AddRectFilled(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), 0x204060FF, 0);
            }
            else if (Ctx.HotID == ID)
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
            if(Ctx.Windows.ContainsKey(ID))
            {
                Wind = Ctx.Windows[ID];
            }else
            {
                Wind = new Window() { ID =ID,Name = Name};
                Ctx.Windows.Add(ID,Wind);
            }
            if(Position != default)
            {
                Wind.Rect.Position = Position;
            }
            Ctx.CurrentWindow = Wind;
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
            Ctx.CurrentWindow = null;
            PopID();
        }*/
    }
}
