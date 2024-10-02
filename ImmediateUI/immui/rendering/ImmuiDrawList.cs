using OpenTK.Mathematics;
using ImmediateUI.immui.font;
using ImmediateUI.immui.math;
using ImmediateUI.immui.utils;
namespace ImmediateUI.immui.rendering
{
    public class ImmuiDrawList
    {
        internal const uint AlphaMask = 0x000000FF;
        public List<ImmuiDrawCommand> Commands = new();
        public Vector<Vertex> VertexBuffer = new();
        public Vector<uint> IndexBuffer = new();
        internal uint VtxCurrentIdx;
        public List<Rect> ClipStack = new();
        public List<int> TextureStack = new();
        public ImmuiDrawCommandHeader CmdHeader = new();
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
            bool push_texture_id = Texture != CmdHeader.TextureID;
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
        public void AddText(Vector2 Position, string Text, float Scale, uint Color, Font Font = null, float WrapWidth = 0, Rect CpuClip = default)
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
            Rect clip_rect = CmdHeader.ClipRect;
            if (!CpuClip.Equals(default))
            {
                clip_rect.X = Math.Max(clip_rect.X, CpuClip.X);
                clip_rect.Y = Math.Max(clip_rect.Y, CpuClip.Y);
                clip_rect.W = Math.Min(clip_rect.XW, CpuClip.XW) - clip_rect.X;
                clip_rect.H = Math.Min(clip_rect.YH, CpuClip.YH) - clip_rect.Y;
            }
            CmdHeader.ClipRect = clip_rect;
            PushTextureID(Font.Atlas.GetHandle());
            RenderText(Font, Scale, Position, Color, clip_rect, Text, WrapWidth, !CpuClip.Equals(default));
            PopTextureID();
        }
        public void RenderText(Font font, float Size, Vector2 Position, uint Color, Rect ClipRect, string Text, float WrapWidth, bool CpuFineClip)
        {
            int textEnd = Text.Length;
            int textBegin = 0;
            float X = MathF.Truncate(Position.X);
            float Y = MathF.Truncate(Position.Y);
            if (Y > ClipRect.YH)
            {
                return;
            }

            float start_x = X;
            float scale = Size / font.Height;
            float line_height = font.Height * scale;
            bool word_wrap_enabled = WrapWidth > 0.0f;
            int s = textBegin;
            if (Y + line_height < ClipRect.Y)
            {
                while (Y + line_height < ClipRect.Y && s < textEnd)
                {
                    int lineEnd = Text.IndexOf('\n', s);
                    if (lineEnd == -1)
                    {
                        lineEnd = textEnd;
                    }

                    if (word_wrap_enabled)
                    {
                        s = CalcWordWrapPositionA(font,scale, Text, s, lineEnd != 0 ? lineEnd : textEnd, WrapWidth);
                        s = CalcWordWrapNextLineStartA(Text, s, textEnd);
                    }
                    else
                    {
                        s = lineEnd + 1;
                    }
                    Y += line_height;
                }
            }

            if (textEnd - s > 10000 && !word_wrap_enabled)
            {
                int sEnd = s;
                float yEnd = Y;
                while (yEnd < ClipRect.YH && sEnd < textEnd)
                {
                    sEnd = Text.IndexOf('\n', sEnd);
                    if (sEnd == -1) sEnd = textEnd;
                    else sEnd += 1;

                    yEnd += line_height;
                }
                textEnd = sEnd;
            }
            if (s == textEnd)
            {
                return;
            }
            int vtxCountMax = (textEnd - s) * 4;
            int idxCountMax = (textEnd - s) * 6;
            int idxExpectedSize = IndexBuffer.Size + idxCountMax;
            var Command = Commands[Commands.Count - 1];
            Command.Count += idxCountMax;
            Commands[Commands.Count - 1] = Command;

            uint colUntinted = Color | ~ImmuiDrawList.AlphaMask;
            int wordWrapEol = 0;
            while (s < textEnd)
            {
                if (word_wrap_enabled)
                {
                    if (wordWrapEol == 0)
                    {
                        wordWrapEol = CalcWordWrapPositionA(font,scale, Text, s, textEnd, WrapWidth - (X - start_x));
                    }
                    if (s >= wordWrapEol)
                    {
                        X = start_x;
                        Y += line_height;
                        if (Y > ClipRect.YH)
                        {
                            break;
                        }
                        wordWrapEol = 0;
                        s = CalcWordWrapNextLineStartA(Text, s, textEnd);
                        continue;
                    }
                }
                char c = Text[s];
                s++;
                if (c == '\n')
                {
                    X = start_x;
                    Y += line_height;
                    if (Y > ClipRect.YH)
                    {
                        break;
                    }
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
                if (glyph.Rendarable)
                {
                    float x1 = X + glyph.Bearing.X * scale;
                    float x2 = x1 + glyph.Size.X * scale;
                    float y1 = Y - (glyph.Bearing.Y) * scale;
                    float y2 = y1 + glyph.Size.Y * scale;
                    if (x1 <= ClipRect.XW && x2 >= ClipRect.X)
                    {
                        float u1 = glyph.UV0.X;
                        float v1 = glyph.UV0.Y;
                        float u2 = glyph.UV1.X;
                        float v2 = glyph.UV1.Y;
                        if (CpuFineClip)
                        {
                            if (x1 < ClipRect.X)
                            {
                                u1 += (1.0f - (x2 - ClipRect.X) / (x2 - x1)) * (u2 - u1);
                                x1 = ClipRect.X;
                            }
                            if (y1 < ClipRect.Y)
                            {
                                v1 += (1.0f - (y2 - ClipRect.Y) / (y2 - y1)) * (v2 - v1);
                                y1 = ClipRect.Y;
                            }
                            if (x2 > ClipRect.XW)
                            {
                                u2 = u1 + ((ClipRect.XW - x1) / (x2 - x1)) * (u2 - u1);
                                x2 = ClipRect.XW;
                            }
                            if (y2 > ClipRect.YH)
                            {
                                v2 = v1 + ((ClipRect.YH - y1) / (y2 - y1)) * (v2 - v1);
                                y2 = ClipRect.YH;
                            }
                            if (y1 >= y2)
                            {
                                X += charWidth;
                                continue;
                            }
                        }

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
                }
                X += charWidth;
            }

            ImmuiDrawCommand DCmd = Commands[^1];
            DCmd.Count -= idxExpectedSize - IndexBuffer.Size;
            Commands[^1] = DCmd;
        }
        public int CalcWordWrapPositionA(Font font, float Scale, string TextString, int Text, int TextEnd, float WrapWidth)
        {
            float line_width = 0.0f;
            float word_width = 0.0f;
            float blank_width = 0.0f;
            WrapWidth /= Scale;
            int word_end = Text;
            int prev_word_end = 0;
            bool inside_word = true;
            int s = Text;
            while (s < TextEnd)
            {
                char c = TextString[s];
                int next_s = s + 1;
                if (c < 32)
                {
                    if (c == '\n')
                    {
                        line_width = word_width = blank_width = 0.0f;
                        inside_word = true;
                        s = next_s;
                        continue;
                    }
                    if (c == '\r')
                    {
                        s = next_s;
                        continue;
                    }
                }
                //TODO : Make a better and more secure way to get character
                float char_width = (font.Characters[c].Advance >> 6);
                if (Font.IsCharBlank(c))
                {
                    if (inside_word)
                    {
                        line_width += blank_width;
                        blank_width = 0.0f;
                        word_end = s;
                    }
                    blank_width += char_width;
                    inside_word = false;
                }
                else
                {
                    word_width += char_width;
                    if (inside_word)
                    {
                        word_end = next_s;
                    }
                    else
                    {
                        prev_word_end = word_end;
                        line_width += word_width + blank_width;
                        word_width = blank_width = 0.0f;
                    }

                    inside_word = (c != '.' && c != ',' && c != ';' && c != '!' && c != '?' && c != '\"');
                }

                if (line_width + word_width > WrapWidth)
                {
                    if (word_width < WrapWidth)
                        s = prev_word_end != 0 ? prev_word_end : word_end;
                    break;
                }

                s = next_s;
            }
            if (s == Text && Text < TextEnd)
            {
                return s + 1;
            }
            return s;
        }
        static int CalcWordWrapNextLineStartA(string StringText, int Text, int TextEnd)
        {
            while (Text < TextEnd && Font.IsCharBlank(StringText[Text]))
            {
                Text++;
            }
            if (StringText[Text] == '\n')
            {
                Text++;
            }
            return Text;
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
        public void PushClipRect(Rect ClipRegion, bool IntersectWithCurrentClip = false)
        {
            if (IntersectWithCurrentClip)
            {
                Rect Current = CmdHeader.ClipRect;
                if (ClipRegion.X < Current.X) ClipRegion.X = Current.X;
                if (ClipRegion.Y < Current.Y) ClipRegion.Y = Current.Y;
                if (ClipRegion.XW > Current.XW) ClipRegion.W = Current.W;
                if (ClipRegion.YH > Current.YH) ClipRegion.H = Current.H;
            }
            ClipRegion.SetMaxX(Math.Max(ClipRegion.X, ClipRegion.XW));
            ClipRegion.SetMaxY(Math.Max(ClipRegion.Y, ClipRegion.YH));
            ClipStack.Add(ClipRegion);
            CmdHeader.ClipRect = ClipRegion;
            OnChangedClipRect();
        }
        public void PushClipRectFullScreen(Vector2 ScreenSize)
        {
            PushClipRect(new(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y));
        }
        public void PopClipRect()
        {
            ClipStack.RemoveAt(ClipStack.Count - 1);
            //CmdHeader.ClipRect = ClipStack.Count == 0 ? new(0,0, (int)Immui.GetScreenSize().X, (int)Immui.GetScreenSize().Y) : ClipStack[ClipStack.Count - 1];
            OnChangedClipRect();
        }
        public void PushTextureID(int TextureID)
        {
            TextureStack.Add(TextureID);
            CmdHeader.TextureID = TextureID;
            OnChangedTextureID();
        }

        public void PopTextureID()
        {
            TextureStack.RemoveAt(TextureStack.Count - 1);
            CmdHeader.TextureID = TextureStack.Count == 0 ? 0 : TextureStack[TextureStack.Count - 1];
            OnChangedTextureID();
        }
        void SetTextureID(int TextureID)
        {
            if (CmdHeader.TextureID == TextureID)
                return;
            CmdHeader.TextureID = TextureID;
            OnChangedTextureID();
        }

        //
        //Helpers
        //
        internal void ResetForNewFrame()
        {
            Commands.Clear();
            IndexBuffer.Size = 0;
            VertexBuffer.Size = 0;
            CmdHeader = new();
            VtxCurrentIdx = 0;
            ClipStack.Clear();
            TextureStack.Clear();
            Commands.Add(new ImmuiDrawCommand());
        }
        internal void OnChangedClipRect()
        {
            ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
            if (curr_cmd.Count != 0 && curr_cmd.ClipRect.Equals(CmdHeader.ClipRect))
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 2)
            {
                ImmuiDrawCommand prev_cmd = Commands[Commands.Count - 2];
                if (curr_cmd.Count == 0 && Commands.Count > 1 && CmdHeader.Equals(prev_cmd) == false && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }
            }
            curr_cmd.ClipRect = CmdHeader.ClipRect;
            Commands[Commands.Count - 1] = curr_cmd;
        }
        internal void OnChangedTextureID()
        {
            ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
            if (curr_cmd.Count != 0 && curr_cmd.TextureID != CmdHeader.TextureID)
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 2)
            {

                ImmuiDrawCommand prev_cmd = Commands[Commands.Count - 2];
                if (curr_cmd.Count == 0 && Commands.Count > 1 && !CmdHeader.Equals(prev_cmd) && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }
            }
            curr_cmd.TextureID = CmdHeader.TextureID;
            Commands[Commands.Count - 1] = curr_cmd;
        }
        internal void AddDrawCmd()
        {
            ImmuiDrawCommand draw_cmd = new();
            draw_cmd.ClipRect = CmdHeader.ClipRect;
            draw_cmd.TextureID = CmdHeader.TextureID;
            draw_cmd.Offset = IndexBuffer.Size;
            Commands.Add(draw_cmd);
        }

        private static bool DrawCmdAreSequentialIdxOffset(ImmuiDrawCommand L, ImmuiDrawCommand R)
        {
            return L.Offset + L.Count == R.Offset;
        }
    }
}
