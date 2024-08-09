/*
 Copyright (c) 2020 rxi

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

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

using OpenTK.Mathematics;

namespace VoxelPrototype.client.ui.prototype
{
    public static class VoxelGui
    {
        const int COMMANDLIST_SIZE = 256 * 1024;
        const int ROOTLIST_SIZE = 32;
        const int CONTAINERSTACK_SIZE = 32;
        const int CLIPSTACK_SIZE = 32;
        const int IDSTACK_SIZE = 32;
        const int LAYOUTSTACK_SIZE = 16;
        const int CONTAINERPOOL_SIZE = 48;
        const int TREENODEPOOL_SIZE = 48;
        const int MAX_WIDTHS = 16;
        const string REAL_FMT = "{0:0.###}";
        const string SLIDER_FMT = "{0:0.00}";
        const int MAX_FMT = 127;
        public class Stack<T>
        {
            public int idx;
            public T[] items;

            public Stack(int size)
            {
                idx = 0;
                items = new T[size];
            }
            public void Push(T Value)
            {
                if (idx < items.Length)
                {
                    items[idx] = Value;
                    idx++;

                }
                else
                {
                    throw new StackOverflowException();
                }
            }
            public T Pop()
            {
                if (idx > 0)
                {
                    idx--;
                    return items[idx];
                }
                else
                {
                    throw new Exception("StackUnderflow");
                }
            }
        }
        public enum Clip
        {
            CLIP_PART = 1,
            CLIP_ALL
        };
        public enum Commands
        {
            COMMAND_JUMP = 1,
            COMMAND_CLIP,
            COMMAND_RECT,
            COMMAND_TEXT,
            COMMAND_ICON,
            COMMAND_MAX
        };
        public enum Color
        {
            COLOR_TEXT,
            COLOR_BORDER,
            COLOR_WINDOWBG,
            COLOR_TITLEBG,
            COLOR_TITLETEXT,
            COLOR_PANELBG,
            COLOR_BUTTON,
            COLOR_BUTTONHOVER,
            COLOR_BUTTONFOCUS,
            COLOR_BASE,
            COLOR_BASEHOVER,
            COLOR_BASEFOCUS,
            COLOR_SCROLLBASE,
            COLOR_SCROLLTHUMB,
            COLOR_MAX
        }
        public enum Icon
        {
            ICON_CLOSE = 1,
            ICON_CHECK,
            ICON_COLLAPSED,
            ICON_EXPANDED,
            ICON_MAX
        }
        public enum Res
        {
            RES_ACTIVE = 0,
            RES_SUBMIT = 1,
            RES_CHANGE = 2
        }
        public enum Opt
        {
            OPT_ALIGNCENTER = 1 << 0,
            OPT_ALIGNRIGHT = 1 << 1,
            OPT_NOINTERACT = 1 << 2,
            OPT_NOFRAME = 1 << 3,
            OPT_NORESIZE = 1 << 4,
            OPT_NOSCROLL = 1 << 5,
            OPT_NOCLOSE = 1 << 6,
            OPT_NOTITLE = 1 << 7,
            OPT_HOLDFOCUS = 1 << 8,
            OPT_AUTOSIZE = 1 << 9,
            OPT_POPUP = 1 << 10,
            OPT_CLOSED = 1 << 11,
            OPT_EXPANDED = 1 << 12
        }
        public enum Mouse
        {
            MOUSE_LEFT = 1 << 0,
            MOUSE_RIGHT = 1 << 1,
            MOUSE_MIDDLE = 1 << 2
        };
        public enum Key
        {
            KEY_SHIFT = 1 << 0,
            KEY_CTRL = 1 << 1,
            KEY_ALT = 1 << 2,
            KEY_BACKSPACE = 1 << 3,
            KEY_RETURN = 1 << 4
        };
        public enum Lay { RELATIVE = 1, ABSOLUTE = 2 };

        public struct Rect
        {
            public int X, Y, W, H;
            public static Rect UnclippedRect = new() { X = 0, Y = 0, W = 0x1000000, H = 0x1000000 };

            public Rect(int x, int y, int w, int h)
            {
                X = x;
                Y = y;
                W = w;
                H = h;
            }
            public Rect Expand(int N)
            {
                return new Rect(X - N, Y - N, W + 2 * N, H + 2 * N);
            }
            public Rect Intersect(Rect rect)
            {
                int x1 = Math.Max(X, rect.X);
                int y1 = Math.Max(Y, rect.Y);
                int x2 = Math.Min(X + W, rect.X + rect.W);
                int y2 = Math.Min(Y + H, rect.Y + rect.H);
                if (x2 < x1) { x2 = x1; }
                if (y2 < y1) { y2 = y1; }
                return new Rect(x1, y1, x2 - x1, y2 - y1);
            }
            public static bool RectOverLapsVec1(Rect r, Vector2i p)
            {
                return p.X >= r.X && p.X < r.X + r.W && p.Y >= r.Y && p.Y < r.Y + r.H;
            }
        }
        public struct PoolItem
        {
            public uint ID;
            public int LastUpdate;
        }
        public class Command
        {
            Commands Type;
        }
        public class JumpCommand : Command
        {
            public int Dest;
        }
        public class ClipCommand : Command
        {
            public Rect Rect;
        }
        public class RectCommand : Command
        {
            public Rect Rect;
            public Vector4 Color;
        }
        public class TextCommand : Command
        {
            public string Font;
            public Vector2i Pos;
            public Vector4 Color;
            public string Text;
        }
        public class IconCommand : Command
        {
            public Rect Rect;
            public Icon Icon;
            public uint Color;
        }

        public struct Layout
        {
            public Rect Body;
            public Rect Next;
            public Vector2i Position;
            public Vector2i Size;
            public Vector2i Max;
            public int[] Widths = new int[MAX_WIDTHS];
            public int Items;
            public int Item_index;
            public int Next_row;
            public Lay Next_type;
            public int Indent;
            public Layout()
            {
            }
        }

        public class Container
        {
            public int Head, Tail;
            public Rect Rect;
            public Rect Body;
            public Vector2i ContentSize;
            public Vector2i Scroll;
            public int ZIndex;
            public bool Open;
        }
        public class Style
        {
            public string Font;
            public Vector2i Size;
            public int Padding;
            public int Spacing;
            public int Indent;
            public int TitleHeight;
            public int ScrollBarSize;
            public int ThumnSize;
            public Vector4[] Colors = new Vector4[(int)Color.COLOR_MAX];
            //default
            public static Style DefaultStyle = new Style()
            {
                Font = null,
                Size = new(68, 10),
                Padding = 5,
                Spacing = 4,
                Indent = 24,
                TitleHeight = 24,
                ScrollBarSize = 12,
                ThumnSize = 8,
                Colors = [
                    new(0.9f, 0.9f, 0.9f,1),
                    new(0.1f, 0.1f, 0.1f,1),
                    new(0.2f, 0.2f, 0.2f,1),
                    new(0.1f, 0.1f, 0.1f,1),
                    new(0.94f, 0.94f, 0.94f,1),
                    new(0,0, 0,0),
                    new(0.29f, 0.29f, 0.29f,1),
                    new(0.37f, 0.37f, 0.37f,1),
                    new(0.45f, 0.45f, 0.45f,1),
                    new(0.12f, 0.12f, 0.12f,1),
                    new(0.14f, 0.14f, 0.14f,1),
                    new(0.16f, 0.16f, 0.16f,1),
                    new(0.17f, 0.17f, 0.17f,1),
                    new(0.12f, 0.12f, 0.12f,1),
                ]
            };

            public Style()
            {
            }
        }
        /* Define the delegates for the callbacks */
        public delegate int TextWidthDelegate(string Font, string Str);
        public delegate int TextHeightDelegate(string Font);
        public delegate void DrawFrameDelegate(Rect Rect, Color ColorID);

        public class Context
        {
            //Calbacks
            public TextWidthDelegate TextWidth;
            public TextHeightDelegate TextHeight;
            public DrawFrameDelegate DrawFrame;
            //Core
            internal Style Style;
            internal uint Hover;
            internal uint Focus;
            internal uint LastID;
            internal Rect LastRect;
            internal int LastZIndex;
            internal bool UpdatedFocus;
            internal int Frame;
            internal Container HoverRoot;
            internal Container NextHoverRoot;
            internal Container ScrollTarget;
            internal string NumberEditBuf = "";
            internal uint NumberEdit;
            //Stacks
            internal Stack<Command> CommandList = new(COMMANDLIST_SIZE);
            internal Stack<Container> RootList = new(ROOTLIST_SIZE);
            internal Stack<Container> ContainerStack = new(CONTAINERSTACK_SIZE);
            internal Stack<Rect> ClipStack = new(CLIPSTACK_SIZE);
            internal Stack<uint> IDStack = new(IDSTACK_SIZE);
            internal Stack<Layout> LayoutStack = new(LAYOUTSTACK_SIZE);
            //Retained state pools
            internal PoolItem[] ContainerPool = new PoolItem[CONTAINERPOOL_SIZE];
            internal Container[] Containers = new Container[CONTAINERPOOL_SIZE];
            internal PoolItem[] TreeNodePool = new PoolItem[CONTAINERPOOL_SIZE];
            //Input State
            internal Vector2i MousePos;
            internal Vector2i LasMousePos;
            internal Vector2i MouseDelta;
            internal Vector2i ScrollDelta;
            internal int MouseDown;
            internal int MousePressed;
            internal int KeyDown;
            internal int KeyPressed;
            internal string InputText;
            public Context()
            {
            }
        }
        public static Context UIContext = new();
        public static void DrawFrame(Rect Rect, Color ColorID)
        {
            DrawRect(Rect, UIContext.Style.Colors[(int)ColorID]);
            if (ColorID == Color.COLOR_SCROLLBASE || ColorID == Color.COLOR_SCROLLTHUMB || ColorID == Color.COLOR_TITLEBG)
            {
                return;
            }
            if (UIContext.Style.Colors[(int)Color.COLOR_BORDER].W != 0)
            {
                DrawBox(Rect.Expand(1), UIContext.Style.Colors[(int)Color.COLOR_BORDER]);
            }
        }
        public static void Init()
        {
            UIContext.DrawFrame = DrawFrame;
            UIContext.Style = Style.DefaultStyle;
        }
        public static void Begin()
        {
            UIContext.CommandList.idx = 0;
            UIContext.RootList.idx = 0;
            UIContext.ScrollTarget = null;
            UIContext.HoverRoot = UIContext.NextHoverRoot;
            UIContext.NextHoverRoot = null;
            UIContext.MouseDelta = UIContext.MousePos - UIContext.LasMousePos;
            UIContext.Frame++;
        }
        private static int CompareZIndex(Container A, Container B)
        {
            return A.ZIndex - B.ZIndex;
        }
        private static void End()
        {
            int i, n;
            System.Diagnostics.Debug.Assert(UIContext.ContainerStack.idx == 0);
            System.Diagnostics.Debug.Assert(UIContext.ClipStack.idx == 0);
            System.Diagnostics.Debug.Assert(UIContext.IDStack.idx == 0);
            System.Diagnostics.Debug.Assert(UIContext.LayoutStack.idx == 0);
            if (UIContext.ScrollTarget != null)
            {
                UIContext.ScrollTarget.Scroll += UIContext.ScrollDelta;
            }
            if (!UIContext.UpdatedFocus) { UIContext.Focus = 0; }
            UIContext.UpdatedFocus = false;
            if (UIContext.MousePressed != 0 && UIContext.NextHoverRoot != null &&
              UIContext.NextHoverRoot.ZIndex < UIContext.LastZIndex &&
              UIContext.NextHoverRoot.ZIndex >= 0
            )
            {
                BringToFront(UIContext.NextHoverRoot);
            }
            /* reset input state */
            UIContext.KeyPressed = 0;
            UIContext.InputText = "";
            UIContext.MousePressed = 0;
            UIContext.ScrollDelta = Vector2i.Zero;
            UIContext.LasMousePos = UIContext.MousePos;

            /* sort root containers by zindex */
            n = UIContext.RootList.idx;
            Array.Sort(UIContext.RootList.items, CompareZIndex);
            /* set root container jump commands */
            for (i = 0; i < n; i++)
            {
                Container cnt = UIContext.RootList.items[i];
                /* if this is the first container then make the first command jump to it.
                ** otherwise set the previous container's tail to jump to this one */
                if (i == 0)
                {
                    JumpCommand cmd = (JumpCommand)UIContext.CommandList.items[0];
                    cmd.Dest = cnt.Head + 1;
                }
                else
                {
                    Container prev = UIContext.RootList.items[i - 1];
                    JumpCommand cmd = (JumpCommand)UIContext.CommandList.items[prev.Tail];
                    cmd.Dest = cnt.Head + 1;
                }
                /* make the last container's tail jump to the end of command list */
                if (i == n - 1)
                {
                    JumpCommand cmd = (JumpCommand)UIContext.CommandList.items[cnt.Tail];
                    cmd.Dest = UIContext.CommandList.items.Length;
                }
            }
        }
        static void SetFocus(uint id)
        {
            UIContext.Focus = id;
            UIContext.UpdatedFocus = true;
        }
        const uint HASH_INITIAL = 2166136261;
        static uint HashString(uint hash, string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                hash = (hash ^ data[i]) & HASH_INITIAL;
            }
            return hash;
        }
        static uint Hash(uint hash, object data)
        {
            return HashString(hash, data.ToString());
        }
        static uint GetID(string Name)
        {
            int idx = UIContext.IDStack.idx;
            uint res = idx > 0 ? UIContext.IDStack.items[idx - 1] : HASH_INITIAL;
            var FHash = Hash(res, Name);
            UIContext.LastID = FHash;
            return FHash;
        }
        static void PushID(string Name)
        {
            UIContext.IDStack.Push(GetID(Name));
        }
        static void PopID()
        {
            UIContext.IDStack.Pop();
        }
        static void PushClipRect(Rect Rect)
        {
            Rect last = GetClipRect();
            UIContext.ClipStack.Push(Rect.Intersect(last));
        }
        static void PopClipRect()
        {
            UIContext.ClipStack.Pop();
        }
        static Rect GetClipRect()
        {
            System.Diagnostics.Debug.Assert(UIContext.ClipStack.idx > 0);
            return UIContext.ClipStack.items[UIContext.ClipStack.idx - 1];
        }
        static Clip CheckClip(Rect Rect)
        {
            Rect cr = GetClipRect();
            if (Rect.X > cr.X + cr.W || Rect.X + Rect.W < cr.X ||
                Rect.Y > cr.Y + cr.H || Rect.Y + Rect.H < cr.Y) { return Clip.CLIP_ALL; }
            if (Rect.X >= cr.X && Rect.X + Rect.W <= cr.X + cr.W &&
                Rect.Y >= cr.Y && Rect.Y + Rect.H <= cr.Y + cr.H) { return 0; }
            return Clip.CLIP_PART;
        }
        static void PushLayout(Rect Body, Vector2i Scroll)
        {
            Layout layout = new();
            layout.Body = new Rect(Body.X - Scroll.X, Body.Y - Scroll.Y, Body.W, Body.H);
            layout.Max = new Vector2i(-0x1000000, -0x1000000);
            UIContext.LayoutStack.Push(layout);
            LayoutRow(1, [0], 0);
        }
        static ref Layout GetLayout()
        {
            return ref UIContext.LayoutStack.items[UIContext.LayoutStack.idx - 1];
        }
        static void PopContainer()
        {
            Container cnt = GetCurrentContainer();
            ref Layout layout = ref GetLayout();
            cnt.ContentSize.X = layout.Max.X - layout.Body.X;
            cnt.ContentSize.Y = layout.Max.Y - layout.Body.Y;
            /* pop container, layout and id */
            UIContext.ContainerStack.Pop();
            UIContext.LayoutStack.Pop();
            PopID();
        }
        static Container GetCurrentContainer()
        {
            System.Diagnostics.Debug.Assert(UIContext.ContainerStack.idx > 0);
            return UIContext.ContainerStack.items[UIContext.ContainerStack.idx - 1];
        }
        static Container GetContainer(uint ID, Opt opt)
        {
            Container cnt;
            /* try to get existing container from pool */
            int idx = PoolGet(ref UIContext.ContainerPool, ID);
            if (idx >= 0)
            {
                if (UIContext.Containers[idx].Open || (~(int)opt & (int)Opt.OPT_CLOSED) != 0)
                {
                    PoolUpdate(ref UIContext.ContainerPool, idx);
                }
                return UIContext.Containers[idx];
            }
            if (((int)opt & (int)Opt.OPT_CLOSED) != 0) { return null; }
            /* container not found in pool: init new container */
            idx = PoolInit(ref UIContext.ContainerPool, ID);
            cnt = UIContext.Containers[idx];
            cnt.Open = true;
            BringToFront(cnt);
            return cnt;
        }
        static Container GetContainer(string Name)
        {
            uint id = GetID(Name);
            return GetContainer(id, 0);
        }
        static void BringToFront(Container Cnt)
        {
            Cnt.ZIndex = ++UIContext.LastZIndex;
        }
        //
        //Pool
        //
        static int PoolInit(ref PoolItem[] Items, uint ID)
        {
            int i, n = -1, f = UIContext.Frame;
            for (i = 0; i < Items.Length; i++)
            {
                if (Items[i].LastUpdate < f)
                {
                    f = Items[i].LastUpdate;
                    n = i;
                }
            }
            System.Diagnostics.Debug.Assert(n > -1);
            Items[n].ID = ID;
            PoolUpdate(ref Items, n);
            return n;
        }
        static int PoolGet(ref PoolItem[] Items, uint ID)
        {
            int i;
            for (i = 0; i < Items.Length; i++)
            {
                if (Items[i].ID == ID) { return i; }
            }
            return -1;
        }
        static void PoolUpdate(ref PoolItem[] Items, int Idx)
        {
            Items[Idx].LastUpdate = UIContext.Frame;
        }
        //
        //Input
        //
        static void InputMouseMove(int X, int Y)
        {
            UIContext.MousePos = new(X, Y);
        }
        static void InputMousedown(int X, int Y, int Btn)
        {
            InputMouseMove(X, Y);
            UIContext.MouseDown |= Btn;
            UIContext.MousePressed |= Btn;
        }
        static void InputMouseUp(int X, int Y, int Btn)
        {
            InputMouseMove(X, Y);
            UIContext.MouseDown &= ~Btn;
        }
        static void InputScroll(int X, int Y)
        {
            UIContext.ScrollDelta.X += X;
            UIContext.ScrollDelta.Y += Y;
        }
        static void InputKeydown(int Key)
        {
            UIContext.KeyPressed |= Key;
            UIContext.KeyDown |= Key;
        }
        static void InputKeyUp(int Key)
        {
            UIContext.KeyDown &= ~Key;
        }
        static void InputText(string Text)
        {
            UIContext.InputText = Text;
        }
        //
        //CommandList
        //
        static int PushJump(int Dst)
        {
            UIContext.CommandList.Push(new JumpCommand() { Dest = Dst });
            return UIContext.CommandList.idx;
        }

        static void SetClip(Rect rect)
        {
            UIContext.CommandList.Push(new ClipCommand() { Rect = rect });
        }
        static void DrawRect(Rect Rect, Vector4 Color)
        {
            Rect = Rect.Intersect(GetClipRect());
            if (Rect.W > 0 && Rect.H > 0)
            {
                UIContext.CommandList.Push(new RectCommand() { Rect = Rect, Color = Color });
            }
        }
        static void DrawBox(Rect Rect, Vector4 Color)
        {
            DrawRect(new(Rect.X + 1, Rect.Y, Rect.W - 2, 1), Color);
            DrawRect(new(Rect.X + 1, Rect.Y + Rect.H - 1, Rect.W - 2, 1), Color);
            DrawRect(new(Rect.X, Rect.Y, 1, Rect.H), Color);
            DrawRect(new(Rect.X + Rect.W - 1, Rect.Y, 1, Rect.H), Color);
        }
        static void DrawText(string Font, string Str, Vector2i Pos, Vector4 Color)
        {
            Rect Rect = new(Pos.X, Pos.Y, UIContext.TextWidth(Font, Str), UIContext.TextHeight(Font));
            Clip Clipped = CheckClip(Rect);
            if (Clipped == Clip.CLIP_ALL)
            {
                return;
            }
            if (Clipped == Clip.CLIP_PART)
            {
                SetClip(GetClipRect());
            }
            UIContext.CommandList.Push(new TextCommand()
            {
                Font = Font,
                Pos = Pos,
                Text = Str,
                Color = Color
            });
            if (Clipped != 0)
            {
                SetClip(Rect.UnclippedRect);
            }
        }
        static void DrawIcon(Icon Icon, Rect Rect, Vector4 Color)
        {
            Clip Clipped = CheckClip(Rect);
            if (Clipped == Clip.CLIP_ALL)
            {
                return;
            }
            if (Clipped == Clip.CLIP_PART)
            {
                SetClip(GetClipRect());
            }
            if (Clipped != 0)
            {
                SetClip(Rect.UnclippedRect);
            }
        }
        //
        //Layout
        //
        static void LayoutBeginColumn()
        {
            PushLayout(LayoutNext(), Vector2i.Zero);
        }
        static void LayoutEndColumn()
        {
            ref Layout b = ref GetLayout();
            UIContext.LayoutStack.Pop();
            /* inherit position/next_row/max from child layout if they are greater */
            ref Layout a = ref GetLayout();
            a.Position.X = Math.Max(a.Position.X, b.Position.X + b.Body.Y - a.Body.Y);
            a.Max.X = Math.Max(a.Max.X, b.Max.X);
            a.Max.Y = Math.Max(a.Max.Y, b.Max.Y);
        }
        static void LayoutRow(int Items, int[] Widths, int Height)
        {
            ref Layout layout = ref GetLayout();
            if (Widths != null && Widths.Length > 0)
            {
                System.Diagnostics.Debug.Assert(Items <= MAX_WIDTHS);
                layout.Widths = Widths;
            }
            layout.Items = Items;
            layout.Position = new(layout.Indent, layout.Next_row);
            layout.Size.Y = Height;
            layout.Item_index = 0;
        }
        static void LayoutWidth(int Width)
        {
            GetLayout().Size.X = Width;
        }
        static void LayoutHeight(int Height)
        {
            GetLayout().Size.Y = Height;
        }
        static void LayoutSetNext(Rect Rect, bool Relative)
        {
            ref Layout layout = ref GetLayout();
            layout.Next = Rect;
            layout.Next_type = Relative ? Lay.RELATIVE : Lay.ABSOLUTE;
        }
        static Rect LayoutNext()
        {
            ref Layout layout = ref GetLayout();
            Style style = UIContext.Style;
            Rect res;
            if ((int)layout.Next_type == 1)
            {
                /* handle rect set by `mu_layout_set_next` */
                int type = (int)layout.Next_type;
                layout.Next_type = 0;
                res = layout.Next;
                if (type == (int)Lay.ABSOLUTE) { return UIContext.LastRect = res; }

            }
            else
            {
                /* handle next row */
                if (layout.Item_index == layout.Items)
                {
                    LayoutRow(layout.Items, null, layout.Size.Y);
                }

                /* position */
                res.X = layout.Position.X;
                res.Y = layout.Position.Y;

                /* size */
                res.W = layout.Items > 0 ? layout.Widths[layout.Item_index] : layout.Size.X;
                res.H = layout.Size.Y;
                if (res.W == 0) { res.W = style.Size.X + style.Padding * 2; }
                if (res.H == 0) { res.H = style.Size.Y + style.Padding * 2; }
                if (res.W < 0) { res.W += layout.Body.W - res.X + 1; }
                if (res.H < 0) { res.H += layout.Body.H - res.Y + 1; }

                layout.Item_index++;
            }
            layout.Position.X += res.W + style.Spacing;
            layout.Next_row = Math.Max(layout.Next_row, res.Y + res.H + style.Spacing);

            /* apply body offset */
            res.X += layout.Body.X;
            res.Y += layout.Body.Y;

            /* update max position */
            layout.Max.X = Math.Max(layout.Max.X, res.X + res.W);
            layout.Max.Y = Math.Max(layout.Max.Y, res.Y + res.H);

            return UIContext.LastRect = res;
        }
        //
        //Controls
        //
        static bool InHoverRoot()
        {
            int i = UIContext.ContainerStack.idx;
            while (i-- > 0)
            {
                if (UIContext.ContainerStack.items[i] == UIContext.HoverRoot) { return true; }
                /* only root containers have their `head` field set; stop searching if we've
                ** reached the current root container */
                if (UIContext.ContainerStack.items[i].Head != 0) { break; }
            }
            return false;
        }
        static void DrawControlFrame(uint ID, Rect Rect, Color Color, Opt opt)
        {
            if (((int)opt & (int)Opt.OPT_NOFRAME) != 0) { return; }
            Color += UIContext.Focus == ID ? 2 : UIContext.Hover == ID ? 1 : 0;
            UIContext.DrawFrame(Rect, Color);
        }
        static void DrawControlText(string Str, Rect Rect, Color Colorid, Opt opt)
        {
            Vector2i pos;
            string font = UIContext.Style.Font;
            int tw = UIContext.TextWidth(font, Str);
            PushClipRect(Rect);
            pos.Y = Rect.Y + (Rect.H - UIContext.TextHeight(font)) / 2;
            if (((int)opt & (int)Opt.OPT_ALIGNCENTER) != 0)
            {
                pos.X = Rect.X + (Rect.W - tw) / 2;
            }
            else if (((int)opt & (int)Opt.OPT_ALIGNRIGHT) != 0)
            {
                pos.X = Rect.X + Rect.W - tw - UIContext.Style.Padding;
            }
            else
            {
                pos.X = Rect.X + UIContext.Style.Padding;
            }
            DrawText(font, Str, pos, UIContext.Style.Colors[(int)Colorid]);
            PopClipRect();
        }
        static bool MouseOver(Rect Rect)
        {
            return Rect.RectOverLapsVec1(Rect, UIContext.MousePos) &&
             Rect.RectOverLapsVec1(GetClipRect(), UIContext.MousePos) &&
              InHoverRoot();
        }
        static void UpdateControl(uint ID, Rect rect, Opt opt)
        {
            bool mouseover = MouseOver(rect);

            if (UIContext.Focus == ID) { UIContext.UpdatedFocus = true; }
            if (((int)opt & (int)Opt.OPT_NOINTERACT) != 0) { return; }
            if (mouseover && UIContext.MouseDown == 0) { UIContext.Hover = ID; }

            if (UIContext.Focus == ID)
            {
                if (UIContext.MousePressed != 0 && !mouseover) { SetFocus(0); }
                if (UIContext.MouseDown == 0 && (~(int)opt & (int)Opt.OPT_HOLDFOCUS) != 0) { SetFocus(0); }
            }

            if (UIContext.Hover == ID)
            {
                if (UIContext.MousePressed != 0)
                {
                    SetFocus(ID);
                }
                else if (!mouseover)
                {
                    UIContext.Hover = 0;
                }
            }
        }
        //Real Controls
        static void Text(string Text)
        {
            string start, end;
            int width = -1;
            string font = UIContext.Style.Font;
            Vector4 color = UIContext.Style.Colors[(int)Color.COLOR_TEXT];
            LayoutBeginColumn();
            LayoutRow(1, [-1], UIContext.TextHeight(font));
            int p = 0;
            do
            {
                Rect r = LayoutNext();
                int w = 0;
                start = end = Text.Substring(0);
                do
                {
                    int wordStart = p;
                    while (p < Text.Length && Text[p] != ' ' && Text[p] != '\n') { p++; }
                    string word = Text.Substring(wordStart, p - wordStart);
                    w += UIContext.TextWidth(font, word);
                    if (w > r.W && end != start) { break; }
                    if (p < Text.Length)
                    {
                        w += UIContext.TextWidth(font, Text.Substring(p, 1));
                    }
                    end = Text.Substring(p, 1);
                    p++;
                } while (end != "\n" && p < Text.Length);
                DrawText(font, Text, new(r.Y, r.Y), color);
                p = Text.IndexOf(end) + 1;
            } while (p < Text.Length);
            LayoutEndColumn();
        }
        static void Label(string Text)
        {
            DrawControlText(Text, LayoutNext(), Color.COLOR_TEXT, Opt.OPT_ALIGNCENTER);
        }
        static Res Button(string Label, Icon Icon = 0, Opt Opt = Opt.OPT_ALIGNCENTER)
        {
            Res res = 0;
            uint id = Label != "" ? GetID(Label)
                         : GetID(Icon.ToString());
            Rect r = LayoutNext();
            UpdateControl(id, r, Opt);
            /* handle click */
            if (UIContext.MousePressed == (int)Mouse.MOUSE_LEFT && UIContext.Focus == id)
            {
                res |= Res.RES_SUBMIT;
            }
            /* draw */
            DrawControlFrame(id, r, Color.COLOR_BUTTON, Opt);
            if (Label != "") { DrawControlText(Label, r, Color.COLOR_TEXT, Opt); }
            if (Icon != 0) { DrawIcon(Icon, r, UIContext.Style.Colors[(int)Color.COLOR_TEXT]); }
            return res;
        }
        static Res Checkbox(string Label, ref bool State)
        {
            Res res = 0;
            uint id = GetID(State.ToString());
            Rect r = LayoutNext();
            Rect box = new(r.X, r.Y, r.H, r.H);
            UpdateControl(id, r, 0);
            /* handle click */
            if (UIContext.MousePressed == (int)Mouse.MOUSE_LEFT && UIContext.Focus == 1)
            {
                res |= Res.RES_CHANGE;
                State = !State;
            }
            /* draw */
            DrawControlFrame(id, box, Color.COLOR_BASE, 0);
            if (State)
            {
                DrawIcon(Icon.ICON_CHECK, box, UIContext.Style.Colors[(int)Color.COLOR_TEXT]);
            }
            r = new(r.X + box.W, r.Y, r.W - box.W, r.H);
            DrawControlText(Label, r, Color.COLOR_TEXT, 0);
            return res;
        }
        static Res TextboxRaw(ref string Buffer, uint ID, Rect R, Opt Opt)
        {
            Res res = 0;
            UpdateControl(ID, R, Opt | Opt.OPT_HOLDFOCUS);

            if (UIContext.Focus == ID)
            {
                if (UIContext.InputText.Length > 0)
                {
                    Buffer += UIContext.InputText;
                    res |= Res.RES_CHANGE;
                }


                // Handle backspace
                if ((UIContext.KeyPressed & (int)Key.KEY_BACKSPACE) != 0)
                {
                    Buffer.Remove(Buffer.Length - 1);
                    res |= Res.RES_CHANGE;
                }

                // Handle return
                if ((UIContext.KeyPressed & (int)Key.KEY_RETURN) != 0)
                {
                    SetFocus(0);
                    res |= Res.RES_SUBMIT;
                }
            }
            /* draw */
            DrawControlFrame(ID, R, Color.COLOR_BASE, Opt);
            if (UIContext.Focus == ID)
            {
                Vector4 color = UIContext.Style.Colors[(int)Color.COLOR_TEXT];
                string font = UIContext.Style.Font;
                int textw = UIContext.TextWidth(font, Buffer);
                int texth = UIContext.TextHeight(font);
                int ofx = R.W - UIContext.Style.Padding - textw - 1;
                int textx = R.X + Math.Min(ofx, UIContext.Style.Padding);
                int texty = R.Y + (R.H - texth) / 2;
                PushClipRect(R);
                DrawText(font, Buffer, new(textx, texty), color);
                DrawRect(new(textx + textw, texty, 1, texth), color);
                PopClipRect();
            }
            else
            {
                DrawControlText(Buffer, R, Color.COLOR_TEXT, Opt);
            }
            return res;
        }
        static int NumberTextbox(ref float Value, Rect R, uint ID)
        {
            if (UIContext.MousePressed == (int)Mouse.MOUSE_LEFT && (UIContext.KeyDown & (int)Key.KEY_SHIFT) != 0 && UIContext.Hover == ID)
            {
                UIContext.NumberEdit = ID;
            }
            if (UIContext.NumberEdit == ID)
            {
                Res res = TextboxRaw(ref UIContext.NumberEditBuf, ID, R, 0);
                if (((int)res & (int)Res.RES_SUBMIT) != 0 || UIContext.Focus != ID)
                {
                    Value = float.Parse(UIContext.NumberEditBuf);
                    UIContext.NumberEdit = 0;
                }
                else
                {
                    return 1;
                }
            }
            return 0;
        }

        static Res Textbox(ref string Buffer, Opt Opt = 0)
        {
            uint id = GetID(Buffer);
            Rect R = LayoutNext();
            return TextboxRaw(ref Buffer, id, R, Opt);
        }
    }
}
