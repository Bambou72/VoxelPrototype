using OpenTK.Mathematics;
using ImmediateUI.immui.font;
using ImmediateUI.immui.math;
using ImmediateUI.immui.utils;
using System.Runtime.InteropServices;
namespace ImmediateUI.immui.rendering
{
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
        public Vector2 Position;
        public Vector2 UV;
        public uint Color;

        public Vertex(Vector2 position, Vector2 uV, uint color)
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
        //
        int CurrentTextureID = 0;
        Rect CurrentClipRect;
        public void AddRect(Vector2 Min, Vector2 Max, uint Color)
        {
            if ((Color & AlphaMask) == 0) return;
            PrimRect(Min, Max, Color);
        }
        public void AddRect(Rect Rect, uint Color)
        {
            if ((Color & AlphaMask) == 0) return;
            PrimRect(new(Rect.X,Rect.Y), new(Rect.XW, Rect.YH), Color);
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
        public void AddText(Vector2 Position, string Text, float Scale, uint Color, Font Font = null,Rect Clip = default)
        {
            if ((Color & AlphaMask) == 0)
            {
                return;
            }
            if (Font == null)
            {
                throw new("Font not ");// Font =Immui.GetContext().font;
            }
            if (Scale == 0.0f)
            {
                Scale = Font.Height;
            }
            PushTextureID(Font.Atlas.GetHandle());
            RenderText(Font, Scale, Position, Color, Text,Clip);
            PopTextureID();
        }
        public void RenderText(Font font, float Size, Vector2 Position, uint Color, string Text, Rect Clip = default)
        {
            bool UseClip = !Clip.Equals(default);
            int textEnd = Text.Length;
            int textBegin = 0;
            float X = MathF.Truncate(Position.X);
            float Y = MathF.Truncate(Position.Y);

            float start_x = X;
            float scale = Size / font.Height;
            float line_height = font.Height * scale;
            int s = textBegin;
            if (s == textEnd)
            {
                return;
            }
            int vtxCountMax = (textEnd - s) * 4;
            int idxCountMax = (textEnd - s) * 6;
            int idxExpectedSize = IndexBuffer.Size + idxCountMax;
            var Command = Commands[Commands.Count - 1] ;
            Command.Count += idxCountMax;
            Commands[Commands.Count - 1] = Command;
            uint colUntinted = Color | ~AlphaMask;
            int wordWrapEol = 0;
            while (s < textEnd)
            {
                char c = Text[s];
                s++;
                if (c == '\n')
                {
                    X = start_x;
                    if(UseClip && Y+line_height > Clip.YH)
                    {
                        break;
                    }
                    Y += line_height;
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
                float charWidth = (glyph.Advance >> 6) * scale;
                if(UseClip && X + charWidth > Clip.XW)
                {
                    break;
                }
                if (glyph.Rendarable)
                {
                    float x1 = X + glyph.Bearing.X * scale;
                    float x2 = x1 + glyph.Size.X * scale;
                    float y1 = Y - (glyph.Bearing.Y) * scale;
                    float y2 = y1 + glyph.Size.Y * scale;
                    float u1 = glyph.UV0.X;
                    float v1 = glyph.UV0.Y;
                    float u2 = glyph.UV1.X;
                    float v2 = glyph.UV1.Y;

                    VertexBuffer.PushBack(new Vertex(new Vector2(x1, y1), new Vector2(u1, v1), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector2(x2, y1), new Vector2(u2, v1), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector2(x2, y2), new Vector2(u2, v2), Color));
                    VertexBuffer.PushBack(new Vertex(new Vector2(x1, y2), new Vector2(u1, v2), Color));
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
        private void PrimRect(Vector2 A, Vector2 C, uint Color = 0xFFFFFFFF,Vector2 UVA = default, Vector2 UVC  =default)
        {
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
            VertexBuffer.PushBack(new(A, UVA, Color));
            VertexBuffer.PushBack(new(new(C.X, A.Y), new(UVC.X, UVA.Y), Color));
            VertexBuffer.PushBack(new(C, UVC, Color));
            VertexBuffer.PushBack(new(new(A.X, C.Y), new(UVA.X, UVC.Y), Color));
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
            CurrentClipRect = ClipStack.Count == 0 ? throw new Exception("Underflow :  you can't pop an empty stack"): ClipStack.Peek();
            OnChangedClipRect();
        }
        public void PushTextureID(int TextureID)
        {
            TextureStack.Push(TextureID);
            CurrentTextureID= TextureID;
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
            if (curr_cmd.Count != 0 && curr_cmd.ClipRect.Equals(CurrentClipRect))
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 2)
            {
                Command prev_cmd = Commands[^2];
                if (curr_cmd.Count == 0  &&  CurrentCommandTest(prev_cmd) && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
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
            if (Commands.Count > 2)
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
            if(!Cmd.ClipRect.Equals(CurrentClipRect) || Cmd.TextureID != CurrentTextureID)  return false;
            return true;
        }
        private bool DrawCmdAreSequentialIdxOffset(Command L, Command R)
        {
            return L.Offset + L.Count == R.Offset;
        }
    }
}
