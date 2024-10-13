using ImmediateUI.immui.math;
using ImmediateUI.immui.rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace ImmediateUI.immui
{
    public  static class Immui
    {
        public static bool Button(Context Ctx, string Label, Rect Rect)
        {
            //Preprocess
            bool Pressed = false;
            ulong ID = Ctx.GetID(Label);
            string RealLabel = Ctx.GetLabel(Label);
            //Widget base
            (bool Hot, bool Active) = Ctx.BaseWiget(Rect, ID);
            if (!Hot && Active)
            {
                Ctx.SetActiveID(0);
            }
            //Logic
            if (Active && Ctx.MouseUp(0))
            {
                Ctx.SetActiveID(0);
                Pressed = true;
            }
            //Rendering
            var DrawList = Ctx.GetDrawList();
            DrawList.AddRect(Rect, (Hot ?  0x509940FFu: 0x357030FF));            
            Vector2 TSize = Ctx.CalculateTextSize(20, RealLabel);
            DrawList.AddText(new(Rect.CenterX - TSize.X/2,Rect.CenterY + TSize.Y / 2 ), RealLabel, 20, 0xFFFFFFFF,Ctx.font);
            //Result
            return Pressed;
        }
        public static bool Slider(Context Ctx , string Label , Rect Rect , ref float Value,float Min , float Max , float Step = 1)
        {
            //Preprocess
            ulong ID = Ctx.GetID(Label);
            string RealLabel = Ctx.GetLabel(Label);
            Vector2 LSize = Ctx.CalculateTextSize(20, RealLabel);
            Rect LabelPart = new(Rect.X, Rect.Y, (int)LSize.X + 10, Rect.H);
            Rect SliderPart = new(Rect.X + (int)LSize.X + 10, Rect.Y, Rect.W - (int)LSize.X - 10, Rect.H);
            //Widget base
            (bool Hot, bool Active) = Ctx.BaseWiget(SliderPart, ID);
            if (!Ctx.MouseDown(0) &&Active)
            {
                Ctx.SetActiveID(0);
            }
            //Logic
            if(Active)
            {
                float RelativeMousePos =Math.Max(0, Math.Min(Ctx.MousePosition.X - SliderPart.X, SliderPart.W)) / (float)SliderPart.W;
                Value = Min + RelativeMousePos * (Max - Min);
                Value = (int)Math.Round(Value / Step) * Step;
                Value = Math.Max(Min, Math.Min(Value, Max));
            }
            int xpos = (int)((SliderPart.W - 6) * ((Value - Min) / (Max - Min)));
            //Rendering
            var DrawList = Ctx.GetDrawList();
            //Label
            DrawList.AddText(new(LabelPart.CenterX - LSize.X / 2, LabelPart.CenterY + LSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, Ctx.font);
            //
            DrawList.AddRect(SliderPart, (Hot ? 0x509940FFu : 0x357030FF));
            DrawList.AddRect(new(SliderPart.X +3, SliderPart.Y+3, xpos, SliderPart.H-6), 0x409030FF);
            Vector2 TSize = Ctx.CalculateTextSize(20, Value.ToString());
            DrawList.AddText(new(SliderPart.CenterX - TSize.X / 2, SliderPart.CenterY + TSize.Y / 2), Value.ToString(), 20, 0xFFFFFFFF, Ctx.font);

            return Active;
        }
        public static bool TextEdit(Context Ctx,string Label,ref string Text, Rect Rect)
        {
            //Preprocess
            string RealLabel = Ctx.GetLabel(Label);
            Vector2 TSize = Ctx.CalculateTextSize(20, RealLabel);
            Rect LabelPart = new(Rect.X,Rect.Y,(int)TSize.X +10,Rect.H);
            Rect TextEditPart = new(Rect.X+(int)TSize.X + 10, Rect.Y, Rect.W- (int)TSize.X - 10, Rect.H);
            ulong ID = Ctx.GetID(Label);
            //Widgetbase
            (bool Hot, bool Active) = Ctx.BaseWiget(TextEditPart, ID);
            if(Active)
            {
                Ctx.SetKeyFocusID(ID);
            }
            if (!Hot && Ctx.GetActiveID() == ID)
            {
                Ctx.SetActiveID(0);
            }
            bool KeyFocused = ID == Ctx.GetKeyFocusID();
            bool Result = false;
            //Logic
            if(KeyFocused)
            {
                if(Ctx.KeyPressed((int)Keys.Backspace) && Text.Length >0)
                {
                    Text = Text[..^1];
                    Result = true;
                }
                if(Ctx.InputedChar != 0)
                {
                    Text += Ctx.InputedChar;
                    Ctx.InputedChar = (char)0;
                    Result = true;

                }
            }
            //Rendering
            var DrawList = Ctx.GetDrawList();
            if(KeyFocused)
            {
                DrawList.AddRect(TextEditPart, 0x807233FF);

            }
            else if (Hot)
            {
                DrawList.AddRect(TextEditPart, 0x509940FF);
            }
            else
            {
                DrawList.AddRect(TextEditPart, 0x357030FF);
            }
            //Label
            DrawList.AddText(new(LabelPart.CenterX - TSize.X / 2, LabelPart.CenterY + TSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, Ctx.font);
            //Text
            Vector2 TextSize = Ctx.CalculateTextSize(20, Text);
            DrawList.AddText(new(TextEditPart.X + 10, LabelPart.CenterY + TextSize.Y / 2), Text, 20, 0xFFFFFFFF, Ctx.font,TextEditPart);
            //Result
            return Result;
        }
        public static void Panel(Context Ctx,Rect PanelRect , uint Color)
        {
            Renderer DrawList = Ctx.GetDrawList();
            DrawList.AddRect(PanelRect, Color);
        }
        public static Rect BeginScrollPanel(Context Ctx,Rect PanelRect , ref int ScrollOffset,int Size,uint Color = 0x00000000)
        {
            const int ScrollHandleSize = 14;
            int SliderWidth = Size > PanelRect.H ? 20:0 ;
            //
            Rect SliderRect = new(PanelRect.XW - SliderWidth, PanelRect.Y, SliderWidth, PanelRect.H);
            int ScrollerSize = Math.Min((int)((float)PanelRect.H / Size * PanelRect.H), PanelRect.H);
            int Ypos = (int)((float)ScrollOffset / Size * PanelRect.H);
            Rect ScrollHandle = new(SliderRect.X + (SliderWidth - ScrollHandleSize)/2, SliderRect.Y + Ypos, ScrollHandleSize, ScrollerSize);
            //Logic
            //TODO : Rework scroller system for better scrolling when big size
            if (Ctx.IsHover(PanelRect))
            {
                ScrollOffset -= Ctx.ScrollDelta.Y * 20;
                if (Ctx.IsHover(ScrollHandle))
                {
                    ScrollOffset += Ctx.DragDistance.Y;
                }
                ScrollOffset = Math.Min(Size - PanelRect.H, Math.Max(ScrollOffset, 0));
            }
            Ctx.PushClipRect(PanelRect);
            //Rendering
            Renderer DrawList = Ctx.GetDrawList();
            DrawList.PushClipRect(new(PanelRect.X, PanelRect.Y, PanelRect.W, PanelRect.H));
            Panel(Ctx, PanelRect, Color);
            DrawList.AddRect(SliderRect, 0x996040FF);
            DrawList.AddRect(ScrollHandle, (Ctx.IsHover(ScrollHandle) ? 0x509940FFu :  0x405930FF));
            //Return
            return new(PanelRect.X, PanelRect.Y-ScrollOffset, PanelRect.W-SliderWidth, PanelRect.H);
        }
        public static void EndScrollPanel(Context Ctx)
        {
            Renderer DrawList = Ctx.GetDrawList();
            DrawList.PopClipRect();
            Ctx.PopClipRect();
        }
    }
}
