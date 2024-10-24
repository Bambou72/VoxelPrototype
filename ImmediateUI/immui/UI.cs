//888     888 8888888
//888     888   888
//888     888   888
//888     888   888
//888     888   888
//888     888   888
//Y88b. .d88P   888
// "Y88888P"  8888888
using NLog.Common;
using NLog.LayoutRenderers.Wrappers;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using VoxelPrototype.client.font;
using VoxelPrototype.client.utils.math;
namespace ImmediateUI.immui
{
    //  ___         _           _   
    // / __|___ _ _| |_ _____ _| |_ 
    //| (__/ _ \ ' \  _/ -_) \ /  _|
    // \___\___/_||_\__\___/_\_\\__|
    public class Context
    {
        //Input
        public Func<int, bool> MouseDown;
        public Func<int, bool> MouseUp;
        public Func<int, bool> MousePressed;
        public Func<int, bool> KeyDown;
        public Func<int, bool> KeyUp;
        public Func<int, bool> KeyPressed;
        internal Vector2i MousePosition;
        internal Vector2i ScrollDelta;
        internal Vector2 ScreenSize;
        public float DT;
        internal char InputedChar = (char)0;
        //Font
        internal Font font = FontLoader.LoadFromFile("Roboto-Regular.ttf");
        //State
        private ulong CurrentTick;
        private ulong ActiveID;
        private ulong LastActiveID;
        private ulong HotID;
        private ulong LastHotID;
        private ulong KeyFocusID;
        internal ulong CurrentID;
        internal Vector2 Offset;
        internal bool NotInteractable = false;
        internal Stack<Rect> ClipStack = new();
        //Text
        internal TextEditState TextEditState = new();
        //TabBar
        internal Stack<TabBar> TabBarStack = new();
        internal TabBar CurrentTabBar;
        internal Dictionary<ulong , TabBar> TabBars = new();
        //IDS
        internal List<ulong> IDStack = new();
        //Rendering
        private Renderer Renderer;
        internal float CurrentZ = 0;

        public Context()
        {
            Renderer = new Renderer(this);
        }
        public void ResetFrame(float DT)
        {
            CurrentTick++;
            this.DT = DT;
            List<ulong> TabsToRemove = new();
            foreach(TabBar tab in TabBars.Values)
            {
                if(CurrentTick - tab.LastTick > 20)
                {
                    TabsToRemove.Add(tab.ID);
                }
            }
            foreach(ulong id in TabsToRemove)
            {
                TabBars.Remove(id);
            }
            CurrentZ = -0.1f;
            if (HotID == 0 && MouseDown(0))
            {
                NotInteractable = true;
            }
            if (NotInteractable && !MouseDown(0))
            {
                NotInteractable = false;
            }
            ClipStack.Clear();
            if (ActiveID != 0 && ActiveID != KeyFocusID || KeyDown((int)Keys.Escape))
            {
                KeyFocusID = 0;
            }
            if (HotID == 0 && MousePressed(0))
            {
                KeyFocusID = 0;
            }
            TabBarStack.Clear();
            Renderer.ResetForNewFrame();
            Renderer.PushClipRect(new(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y));
            Renderer.PushTextureID(font.Atlas.GetHandle());
            LastHotID = HotID;
            LastActiveID = ActiveID;
            HotID = 0;

        }

        // _   _ _   _ _    
        //| | | | |_(_) |___
        //| |_| |  _| | (_-<
        // \___/ \__|_|_/__/

        #region Utils
        public ulong GetHotID()
        {
            return HotID;
        }
        public ulong GetLastHotID()
        {
            return LastHotID;
        }
        public void SetHotID(ulong ID)
        {
            HotID = ID;
        }
        public ulong GetActiveID()
        {
            return ActiveID;
        }
        public ulong GetlastActiveID()
        {
            return LastActiveID;
        }
        public void SetActiveID(ulong ID)
        {
            ActiveID = ID;
        }
        public ulong GetKeyFocusID()
        {
            return KeyFocusID;
        }
        public void SetKeyFocusID(ulong ID)
        {
            KeyFocusID = ID;
        }
        

        public void OnResize(Vector2 ScreenSize)
        {
            this.ScreenSize = ScreenSize;
        }
        public void OnTextInput(char Char)
        {
            InputedChar = Char;
        }
        public Vector2i CalculateTextSize(string Text, float Size, Rect Clip = default)
        {
            return Renderer.CalcTextSize(font, Text, Size, Clip);
        }
        public Vector2i CalculateTextPos(string Text, float Size, Rect Clip = default)
        {
            return Renderer.CalcTextPos(font, Text, Size, Clip);
        }
        public Renderer GetRenderer()
        {
            return Renderer;
        }
        public Vector2 GetScreenSize()
        {
            return ScreenSize;
        }
        public (bool, bool) BaseWiget(Rect Rect, ulong ID)
        {
            if (IsHover(Rect))
            {
                if (GetHotID() == 0)
                {
                    SetHotID(ID);
                    if (MousePressed(0) && GetActiveID() == 0 && !NotInteractable)
                    {
                        SetActiveID(ID);
                    }
                }
            }
            return (ID == HotID, ActiveID == ID);
        }
        public bool IsHover(Rect Rect)
        {
            if (ClipStack.Count > 0)
            {
                Rect ClipRect = ClipStack.Peek();
                if (!MouseInRect(ClipRect))
                {
                    return false;
                }
            }
            return MouseInRect(Rect);
        }
        private bool MouseInRect(Rect Rect)
        {
            if (Rect.ContainsPoint(MousePosition))
            {
                return true;
            }
            return false;
        }
        public string GetLabel(string Str)
        {
            return Str.Split("##")[0];
        }
        public ulong Hash(string Str, ulong Seed = 0)
        {
            byte[] Data = Encoding.Unicode.GetBytes(Str);
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public ulong Hash(byte[] Data, ulong Seed = 0)
        {
            return xxHash64.ComputeHash(Data, Data.Length, Seed);
        }
        public ulong GetID(string Str)
        {
            ulong Seed = GetSeed();
            ulong ID = Hash(Str, Seed);
            CurrentID = ID;
            return ID;
        }
        public ulong GetID(int N)
        {
            ulong Seed = GetSeed();
            return Hash(BitConverter.GetBytes(N), Seed);
        }
        public ulong GetSeed()
        {
            if (IDStack.Count > 0)
            {
                return IDStack[^1];
            }
            return 0;
        }
        public void PushID(string Str)
        {
            ulong ID = GetID(Str);
            IDStack.Add(ID);
        }
        public void PushID(int N)
        {
            ulong ID = GetID(N);
            IDStack.Add(ID);
        }
        public void PushGeneratedID(ulong ID)
        {
            IDStack.Add(ID);
        }
        public void PopID()
        {
            IDStack.RemoveAt(IDStack.Count - 1);
        }
        public ulong GetCurrentStackID()
        {
            return IDStack[^1];
        }
        public ulong GetCurrentID()
        {
            return CurrentID;
        }
        public void PushClipRect(Rect Rect)
        {
            ClipStack.Push(Rect);
        }
        public void PopClipRect()
        {
            ClipStack.Pop();
        }
        public void PushZLevel()
        {
            CurrentZ += 0.1f;
        }
        public void PopZLevel()
        {
            CurrentZ -= 0.1f;
        }
        public void PushIntZLevel()
        {
            CurrentZ += 0.01f;
        }
        public void PopIntZLevel()
        {
            CurrentZ -= 0.01f;
        }
        #endregion
        // _                       _   
        //| |   __ _ _  _ ___ _  _| |_ 
        //| |__/ _` | || / _ \ || |  _|
        //|____\__,_|\_, \___/\_,_|\__|
        //           |__/              
        #region Layout
        internal StackType CurrentStackType;
        internal Rect CurrentGeneratedRect = default;
        internal Stack<Rect> AreaStack = new();
        internal Rect CurrentArea => AreaStack.Peek();
        internal Vector2i Cursor;
        internal Vector4i Padding = new(10);
        internal int CurrentRowHeight = 0;
        public void ResetState()
        {
            Cursor = Vector2i.Zero;
            CurrentRowHeight = 0;
        }
        public Rect GetNextLayout(Vector2i RequestedMinSize)
        {
            Vector2i RealSize = RequestedMinSize;
            if(CurrentGeneratedRect.Equals(default))
            {
                if (CurrentStackType == StackType.Vertical)
                {
                    NextRow(RealSize.Y);
                    NextCollumn(RealSize.X);

                }
                else if(CurrentStackType == StackType.Horizontal)
                {
                    Next(RealSize);
                }
            }
            Rect Copy = CurrentGeneratedRect;
            CurrentGeneratedRect = default;
            return Copy;
        }

        public void Next(Vector2 Size)
        {
            if (Size.X <= 1)
            {
                Size.X = CurrentArea.W * Size.X;
            }
            if (Size.Y<= 1)
            {
                Size.Y= CurrentArea.H * Size.Y;
            }
            if (Cursor.X + Size.X  <= CurrentArea.W)
            {
                Rect Fin = new(CurrentArea.X + Cursor.X + Padding.X, CurrentArea.Y + Cursor.Y + Padding.Y, (int)Size.X - Padding.X - Padding.Z, (int)Size.Y - Padding.Y- Padding.W);
                Cursor.X += (int)Size.X;
                CurrentRowHeight = Math.Max(CurrentRowHeight, (int)Size.Y + Padding.Y + Padding.W - Padding.Y - Padding.W);
                CurrentGeneratedRect =  Fin;
            }
            else
            {
                Cursor.Y += CurrentRowHeight;
                CurrentRowHeight = 0;
                Cursor.X = 0;
                Rect Fin = new(CurrentArea.X + Cursor.X + Padding.X, CurrentArea.Y + Cursor.Y + Padding.Y, (int)Size.X - Padding.X - Padding.Z, (int)Size.Y - Padding.Y - Padding.W);
                Cursor.X += (int)Size.X;
                CurrentRowHeight = Math.Max(CurrentRowHeight, (int)Size.Y + Padding.Y + Padding.W);
                CurrentGeneratedRect = Fin;
            }
        }
        public void NextRow(float RowHeight)
        {
            if (RowHeight <= 1)
            {
                RowHeight = CurrentArea.H * RowHeight;
            }
            Cursor.X = 0;
            Cursor.Y += CurrentRowHeight;
            CurrentRowHeight = (int)RowHeight;
        }
        public void NextCollumn(float ColumnWidth)
        {
            if (ColumnWidth <= 1)
            {
                ColumnWidth = CurrentArea.W * ColumnWidth;
            }
            if (Cursor.X + ColumnWidth <= CurrentArea.W)
            {
                Rect Fin = new(CurrentArea.X + Cursor.X + Padding.X, CurrentArea.Y + Cursor.Y + Padding.X, (int)ColumnWidth - Padding.X - Padding.Z, CurrentRowHeight - Padding.Y- Padding.W);
                Cursor.X += (int)ColumnWidth;
                CurrentGeneratedRect = Fin;

            }
        }
        public void SetStackType(StackType Type)
        {
            CurrentStackType = Type;
        }
        public void PushArea(Rect Area)
        {
            AreaStack.Push(Area);
            ResetState();
        }
        public void PopArea()
        {
            AreaStack.Pop();
        }
        public enum StackType
        {
            Vertical,
            Horizontal,
        }
        #endregion
        //__      ___    _          _      
        //\ \    / (_)__| |__ _ ___| |_ ___
        // \ \/\/ /| / _` / _` / -_)  _(_-<
        //  \_/\_/ |_\__,_\__, \___|\__/__/
        //                |___/            
        #region Widgets
        public bool Button(string Label)
        {
            //Preprocess
            ulong ID = GetID(Label);
            string RealLabel = GetLabel(Label);
            Vector2i TSize = CalculateTextSize(RealLabel, 20);
            Rect Rect = GetNextLayout(TSize + new Vector2i(10) + Padding.Xy + Padding.Zw);
            //Widget base
            (bool Hot, bool Active) = BaseWiget(Rect, ID);
            bool Pressed = false;
            if (!Hot && Active)
            {
                SetActiveID(0);
            }
            //Logic
            if (Active && MouseUp(0))
            {
                SetActiveID(0);
                Pressed = true;
            }
            //Rendering
            Renderer.AddRect(Rect, Hot ? 0x509940FFu : 0x357030FF);
            Renderer.AddText(new Vector2i(Rect.CenterX - TSize.X / 2, Rect.CenterY - (TSize.Y / 2)), RealLabel, 20, 0xFFFFFFFF, font);
            //Result
            return Pressed;
        }
        public bool ComboBox(string Label, ref int Selected, string[] Data)
        {

            ulong ID = GetID(Label);
            string RealLabel = GetLabel(Label);
            Vector2i LSize = CalculateTextSize(RealLabel, 20);
            string SelectedStr = Data[Selected];
            Vector2i TextSize = CalculateTextSize(SelectedStr, 20);
            Rect Rect = GetNextLayout(new Vector2i((LSize.X + 10) + 10 + TextSize.X + 10, TextSize.Y) + Padding.Xy + Padding.Zw);
            int BaseHeight = Rect.H;
            Rect LabelPart = new(Rect.X, Rect.Y, (int)LSize.X + 10, Rect.H);
            if (GetlastActiveID() == ID)
            {
                Rect.H += Data.Length * BaseHeight;
            }
            //Preprocess
            Rect ComboPart = new(LabelPart.XW, Rect.Y, Rect.W - (int)LSize.X - 10, Rect.H);
            //Widget base
            (bool Hot, bool Active) = BaseWiget(ComboPart, ID);
            if (Active && GetlastActiveID() == ID && !IsHover(new(ComboPart.X, ComboPart.Y + BaseHeight, ComboPart.W, ComboPart.H - BaseHeight)) && MousePressed(0))
            {
                if (GetHotID() != ID)
                {
                    SetActiveID(GetHotID());
                }
                else
                {
                    SetActiveID(0);
                }
            }
            int YPOS = ComboPart.Y + BaseHeight;
            if (Active)
            {
                PushIntZLevel();
            }
            Renderer.AddRect(ComboPart, 0x357030FF);
            Renderer.AddText(new(ComboPart.X + 10, ComboPart.Y + BaseHeight / 2 - TextSize.Y / 2), SelectedStr, 20, 0xFFFFFFFF, font);
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - LSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            //Current selected 
            if (Active)
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    Rect CurrRect = new(ComboPart.X, YPOS, ComboPart.W, BaseHeight);
                    bool Hover = IsHover(CurrRect);
                    if (Hover)
                    {
                        Renderer.AddRect(CurrRect, 0x509980FFu);
                        if (MousePressed(0))
                        {
                            SetActiveID(0);
                            Selected = i;
                        }
                    }
                    Renderer.AddText(new(ComboPart.X + 10, YPOS + BaseHeight / 2 - TextSize.Y / 2), Data[i], 20, 0xFFFFFFFF, font);
                    YPOS += BaseHeight;
                }
                PopIntZLevel();
            }
            return Active;
        }
        public void ProgressBar( string Label, float Percentage)
        {
            ulong ID = GetID(Label);
            string RealLabel = GetLabel(Label) + " : " + Percentage.ToString("00.00") + "%";
            Vector2i LSize = CalculateTextSize(RealLabel, 20);
            Rect Rect = GetNextLayout(new(1, LSize.Y + Padding.Y + Padding.W));
            Rect LabelPart = new(Rect.X, Rect.Y, (int)LSize.X + 10, Rect.H);
            Rect SliderPart = new(Rect.X + (int)LSize.X + 10, Rect.Y, Rect.W - (int)LSize.X - 10, Rect.H);
            int xpos = (int)((SliderPart.W - 6) * ((Percentage - 0) / (float)(100)));
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - LSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            Renderer.AddRect(SliderPart, 0x357030FF);
            Renderer.AddRect(new(SliderPart.X + 3, SliderPart.Y + 3, xpos, SliderPart.H - 6), 0x409030FF);
        }
        public bool SliderFloat( string Label, ref float Value, float Min, float Max, float Step = 1)
        {
            //Preprocess
            ulong ID = GetID(Label);
            string RealLabel = GetLabel(Label);
            Vector2i LSize = CalculateTextSize(RealLabel, 20);
            Rect Rect = GetNextLayout(new(1, LSize.Y + Padding.Y + Padding.W));
            Rect LabelPart = new(Rect.X, Rect.Y, (int)LSize.X + 10, Rect.H);
            Rect SliderPart = new(Rect.X + (int)LSize.X + 10, Rect.Y, Rect.W - (int)LSize.X - 10, Rect.H);
            //Widget base
            (bool Hot, bool Active) = BaseWiget(SliderPart, ID);
            if (!MouseDown(0) && Active)
            {
                SetActiveID(0);
            }
            //Logic
            if (Active)
            {
                float RelativeMousePos = Math.Max(0, Math.Min(MousePosition.X - SliderPart.X, SliderPart.W)) / (float)SliderPart.W;
                Value = Min + RelativeMousePos * (Max - Min);
                Value = (int)Math.Round(Value / Step) * Step;
                Value = Math.Max(Min, Math.Min(Value, Max));
            }
            int xpos = (int)((SliderPart.W - 6) * ((Value - Min) / (Max - Min)));
            //Rendering
            //Label
            Vector2i TSize = CalculateTextSize(Value.ToString(), 20);
            //
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - LSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            Renderer.AddRect(SliderPart, Hot ? 0x509940FFu : 0x357030FF);
            Renderer.AddRect(new(SliderPart.X + 3, SliderPart.Y + 3, xpos, SliderPart.H - 6), 0x409030FF);
            Renderer.AddText(new(SliderPart.CenterX - TSize.X / 2, SliderPart.CenterY - TSize.Y / 2), Value.ToString(), 20, 0xFFFFFFFF, font);
            return Active;
        }
        public bool SliderInt( string Label, ref int Value, int Min, int Max, int Step = 1)
        {
            //Preprocess
            ulong ID = GetID(Label);
            string RealLabel = GetLabel(Label);
            Vector2i LSize = CalculateTextSize(RealLabel, 20);
            Rect Rect = GetNextLayout(new(1, LSize.Y + Padding.Y + Padding.W));
            Rect LabelPart = new(Rect.X, Rect.Y, (int)LSize.X + 10, Rect.H);
            Rect SliderPart = new(Rect.X + (int)LSize.X + 10, Rect.Y, Rect.W - (int)LSize.X - 10, Rect.H);
            //Widget base
            (bool Hot, bool Active) = BaseWiget(SliderPart, ID);
            if (!MouseDown(0) && Active)
            {
                SetActiveID(0);
            }
            //Logic
            if (Active)
            {
                float RelativeMousePos = Math.Max(0, Math.Min(MousePosition.X - SliderPart.X, SliderPart.W)) / (float)SliderPart.W;
                Value = (int)(Min + RelativeMousePos * (Max - Min));
                Value = (int)(Math.Round(Value / (float)Step) * Step);
                Value = Math.Max(Min, Math.Min(Value, Max));
            }
            int xpos = (int)((SliderPart.W - 6) * ((Value - Min) / (float)(Max - Min)));
            //Rendering
            //Label
            Vector2i TSize = CalculateTextSize(Value.ToString(), 20);
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - LSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            Renderer.AddRect(SliderPart, Hot ? 0x509940FFu : 0x357030FF);
            Renderer.AddRect(new(SliderPart.X + 3, SliderPart.Y + 3, xpos, SliderPart.H - 6), 0x409030FF);
            Renderer.AddText(new(SliderPart.CenterX - TSize.X / 2, SliderPart.CenterY - TSize.Y / 2), Value.ToString(), 20, 0xFFFFFFFF, font);
            return Active;
        }
        public bool TextEdit( string Label, ref string Text, int MaxSize, TextEditFlag Flags = TextEditFlag.None, bool Clip = true)
        {
            //Preprocess
            string RealLabel = GetLabel(Label);
            ulong ID = GetID(Label);
            //Text
            Vector2i TSize = CalculateTextSize(RealLabel, 20);
            Vector2i TextSize = CalculateTextSize(Text, 20);
            Rect Rect = GetNextLayout(new(1, TextSize.Y + 10 + Padding.Y + Padding.W));
            Rect LabelPart = new(Rect.X, Rect.Y, (int)TSize.X + 10, Rect.H);
            Rect TextEditPart = new(Rect.X + (int)TSize.X + 10, Rect.Y, Rect.W - (int)TSize.X - 10, Rect.H);
            Vector2i TextPos = new(TextEditPart.X + 10, LabelPart.CenterY - TextSize.Y / 2);
            //Widgetbase
            (bool Hot, bool Active) = BaseWiget(TextEditPart, ID);
            if (Active && GetlastActiveID() != ID)
            {
                SetKeyFocusID(ID);
            }
            if (Active && (!Hot || MouseUp(0)))
            {
                SetActiveID(0);
            }
            bool KeyFocused = ID == GetKeyFocusID();
            bool Result = false;
            TextEditState State = new();
            if (KeyFocused)
            {
                if (TextEditState.ID != ID)
                {
                    TextEditState = new() { ID = ID };
                }
                State = TextEditState;
                Result = State.Update(this, ref Text, MaxSize, TextEditPart, TextPos, Flags);
            }
            //Rendering
            Renderer.AddRect(TextEditPart, KeyFocused ? 0x807233FF : Hot ? 0x509940FF : 0x357030FFu);
            //Label
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - TSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            int TextHeight = (int)(TextSize.Y > 0 ? TextSize.Y : TSize.Y);
            //Cursor
            if (KeyFocused && State.hasSelection)
            {
                Renderer.AddText(TextPos, Text, 20, 0xFFFFFFFF, font, Clip ? TextEditPart : default, [new Vector3i(State.SelectionZone.X, State.SelectionZone.Y, -16776961)]);
            }
            else
            {
                Renderer.AddText(TextPos, Text, 20, 0xFFFFFFFF, font, Clip ? TextEditPart : default);
            }
            if (KeyFocused && State.RenderCursor)
            {
                float Scale = font.Height / 20;
                Vector2i CursorPos = CalculateTextPos(Text.Substring(0, State.CursorPos), 20);
                if (TextEditPart.X + 10 + CursorPos.X < TextEditPart.XW)
                {
                    Renderer.AddRect(new Rect(TextEditPart.X + 10 + CursorPos.X, LabelPart.CenterY - (int)TextHeight / 2 + (int)CursorPos.Y, Math.Max((int)(1 * Scale), 1), (int)TSize.Y), 0xFFFFFFFF);
                }
            }
            return Result;
        }
        public bool IntEdit( string Label, ref int Value, bool Clip = true)
        {
            string Text = Value.ToString();
            //Preprocess
            string RealLabel = GetLabel(Label);
            ulong ID = GetID(Label);
            //Text
            Vector2i TSize = CalculateTextSize(RealLabel, 20);
            Vector2i TextSize = CalculateTextSize(Text, 20);
            Rect Rect = GetNextLayout(new(1, TSize.Y + 10 + Padding.Y + Padding.W));
            //
            Rect LabelPart = new(Rect.X, Rect.Y, (int)TSize.X + 10, Rect.H);
            Rect TextEditPart = new(Rect.X + (int)TSize.X + 10, Rect.Y, Rect.W - (int)TSize.X - 10, Rect.H);
            //
            Vector2i TextPos = new(TextEditPart.X + 10, LabelPart.CenterY - TextSize.Y / 2);
            //Widgetbase
            (bool Hot, bool Active) = BaseWiget(TextEditPart, ID);
            if (Active && GetlastActiveID() != ID)
            {
                SetKeyFocusID(ID);
            }
            if (Active && (!Hot || MouseUp(0)))
            {
                SetActiveID(0);
            }
            bool KeyFocused = ID == GetKeyFocusID();
            bool Result = false;
            TextEditState State = new();
            if (KeyFocused)
            {
                if (TextEditState.ID != ID)
                {
                    TextEditState = new() { ID = ID };
                }
                State = TextEditState;
                Result = State.Update(this, ref Text, 10000, TextEditPart, TextPos, TextEditFlag.NumberOnly | TextEditFlag.EnterExecute);
            }
            //Rendering
            Renderer.AddRect(TextEditPart, KeyFocused ? 0x807233FF : Hot ? 0x509940FF : 0x357030FFu);
            //Label
            Renderer.AddText(new(LabelPart.X, LabelPart.CenterY - TSize.Y / 2), RealLabel, 20, 0xFFFFFFFF, font);
            int TextHeight = (int)(TextSize.Y > 0 ? TextSize.Y : TSize.Y);
            //Cursor
            if (KeyFocused && State.hasSelection)
            {
                Renderer.AddText(TextPos, Text, 20, 0xFFFFFFFF, font, Clip ? TextEditPart : default, [new Vector3i(State.SelectionZone.X, State.SelectionZone.Y, -16776961)]);
            }
            else
            {
                Renderer.AddText(TextPos, Text, 20, 0xFFFFFFFF, font, Clip ? TextEditPart : default);            
            }
            if (KeyFocused && State.RenderCursor)
            {
                float Scale = font.Height / 20;
                Vector2i CursorPos = CalculateTextPos(Text.Substring(0, State.CursorPos), 20);
                if (TextEditPart.X + 10 + CursorPos.X < TextEditPart.XW)
                {
                    Renderer.AddRect(new Rect(TextEditPart.X + 10 + CursorPos.X, LabelPart.CenterY - (int)TextHeight / 2 + (int)CursorPos.Y, Math.Max((int)(1 * Scale), 1), (int)TSize.Y), 0xFFFFFFFF);
                }
            }
            if (KeyDown((int)Keys.Enter) && int.TryParse(Text, out var NewVal))
            {
                Value = NewVal;
            }
            return Result;
        }
        public void Panel(Rect PanelRect, uint Color)
        {
            Renderer.AddRect(PanelRect, Color);
        }
        public void BeginScrollPanel(string Label, ref int ScrollOffset, int Size, uint Color = 0x00000000,Vector2i PanelSize = default)
        {
            Rect PanelRect = GetNextLayout(PanelSize != default ? PanelSize : new Vector2i(1));
            const int ScrollHandleSize = 14;
            int SliderWidth = Size > PanelRect.H ? 20 : 0;
            ulong ID = GetID(Label);
            PushGeneratedID(ID);
            Rect SliderRect = new(PanelRect.XW - SliderWidth, PanelRect.Y, SliderWidth, PanelRect.H);
            int ScrollerSize = Math.Min((int)((float)PanelRect.H / Size * PanelRect.H), PanelRect.H);
            int Ypos = (int)((float)ScrollOffset / Size * PanelRect.H);
            Rect ScrollHandle = new(SliderRect.X + (SliderWidth - ScrollHandleSize) / 2, SliderRect.Y + Ypos, ScrollHandleSize, ScrollerSize);
            (bool Hot, bool Active) = BaseWiget(SliderRect, ID);
            if (Active && GetlastActiveID() != ID)
            {
                Offset = new(ScrollOffset, MousePosition.Y);
            }
            //Logic
            if (IsHover(PanelRect))
            {
                ScrollOffset -= ScrollDelta.Y * 20;
            }
            if (Active)
            {
                float Delta = MousePosition.Y - Offset.Y;
                float maxOffset = Size - PanelRect.H;
                ScrollOffset = (int)Math.Clamp(Offset.X + Delta * (maxOffset / (PanelRect.H - ScrollHandle.H)), 0.0f, maxOffset);
            }
            ScrollOffset = Math.Min(Size - PanelRect.H, Math.Max(ScrollOffset, 0));
            if (!MouseDown(0) && Active)
            {
                Offset = Vector2.Zero;
                SetActiveID(0);
            }
            PushClipRect(PanelRect);
            PushArea(new(PanelRect.X, PanelRect.Y - ScrollOffset, PanelRect.W - SliderWidth, PanelRect.H));
            Renderer.PushClipRect(new(PanelRect.X, PanelRect.Y, PanelRect.W, PanelRect.H));
            Panel(PanelRect, Color);
            Renderer.AddRect(SliderRect, 0x996040FF);
            Renderer.AddRect(ScrollHandle, 0x405930FF);
        }
        public void EndScrollPanel()
        {
            Renderer.PopClipRect();
            PopArea();
            PopClipRect();
            PopID();
        }
        public void BeginTabBar(string Label, Vector2i Size = default)
        {
            Rect Area = GetNextLayout(Size != default ? Size : new Vector2i(1));

            ulong ID = GetID(Label);
            PushGeneratedID(ID);
            if(!TabBars.ContainsKey(ID))
            {
                TabBars.Add(ID, new() { ID = ID, });
            }
            var Cur = TabBars[ID];
            Cur.LastTick = CurrentTick;
            TabBarStack.Push(Cur);
            CurrentTabBar = Cur;
            Rect TabPart = new(Area.X,Area.Y,Area.W,30);
            Rect ContentPart = new(Area.X, Area.Y +30, Area.W, Area.H-30);
            Panel(Area, 0x252525FF);
            PushClipRect(Area);
            Renderer.PushClipRect(Area);
            Renderer.AddRect(ContentPart, 0x353535FF);
            int CurrentX = 5;
            if(Cur.Empty && Cur.TabItems.Count >0)
            {
                Cur.SelectedTabItem = Cur.TabItems[0].ID;
                Cur.Empty = false;
            }
            bool FindNeeded =Cur.Empty ? true : false;
            foreach(TabItem tabItem in Cur.TabItems)
            {
                Vector2i TSize = CalculateTextSize( tabItem.Text,20);
                Rect SingleTabPart = new(TabPart.X + CurrentX, TabPart.Y+5, TSize.X + 10, 25);
                ( bool TabHot, bool TabActive) = BaseWiget(SingleTabPart, tabItem.ID);
                Renderer.AddRect(SingleTabPart, TabHot ?  0x555555FF : 0x454545FFu);
                Renderer.AddText(new(SingleTabPart.CenterX - TSize.X / 2, SingleTabPart.CenterY - TSize.Y / 2), tabItem.Text, 20, 0xFFFFFFFF);
                if(tabItem.ID == Cur.SelectedTabItem)
                {
                    FindNeeded = true;
                }
                if(TabHot  && TabActive)
                {
                    Cur.SelectedTabItem = tabItem.ID;
                    FindNeeded = true;
                }
                if (MouseUp(0) && TabActive)
                {
                    SetActiveID(0);
                }
                
                CurrentX += SingleTabPart.W + 5;
            }
            if(!FindNeeded )
            {
                Cur.SelectedTabItem = Cur.TabItems[0].ID;
            }
            Cur.TabItems.Clear();
            PushArea(ContentPart);
        }
        public void EndTabBar()
        {
            PopClipRect();
            Renderer.PopClipRect();
            PopArea();
            PopID();
            TabBarStack.Pop();
            if(TabBarStack.Count >0)
            {
                CurrentTabBar = TabBarStack.Peek();
            }else
            {
                CurrentTabBar = null;
            }
        }
        public bool BeginTabItem(string Label)
        {
            ulong ID = GetID(Label);

            CurrentTabBar.TabItems.Add(new() { ID = ID, Text = Label });
            if(CurrentTabBar.SelectedTabItem == ID)
            {
                PushGeneratedID(ID);

                return true;
            }
            return false;
        }
        public void EndTabItem()
        {
            PopID();
        }
        #endregion
    }
    // ___ _        _     
    /// __| |_ _  _| |___ 
    //\__ \  _| || | / -_)
    //|___/\__|\_, |_\___|
    //         |__/       

    // ___             _                 
    //| _ \___ _ _  __| |___ _ _ ___ _ _ 
    //|   / -_) ' \/ _` / -_) '_/ -_) '_|
    //|_|_\___|_||_\__,_\___|_| \___|_|
    public struct Command
    {
        internal int TextureID;
        internal int Offset;
        internal int Count;
        internal Rect ClipRect;
        public bool Equals(Command other)
        {
            if (!ClipRect.Equals(other.ClipRect)) return false;
            if (TextureID != other.TextureID) return false;
            return true;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 UV;
        public uint Color;

        public Vertex(Vector3 position, Vector2 uV, uint color)
        {
            Position = position;
            UV = uV;
            Color = color;
        }
    }
    public class Renderer
    {
        internal const uint AlphaMask = 0x000000FF;
        public List<Command> Commands = new();
        public Vector<Vertex> VertexBuffer = new();
        public Vector<uint> IndexBuffer = new();
        internal uint VtxCurrentIdx;
        public Stack<Rect> ClipStack = new();
        public Stack<int> TextureStack = new();
        public Context Ctx;
        //
        int CurrentTextureID = 0;
        Rect CurrentClipRect;

        public Renderer(Context ctx)
        {
            Ctx = ctx;
        }

        public void AddRect(Vector2 Min, Vector2 Max, uint Color)
        {
            if ((Color & AlphaMask) == 0) return;
            PrimRect(Min, Max, Color);
        }
        public void AddRect(Rect Rect, uint Color)
        {
            if ((Color & AlphaMask) == 0) return;
            PrimRect(new(Rect.X, Rect.Y), new(Rect.XW, Rect.YH), Color);
        }
        public void AddTexturedRect(int Texture, Vector2 Min, Vector2 Max, Vector2 UV0 = default, Vector2 UV1 = default, uint Color = 0xFFFFFFFF)
        {
            if (UV1 == default)
            {
                UV1 = Vector2.One;
            }
            if ((Color & AlphaMask) == 0)
            {
                return;
            }
            bool push_texture_id = Texture != CurrentTextureID;
            if (push_texture_id)
            {
                PushTextureID(Texture);
            }
            PrimRect(Min, Max, Color, UV0, UV1);
            if (push_texture_id)
            {
                PopTextureID();
            }
        }
        public void AddText(Vector2i Position, string Text, float Scale, uint Color, Font Font = null, Rect Clip = default, Vector3i[] Background = null, Vector2i[] UnderLine = null)
        {
            if ((Color & AlphaMask) == 0)
            {
                return;
            }
            if (Font == null)
            {
                Font = Ctx.font;// Font =Immui.GetContext().font;
            }
            if (Scale == 0.0f)
            {
                Scale = Font.Height;
            }
            bool DifText = CurrentTextureID != Font.Atlas.GetHandle();

            if (DifText)
            {
                PushTextureID(Font.Atlas.GetHandle());
            }
            RenderText(Font, Scale, Position, Color, Text, Clip, Background, UnderLine);
            if (DifText)
            {
                PopTextureID();
            }
        }
        public Vector2i CalcTextPos(Font font, string Text, float Size, Rect Clip = default)
        {
            float line_height = 0;
            float scale = Size / font.Height;
            bool UseClip = !Clip.Equals(default);
            int i = 0;
            int X = 0, Y = 0;
            while (i < Text.Length)
            {
                char c = Text[i];
                i++;
                if (c == '\n')
                {
                    X = 0;
                    if (UseClip && Y + line_height > Clip.YH)
                    {
                        break;
                    }
                    Y += (int)((font.Ascent - font.Descent + font.LineGap) * scale);
                    continue;
                }
                if (c == '\r')
                {
                    continue;
                }
                //TODO : Make a better and more secure way to get character
                Character glyph = font.Characters[c];
                if (glyph == null)
                {
                    continue;
                }
                int charWidth = (int)MathF.Ceiling((glyph.Advance) * scale);
                if (UseClip && X + charWidth > Clip.XW)
                {
                    break;
                }
                X += charWidth;
            }
            return new(X, Y);
        }
        public Vector2i CalcTextSize(Font font, string Text, float Size, Rect Clip = default)
        {
            int line_height = 0;
            float scale = Size / font.Height;
            bool UseClip = !Clip.Equals(default);
            int i = 0;
            int X = 0, Y = 0;
            int Width = 0;
            while (i < Text.Length)
            {
                char c = Text[i];
                i++;
                if (c == '\n')
                {

                    X = 0;
                    if (UseClip && Y + line_height > Clip.YH)
                    {
                        break;
                    }
                    Y += line_height + font.LineGap;
                    continue;
                }
                if (c == '\r')
                {
                    continue;
                }

                //TODO : Make a better and more secure way to get character
                Character glyph = font.Characters[c];
                if (glyph == null)
                {
                    continue;
                }
                int charWidth = (int)((glyph.Advance) * scale);
                if (UseClip && X + charWidth > Clip.XW)
                {
                    break;
                }
                X += charWidth;
                line_height = (int)((font.Ascent - font.Descent) * scale); //= Math.Max(line_height, glyph.Size.Y *scale);
                Width = Math.Max(Width, X);
            }


            return new(Width, Y + line_height);
        }
        public void AddUnderline(Font font, Vector2i[] Underline, uint Color, float scale, string Text, Vector2i Position)
        {
            int LineHeight = (int)(font.LineHeight * scale);
            for (int j = 0; j < Underline.Length; j++)
            {
                Vector2i Current = Underline[j];
                Vector2i StartPos = CalcTextPos(font, Text.Substring(0, Current.X), scale * font.Height);
                int W = 0;
                for (int Index = Current.X; Index < Current.Y; Index++)
                {
                    if (Text[Index] != '\n')
                    {
                        W += (int)(font.Characters[Text[Index]].Advance * scale);
                    }
                    if (Index == Current.Y - 1 || Text[Index] == '\n')
                    {
                        AddRect(new Rect(Position.X + StartPos.X, Position.Y + StartPos.Y + (int)(font.Ascent + 1 * scale), W, (int)(2 * scale)), Color);
                        StartPos = new(0, StartPos.Y + LineHeight);
                        W = 0;
                    }
                }
            }
        }
        public void AddBackGround(Font font, Vector3i[] Background, float scale, string Text, Vector2i Position)
        {
            int LineHeight = (int)(font.LineHeight * scale);
            for (int j = 0; j < Background.Length; j++)
            {
                Vector3i Current = Background[j];
                Vector2i StartPos = CalcTextPos(font, Text.Substring(0, Current.X), scale * font.Height);
                int W = 0;
                for (int Index = Current.X; Index < Current.Y; Index++)
                {
                    if (Text[Index] != '\n')
                    {
                        W += (int)(font.Characters[Text[Index]].Advance * scale);
                    }
                    if (Index == Current.Y - 1 || Text[Index] == '\n')
                    {
                        AddRect(new Rect(Position.X + StartPos.X, Position.Y + StartPos.Y, W, LineHeight), (uint)Current.Z);
                        StartPos = new(0, StartPos.Y + LineHeight);
                        W = 0;
                    }
                }
            }
        }
        public void RenderText(Font font, float Size, Vector2i Position, uint Color, string Text, Rect Clip = default, Vector3i[]? Background =null, Vector2i[]? UnderLine = null)
        {
            float CurrentZ = Ctx.CurrentZ;
            bool UseClip = !Clip.Equals(default);
            int X = (int)MathF.Truncate(Position.X);
            int Y = (int)MathF.Truncate(Position.Y);

            int start_x = X;
            float scale = Size / font.Height;
            float line_height = font.Height * scale;
            if (Text.Length == 0)
            {
                return;
            }
            int vtxCountMax = Text.Length * 4;
            int idxCountMax = Text.Length * 6;
            int idxExpectedSize = IndexBuffer.Size + idxCountMax;
            var Command = Commands[Commands.Count - 1];
            Command.Count += idxCountMax;
            Commands[Commands.Count - 1] = Command;
            uint colUntinted = Color | ~AlphaMask;
            if (Background != null)
            {
                AddBackGround(font, Background, scale, Text, Position);
            }
            if (UnderLine != null)
            {
                AddUnderline(font, UnderLine, Color, scale, Text, Position);
            }
            int i = 0;
            while (i < Text.Length)
            {
                char c = Text[i];
                i++;
                if (c == '\n')
                {
                    X = start_x;
                    if (UseClip && Y + (font.LineGap + font.Ascent - font.Descent) * scale > Clip.YH)
                    {
                        break;
                    }
                    Y += (int)((font.LineGap + font.Ascent - font.Descent) * scale); // line_height;
                    continue;
                }
                if (c == '\r')
                {
                    continue;
                }
                //TODO : Make a better and more secure way to get character
                Character glyph = font.Characters[c];
                if (glyph == null)
                {
                    continue;
                }
                int charWidth = (int)((glyph.Advance) * scale);
                float BaseLine = Y + font.Ascent * scale;
                if (UseClip && X + charWidth > Clip.XW)
                {
                    break;
                }
                if (glyph.Rendarable)
                {
                    float x1 = X + glyph.Bearing.X * scale;
                    float x2 = x1 + glyph.Size.X * scale;
                    float y1 = BaseLine + scale * glyph.Bearing.Y /* Y -( glyph.Size.Y - glyph.Bearing.Y) *scale */ ;
                    float y2 = y1 - glyph.Size.Y * scale;
                    float u1 = glyph.UV0.X;
                    float v1 = glyph.UV0.Y;
                    float u2 = glyph.UV1.X;
                    float v2 = glyph.UV1.Y;
                    VertexBuffer.PushBack(new Vertex(new Vector3(x1, y1, CurrentZ), new Vector2(u1, v2), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector3(x2, y1, CurrentZ), new Vector2(u2, v2), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector3(x2, y2, CurrentZ), new Vector2(u2, v1), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector3(x1, y2, CurrentZ), new Vector2(u1, v1), Color));
                    IndexBuffer.PushBack(VtxCurrentIdx);
                    IndexBuffer.PushBack(VtxCurrentIdx + 1);
                    IndexBuffer.PushBack(VtxCurrentIdx + 2);
                    IndexBuffer.PushBack(VtxCurrentIdx);
                    IndexBuffer.PushBack(VtxCurrentIdx + 2);
                    IndexBuffer.PushBack(VtxCurrentIdx + 3);
                    VtxCurrentIdx += 4;
                }
                X += charWidth;
            }
            Command DCmd = Commands[^1];
            DCmd.Count -= idxExpectedSize - IndexBuffer.Size;
            Commands[^1] = DCmd;
        }
        private void PrimRect(Vector2 A, Vector2 C, uint Color = 0xFFFFFFFF, Vector2 UVA = default, Vector2 UVC = default)
        {
            float CurrentZ = Ctx.CurrentZ;
            var Command = Commands[Commands.Count - 1];
            Command.Count += 6;
            Commands[Commands.Count - 1] = Command;
            uint Index = VtxCurrentIdx;
            //Index
            IndexBuffer.PushBack(Index);
            IndexBuffer.PushBack(Index + 1);
            IndexBuffer.PushBack(Index + 2);
            IndexBuffer.PushBack(Index);
            IndexBuffer.PushBack(Index + 2);
            IndexBuffer.PushBack(Index + 3);
            //Vertex
            VertexBuffer.PushBack(new(new(A.X, A.Y, CurrentZ), UVA, Color));
            VertexBuffer.PushBack(new(new(C.X, A.Y, CurrentZ), new(UVC.X, UVA.Y), Color));
            VertexBuffer.PushBack(new(new(C.X, C.Y, CurrentZ), UVC, Color));
            VertexBuffer.PushBack(new(new(A.X, C.Y, CurrentZ), new(UVA.X, UVC.Y), Color));
            VtxCurrentIdx += 4;
        }
        //
        //Internals
        //
        public void PushClipRect(Rect ClipRegion)
        {
            ClipStack.Push(ClipRegion);
            CurrentClipRect = ClipRegion;
            OnChangedClipRect();
        }
        public void PopClipRect()
        {
            ClipStack.Pop();
            CurrentClipRect = ClipStack.Count == 0 ? throw new Exception("Underflow :  you can't pop an empty stack") : ClipStack.Peek();
            OnChangedClipRect();
        }
        public void PushTextureID(int TextureID)
        {
            TextureStack.Push(TextureID);
            CurrentTextureID = TextureID;
            OnChangedTextureID();
        }
        public void PopTextureID()
        {
            TextureStack.Pop();
            CurrentTextureID = TextureStack.Count == 0 ? 0 : TextureStack.Peek();
            OnChangedTextureID();
        }
        //
        //Helpers
        //
        internal void ResetForNewFrame()
        {
            Commands.Clear();
            ClipStack.Clear();
            TextureStack.Clear();
            Commands.Add(new Command());
            IndexBuffer.Size = 0;
            VertexBuffer.Size = 0;
            VtxCurrentIdx = 0;
            CurrentTextureID = 0;
            CurrentClipRect = new();
        }
        internal void OnChangedClipRect()
        {
            Command curr_cmd = Commands[^1];
            if (curr_cmd.Count != 0 && !curr_cmd.ClipRect.Equals(CurrentClipRect))
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 1)
            {
                Command prev_cmd = Commands[^2];
                if (curr_cmd.Count == 0 && CurrentCommandTest(prev_cmd) && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }
            }
            curr_cmd.ClipRect = CurrentClipRect;
            Commands[^1] = curr_cmd;
        }
        internal void OnChangedTextureID()
        {
            Command curr_cmd = Commands[^1];
            if (curr_cmd.Count != 0 && curr_cmd.TextureID != CurrentTextureID)
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 1)
            {

                Command prev_cmd = Commands[^2];
                if (curr_cmd.Count == 0 && CurrentCommandTest(prev_cmd) && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }
            }
            curr_cmd.TextureID = CurrentTextureID;
            Commands[^1] = curr_cmd;
        }
        internal void AddDrawCmd()
        {
            Command draw_cmd = new();
            draw_cmd.ClipRect = CurrentClipRect;
            draw_cmd.TextureID = CurrentTextureID;
            draw_cmd.Offset = IndexBuffer.Size;
            Commands.Add(draw_cmd);
        }
        internal bool CurrentCommandTest(Command Cmd)
        {
            if (!Cmd.ClipRect.Equals(CurrentClipRect) || Cmd.TextureID != CurrentTextureID) return false;
            return true;
        }
        private bool DrawCmdAreSequentialIdxOffset(Command L, Command R)
        {
            return L.Offset + L.Count == R.Offset;
        }
    }
    // _   _ _   _ _    
    //| | | | |_(_) |___
    //| |_| |  _| | (_-<
    // \___/ \__|_|_/__/
    [Flags]
    public enum TextEditFlag
    {
        None = 0,
        Multiline = 1,
        EnterExecute = 2,
        NumberOnly = 4
    }
    internal class TextEditState
    {
        internal ulong ID;

        internal float Delay;

        internal float AccumulatedDT;
        internal bool RenderCursor;

        internal bool StartZoneSelection = false;
        internal int CursorPos = -1;
        internal int StartPos = 0;
        internal Vector2i SelectionZone = new();
        internal bool hasSelection => SelectionZone != default;
        internal int MousePositionToIndex(string Text, Vector2i Position, Context Ctx, Font font, float Scale, Vector2i MousePosition)
        {
            Scale = font.Height / Scale;
            int lineHeight = (int)(font.LineHeight * Scale);
            Vector2 Pos = Position;
            if (MousePosition.X < Position.X || MousePosition.Y < Position.Y)
            {
                return 0;
            }
            for (int charIndex = 0; charIndex < Text.Length; charIndex++)
            {
                char CurChar = Text[charIndex];
                if (CurChar == '\n')
                {
                    if (MousePosition.Y >= Pos.Y && MousePosition.Y < Pos.Y + lineHeight && MousePosition.X >= Pos.X)
                    {
                        return charIndex;
                    }
                    Pos.Y += lineHeight;
                    Pos.X = Position.X;
                    continue;
                }
                int charWidth = (int)(font.Characters[CurChar].Advance * Scale);
                if (MousePosition.X >= Pos.X && MousePosition.X < Pos.X + charWidth &&
                    MousePosition.Y >= Pos.Y && MousePosition.Y < Pos.Y + lineHeight)
                {
                    return charIndex;
                }
                Pos.X += charWidth;
            }
            return Text.Length;
        }
        internal void ClearSelection()
        {
            SelectionZone = default;
        }
        internal void BackAndDel(Context Ctx, ref string Text, ref bool Result)
        {
            if (Ctx.KeyDown((int)Keys.Backspace) && Text.Length > 0)
            {
                if (Ctx.KeyDown((int)Keys.LeftControl) && !hasSelection)
                {
                    int CurrentIndex = CursorPos;
                    bool FindNoSpace = false;
                    while (CurrentIndex > 0)
                    {
                        if (FindNoSpace && !char.IsLetterOrDigit(Text[CurrentIndex - 1]))
                        {
                            break;
                        }
                        if (char.IsLetterOrDigit(Text[CurrentIndex - 1]))
                        {
                            FindNoSpace = true;
                        }
                        CurrentIndex--;
                    }
                    Text = Text.Remove(CurrentIndex, CursorPos - CurrentIndex);
                    CursorPos = CurrentIndex;
                }
                else
                {
                    if (hasSelection)
                    {
                        Text = Text.Remove(SelectionZone.X, SelectionZone.Y - SelectionZone.X);
                        CursorPos = SelectionZone.X;
                    }
                    else if (CursorPos > 0)
                    {
                        Text = Text.Remove(CursorPos - 1, 1);
                        CursorPos -= 1;
                    }
                }
                Result = true;
                Delay = 0.2f;
                RenderCursor = true;
                ClearSelection();
            }
            if (Ctx.KeyDown((int)Keys.Delete) && Text.Length > 0)
            {
                if (Ctx.KeyDown((int)Keys.LeftControl) && !hasSelection)
                {
                    int CurrentIndex = CursorPos;
                    bool FindSpace = false;
                    while (CurrentIndex < Text.Length)
                    {
                        if (FindSpace && char.IsLetterOrDigit(Text[CurrentIndex]))
                        {
                            break;
                        }
                        if (!char.IsLetterOrDigit(Text[CurrentIndex]))
                        {
                            FindSpace = true;
                        }
                        CurrentIndex++;
                    }
                    Text = Text.Remove(CursorPos, CurrentIndex - CursorPos);
                }
                else
                {
                    if (hasSelection)
                    {
                        Text = Text.Remove(SelectionZone.X, SelectionZone.Y - SelectionZone.X);
                        CursorPos = SelectionZone.X;

                    }
                    else if (CursorPos < Text.Length)
                    {
                        Text = Text.Remove(CursorPos, 1);

                    }
                }
                Result = true;
                Delay = 0.2f;
                RenderCursor = true;
                ClearSelection();

            }
        }
        internal void Arrow(Context Ctx, ref string Text)
        {

            if (Ctx.KeyDown((int)Keys.Left))
            {
                if (CursorPos > 0)
                {

                    if (Ctx.KeyDown((int)Keys.LeftShift))
                    {
                        if (!hasSelection)
                        {
                            SelectionZone.Y = CursorPos;
                            SelectionZone.X = CursorPos - 1;
                        }
                        else
                        {

                            if (CursorPos >= SelectionZone.Y)
                            {
                                SelectionZone.Y--;
                            }
                            else
                            {

                                SelectionZone.X--;

                            }
                        }
                        CursorPos--;
                    }
                    else if (Ctx.KeyDown((int)Keys.LeftControl))
                    {
                        int CurrentIndex = CursorPos;
                        bool FindNoSpace = false;
                        while (CurrentIndex > 0)
                        {
                            if (FindNoSpace && !char.IsLetterOrDigit(Text[CurrentIndex - 1]))
                            {
                                break;
                            }
                            if (char.IsLetterOrDigit(Text[CurrentIndex - 1]))
                            {
                                FindNoSpace = true;
                            }
                            CurrentIndex--;
                        }
                        CursorPos = CurrentIndex;
                        ClearSelection();

                    }
                    else
                    {
                        CursorPos -= 1;
                        ClearSelection();

                    }
                    CursorPos = Math.Max(CursorPos, 0);

                    RenderCursor = true;
                    Delay = 0.05f;

                }
                else if (!Ctx.KeyDown((int)Keys.LeftShift))
                {
                    ClearSelection();

                }

            }
            if (Ctx.KeyDown((int)Keys.Right))
            {
                if (CursorPos < Text.Length)
                {
                    if (Ctx.KeyDown((int)Keys.LeftShift))
                    {
                        if (!hasSelection)
                        {
                            SelectionZone.X = CursorPos;
                            SelectionZone.Y = CursorPos + 1;
                        }
                        else
                        {


                            if (CursorPos <= SelectionZone.X)
                            {
                                SelectionZone.X++;
                            }
                            else
                            {

                                SelectionZone.Y++;

                            }
                        }
                        CursorPos++;

                    }
                    else if (Ctx.KeyDown((int)Keys.LeftControl))
                    {
                        int CurrentIndex = CursorPos;
                        bool FindSpace = false;
                        while (CurrentIndex < Text.Length)
                        {
                            if (FindSpace && char.IsLetterOrDigit(Text[CurrentIndex]))
                            {
                                break;
                            }
                            if (!char.IsLetterOrDigit(Text[CurrentIndex]))
                            {
                                FindSpace = true;
                            }
                            CurrentIndex++;
                        }
                        CursorPos = CurrentIndex;
                        ClearSelection();

                    }
                    else
                    {
                        CursorPos += 1;
                        ClearSelection();

                    }
                    CursorPos = Math.Min(CursorPos, Text.Length);
                    RenderCursor = true;
                    Delay = 0.05f;
                }
                else if (!Ctx.KeyDown((int)Keys.LeftShift))
                {
                    ClearSelection();
                }
            }
            if (SelectionZone.X == SelectionZone.Y)
            {
                ClearSelection();
            }
        }
        internal void Action(Context Ctx, ref string Text, int MaxSize, ref bool Result, TextEditFlag Flag)
        {
            BackAndDel(Ctx, ref Text, ref Result);
            if (Ctx.KeyDown((int)Keys.Enter))
            {
                if (Flag.HasFlag(TextEditFlag.EnterExecute))
                {
                    Result = true;
                }
                if (Flag.HasFlag(TextEditFlag.Multiline))
                {
                    bool Work = true;
                    if (Flag.HasFlag(TextEditFlag.EnterExecute))
                    {
                        Work = Ctx.KeyDown((int)Keys.LeftControl);
                    }
                    if (Work)
                    {
                        if (Text.Length < MaxSize)
                        {
                            Text += '\n';
                        }
                        Delay = 0.2f;
                        ClearSelection();
                        CursorPos += 1;
                    }
                }
            }
            Arrow(Ctx, ref Text);
        }
        internal void InputedChar(Context Ctx, ref string Text, int MaxSize, ref bool Result, TextEditFlag Flag)
        {
            if (Ctx.InputedChar != 0)
            {
                if (Text.Length < MaxSize)
                {
                    if (Ctx.InputedChar != 0)
                    {
                        if (Flag.HasFlag(TextEditFlag.NumberOnly) && !char.IsNumber(Ctx.InputedChar))
                        {
                            return;
                        }
                        if (hasSelection)
                        {
                            Text = Text.Remove(SelectionZone.X, SelectionZone.Y - SelectionZone.X);
                            CursorPos = SelectionZone.X;
                            ClearSelection();
                        }
                        Text = Text.Insert(CursorPos, Ctx.InputedChar.ToString());
                        CursorPos += 1;
                        Result = true;
                        RenderCursor = true;

                    }

                }

                Ctx.InputedChar = (char)0;

            }
        }
        internal void Cursor(float DT)
        {
            AccumulatedDT += DT;
            if (AccumulatedDT >= 0.5f)
            {
                RenderCursor = !RenderCursor;
                AccumulatedDT = 0;
            }
        }
        internal void Inital(Context Ctx, Rect Zone, string Text, Vector2i TextPos)
        {
            if (Ctx.IsHover(Zone) && Ctx.MouseDown(0) && !StartZoneSelection)
            {
                int Index = MousePositionToIndex(Text, TextPos, Ctx, Ctx.font, 20, Ctx.MousePosition);
                StartZoneSelection = true;
                StartPos = Index;
                CursorPos = Index;
            }
            if (StartZoneSelection && Ctx.MouseDown(0))
            {
                int Index = MousePositionToIndex(Text, TextPos, Ctx, Ctx.font, 20, Ctx.MousePosition);
                CursorPos = Index;
                if (StartPos > Index)
                {
                    SelectionZone.Y = StartPos;
                    SelectionZone.X = Index;
                }
                else
                {
                    SelectionZone.X = StartPos;
                    SelectionZone.Y = Index;
                }
            }
            else if (StartZoneSelection && Ctx.MouseUp(0))
            {
                if (SelectionZone.X == SelectionZone.Y)
                {
                    ClearSelection();
                }
                StartZoneSelection = false;
            }
        }
        internal bool Update(Context Ctx, ref string Text, int MaxSize, Rect Zone, Vector2i TextPos, TextEditFlag Flags)
        {
            if (CursorPos > Text.Length || CursorPos == -1)
            {
                CursorPos = Text.Length;
            }
            Inital(Ctx, Zone, Text, TextPos);
            Cursor(Ctx.DT);
            bool Result = false;
            if (Delay > 0)
            {
                Delay -= Ctx.DT;
            }
            else
            {
                Action(Ctx, ref Text, MaxSize, ref Result, Flags);
            }
            InputedChar(Ctx, ref Text, MaxSize, ref Result, Flags);
            return Result;
        }

    }
    internal class TabBar
    {
        internal ulong ID;
        internal ulong LastTick;
        internal List<TabItem> TabItems =new();
        internal bool Empty = true;
        internal ulong SelectedTabItem ;
    }
    internal class TabItem
    {
        internal ulong ID;
        internal  string Text;
    }
    public class Vector<T>
    {
        public int Size { get; set; }
        public int Capacity { get; set; }
        public T[] Data { get; set; }

        public Vector()
        {
            Size = Capacity = 0;
            Data = Array.Empty<T>();
        }
        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= Size)
                    throw new ArgumentOutOfRangeException();
                return Data[i];
            }
            set
            {
                if (i < 0 || i >= Size)
                    throw new ArgumentOutOfRangeException();
                Data[i] = value;
            }
        }


        private int GrowCapacity(int sz)
        {
            int newCapacity = Capacity > 0 ? Capacity + Capacity / 2 : 8;
            return newCapacity > sz ? newCapacity : sz;
        }

        public void Resize(int newSize)
        {
            if (newSize > Capacity)
                Reserve(GrowCapacity(newSize));
            Size = newSize;
        }

        public void Reserve(int newCapacity)
        {
            if (newCapacity <= Capacity)
                return;

            T[] newData = new T[newCapacity];
            if (Data != null)
                Array.Copy(Data, newData, Size);
            Data = newData;
            Capacity = newCapacity;
        }

        public void PushBack(T value)
        {
            if (Size == Capacity)
                Reserve(GrowCapacity(Size + 1));
            Data[Size++] = value;
        }
    }
    /*
    MIT License
    Copyright (c) 2018 Melnik Alexander
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */
    public static class xxHash64
    {
        private static readonly ulong XXH_PRIME64_1 = 11400714785074694791UL;
        private static readonly ulong XXH_PRIME64_2 = 14029467366897019727UL;
        private static readonly ulong XXH_PRIME64_3 = 1609587929392839161UL;
        private static readonly ulong XXH_PRIME64_4 = 9650029242287828579UL;
        private static readonly ulong XXH_PRIME64_5 = 2870177450012600261UL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH_rotl64(ulong x, int r)
        {
            return x << r | x >> 64 - r;
        }
        /// <summary>
        /// Compute xxHash for the data byte array
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(byte[] data, int length, ulong seed = 0)
        {
            fixed (byte* pData = &data[0])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        /// <summary>
        /// Compute xxHash for the data byte array
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(byte[] data, int offset, int length, ulong seed = 0)
        {
            fixed (byte* pData = &data[0 + offset])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeComputeHash(byte* ptr, int length, ulong seed)
        {
            return __inline__XXH64(ptr, length, seed);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_round(ulong acc, ulong input)
        {
            acc += input * XXH_PRIME64_2;
            acc = XXH_rotl64(acc, 31);
            acc *= XXH_PRIME64_1;
            return acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_avalanche(ulong hash)
        {
            hash ^= hash >> 33;
            hash *= XXH_PRIME64_2;
            hash ^= hash >> 29;
            hash *= XXH_PRIME64_3;
            hash ^= hash >> 32;
            return hash;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong __inline__XXH64(byte* input, int len, ulong seed)
        {
            ulong h64;

            if (len >= 32)
            {
                byte* end = input + len;
                byte* limit = end - 31;
                ulong v1 = seed + XXH_PRIME64_1 + XXH_PRIME64_2;
                ulong v2 = seed + XXH_PRIME64_2;
                ulong v3 = seed + 0;
                ulong v4 = seed - XXH_PRIME64_1;
                do
                {
                    var reg1 = *(ulong*)(input + 0);
                    var reg2 = *(ulong*)(input + 8);
                    var reg3 = *(ulong*)(input + 16);
                    var reg4 = *(ulong*)(input + 24);

                    // XXH64_round
                    v1 += reg1 * XXH_PRIME64_2;
                    v1 = v1 << 31 | v1 >> 64 - 31;
                    v1 *= XXH_PRIME64_1;

                    // XXH64_round
                    v2 += reg2 * XXH_PRIME64_2;
                    v2 = v2 << 31 | v2 >> 64 - 31;
                    v2 *= XXH_PRIME64_1;

                    // XXH64_round
                    v3 += reg3 * XXH_PRIME64_2;
                    v3 = v3 << 31 | v3 >> 64 - 31;
                    v3 *= XXH_PRIME64_1;

                    // XXH64_round
                    v4 += reg4 * XXH_PRIME64_2;
                    v4 = v4 << 31 | v4 >> 64 - 31;
                    v4 *= XXH_PRIME64_1;
                    input += 32;
                } while (input < limit);

                h64 = (v1 << 1 | v1 >> 64 - 1) +
                      (v2 << 7 | v2 >> 64 - 7) +
                      (v3 << 12 | v3 >> 64 - 12) +
                      (v4 << 18 | v4 >> 64 - 18);

                // XXH64_mergeRound
                v1 *= XXH_PRIME64_2;
                v1 = v1 << 31 | v1 >> 64 - 31;
                v1 *= XXH_PRIME64_1;
                h64 ^= v1;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v2 *= XXH_PRIME64_2;
                v2 = v2 << 31 | v2 >> 64 - 31;
                v2 *= XXH_PRIME64_1;
                h64 ^= v2;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v3 *= XXH_PRIME64_2;
                v3 = v3 << 31 | v3 >> 64 - 31;
                v3 *= XXH_PRIME64_1;
                h64 ^= v3;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;

                // XXH64_mergeRound
                v4 *= XXH_PRIME64_2;
                v4 = v4 << 31 | v4 >> 64 - 31;
                v4 *= XXH_PRIME64_1;
                h64 ^= v4;
                h64 = h64 * XXH_PRIME64_1 + XXH_PRIME64_4;
            }
            else
            {
                h64 = seed + XXH_PRIME64_5;
            }

            h64 += (ulong)len;

            // XXH64_finalize
            len &= 31;
            while (len >= 8)
            {
                ulong k1 = XXH64_round(0, *(ulong*)input);
                input += 8;
                h64 ^= k1;
                h64 = XXH_rotl64(h64, 27) * XXH_PRIME64_1 + XXH_PRIME64_4;
                len -= 8;
            }
            if (len >= 4)
            {
                h64 ^= *(uint*)input * XXH_PRIME64_1;
                input += 4;
                h64 = XXH_rotl64(h64, 23) * XXH_PRIME64_2 + XXH_PRIME64_3;
                len -= 4;
            }
            while (len > 0)
            {
                h64 ^= *input++ * XXH_PRIME64_5;
                h64 = XXH_rotl64(h64, 11) * XXH_PRIME64_1;
                --len;
            }
            // XXH64_avalanche
            h64 ^= h64 >> 33;
            h64 *= XXH_PRIME64_2;
            h64 ^= h64 >> 29;
            h64 *= XXH_PRIME64_3;
            h64 ^= h64 >> 32;
            return h64;
        }
    }
}
