using ImmediateUI.immui.drawing;
using ImmediateUI.immui.math;
using OpenTK.Mathematics;
using VoxelPrototype.client.rendering.texture;
namespace ImmediateUI.immui.font
{
    public class Font
    {
        internal Dictionary<char, Character> Characters = new();
        internal ITexture Atlas;
        internal int Height;
        public void RenderText(ImmuiDrawList DrawList, float Size, Vector2 Position,uint Color,Rect ClipRect, string Text,float WrapWidth, bool CpuFineClip)
        {
            var VertexBuffer = DrawList.VertexBuffer;
            var IndexBuffer = DrawList.IndexBuffer;
            int textEnd = Text.Length;
            int textBegin = 0;
            float X = MathF.Truncate(Position.X);
            float Y = MathF.Truncate(Position.Y);
            if(Y > ClipRect.YH)
            {
                return;
            }
            
            float start_x = X;
            float scale = Size / Height;
            float line_height = Height * scale;
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
                        s = CalcWordWrapPositionA(scale,Text, s, lineEnd !=0 ? lineEnd : textEnd, WrapWidth);
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
            int idxExpectedSize = DrawList.IndexBuffer.Size + idxCountMax;
            DrawList.PrimReserve(idxCountMax, vtxCountMax);
            uint colUntinted = Color | ~ImmuiDrawList.AlphaMask;
            int wordWrapEol = 0;
            while (s < textEnd)
            {
                if (word_wrap_enabled)
                {
                    if (wordWrapEol == 0)
                    {
                        wordWrapEol = CalcWordWrapPositionA(scale, Text ,s,textEnd, WrapWidth - (X - start_x));
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
                Character glyph = Characters[c];
                if (glyph == null)
                {
                    continue;
                }
                float charWidth = (glyph.Advance >> 6) * scale;
                if (glyph.Rendarable)
                {
                    float x1 = X + glyph.Bearing.X * scale;
                    float x2 = x1 + glyph.Size.X * scale;
                    float y1 = Y - ( glyph.Bearing.Y )*scale;
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
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx);
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx + 1);
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx + 2);
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx);
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx + 2);
                        IndexBuffer.PushBack(DrawList.VtxCurrentIdx + 3);
                        DrawList.VtxCurrentIdx += 4;
                    }
                }
                X += charWidth;
            }

           ImmuiDrawCommand DCmd = DrawList.Commands[^1];
           DCmd.Count -= idxExpectedSize -  DrawList.IndexBuffer.Size;
           DrawList.Commands[^1] = DCmd;
        }
        public Vector2 CalcTextSize(float size, string Text, float wrap_width, float max_width = -1) 
        {

            float line_height = 0;
            float scale = size / Height;

            Vector2 text_size =Vector2.Zero;
            float line_width = 0.0f;

            bool word_wrap_enabled = (wrap_width > 0.0f);
            int word_wrap_eol = 0;

            int s = 0;
            while (s < Text.Length)
            {
                if (word_wrap_enabled)
                {
                    if (word_wrap_eol == 0)
                    {
                        word_wrap_eol = CalcWordWrapPositionA(scale, Text, s, Text.Length, wrap_width - line_width);
                    }

                    if (s >= word_wrap_eol)
                    {
                        if (text_size.X < line_width)
                        {
                            text_size.X = line_width;
                        }
                        text_size.Y += line_height;
                        line_width = 0.0f;
                        word_wrap_eol = 0;
                        s = CalcWordWrapNextLineStartA(Text, s, Text.Length);
                        continue;
                    }
                }


                int prev_s = s;
                char c = Text[s];
                s++;
                if (c < 32)
                {
                    if (c == '\n')
                    {
                        text_size.X = Math.Min(text_size.X, line_width);
                        text_size.Y += line_height;
                        line_width = 0.0f;
                        continue;
                    }
                    if (c == '\r')
                        continue;
                }
                Character glyph = Characters[c];
                float char_width = (glyph.Advance >> 6) * scale;
                if (max_width != -1 && line_width + char_width >= max_width)
                {
                    s = prev_s;
                    break;
                }

                line_width += char_width;
                line_height = MathF.Max(line_height, glyph.Size.Y * scale);
            }
            if (text_size.X < line_width)
            {
                text_size.X = line_width;
            }
            if (line_width > 0 || text_size.Y == 0.0f)
            {
                text_size.Y += line_height;
            }
            

            return text_size;
        }
        public int CalcWordWrapPositionA(float Scale,string TextString, int Text,int TextEnd, float WrapWidth)
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
                float char_width = (Characters[c].Advance>>6 );
                if (IsCharBlank(c))
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
        static int CalcWordWrapNextLineStartA(string StringText ,int Text, int TextEnd)
        {
            while (Text < TextEnd && IsCharBlank(StringText[Text]))
            {
                Text++;
            }
            if (StringText[Text] == '\n')
            {
                Text++;
            }
            return Text;
        }
        static bool IsCharBlank(char c)
        {
            return c == ' ' || c == '\t' || c == 0x3000;
        }
    }
    public class Character
    {
        internal Vector2i Size;
        internal Vector2i Bearing;
        internal uint Advance;
        internal Vector2 UV0;
        internal Vector2 UV1;
        internal bool Rendarable = true;
    }
}
