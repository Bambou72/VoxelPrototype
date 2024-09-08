using ImmediateUI.immui.drawing;
using ImmediateUI.immui.math;
using ImmediateUI.immui.utils;
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
            int textEnd = Text.Length;
            int textBegin = 0;
            float X = MathF.Truncate(Position.X);
            float Y = MathF.Truncate(Position.Y);
            if(Y > ClipRect.Max.Y)
            {
                return;
            }
            
            float start_x = X;
            float scale = Size / Height;
            float line_height = Height * scale;
            bool word_wrap_enabled = WrapWidth > 0.0f;
            int s = textBegin;
            if (Y + line_height < ClipRect.Min.Y)
            {
                while (Y + line_height < ClipRect.Min.Y && s < textEnd)
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
                while (yEnd < ClipRect.Max.Y && sEnd < textEnd)
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
            int idxExpectedSize = DrawList.IndexBuffer.Count + idxCountMax;

           DrawList.PrimReserve(idxCountMax, vtxCountMax);
           
           uint colUntinted = Color | ~Immui.AlphaMask;
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
                       if (Y > ClipRect.Max.Y)
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
                   if (Y > ClipRect.Max.Y)
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
                   float y1 = Y - glyph.Bearing.Y *scale;
                   float y2 = y1 + glyph.Size.Y * scale;
                   if (x1 <= ClipRect.Max.X && x2 >= ClipRect.Min.X)
                   {
                       float u1 = glyph.UV0.X;
                       float v1 = glyph.UV0.Y;
                       float u2 = glyph.UV1.X;
                       float v2 = glyph.UV1.Y;
                       if (CpuFineClip)
                       {
                           if (x1 < ClipRect.Min.X)
                           {
                               u1 += (1.0f - (x2 - ClipRect.Min.X) / (x2 - x1)) * (u2 - u1);
                               x1 = ClipRect.Min.X;
                           }
                           if (y1 < ClipRect.Min.Y)
                           {
                               v1 += (1.0f - (y2 - ClipRect.Min.Y) / (y2 - y1)) * (v2 - v1);
                               y1 = ClipRect.Min.Y;
                           }
                           if (x2 > ClipRect.Max.X)
                           {
                               u2 = u1 + ((ClipRect.Max.X - x1) / (x2 - x1)) * (u2 - u1);
                               x2 = ClipRect.Max.X;
                           }
                           if (y2 > ClipRect.Max.Y)
                           {
                               v2 = v1 + ((ClipRect.Max.Y - y1) / (y2 - y1)) * (v2 - v1);
                               y2 = ClipRect.Max.Y;
                           }
                           if (y1 >= y2)
                           {
                               X += charWidth;
                               continue;
                           }
                       }

                       DrawList.VertexBuffer[DrawList.VertexPtr] = new Vertex { Position = new Vector2(x1, y1), Color = Color, UV = new Vector2(u1, v1) };
                       DrawList.VertexBuffer[DrawList.VertexPtr + 1] = new Vertex { Position = new Vector2(x2, y1), Color = Color, UV = new Vector2(u2, v1) };
                       DrawList.VertexBuffer[DrawList.VertexPtr + 2] = new Vertex { Position = new Vector2(x2, y2), Color = Color, UV = new Vector2(u2, v2) };
                       DrawList.VertexBuffer[DrawList.VertexPtr + 3] = new Vertex { Position = new Vector2(x1, y2), Color = Color, UV = new Vector2(u1, v2) };
                       DrawList.IndexBuffer[DrawList.IndexPtr] = DrawList.VtxCurrentIdx;
                       DrawList.IndexBuffer[DrawList.IndexPtr + 1] = DrawList.VtxCurrentIdx + 1;
                       DrawList.IndexBuffer[DrawList.IndexPtr + 2] = DrawList.VtxCurrentIdx + 2;
                       DrawList.IndexBuffer[DrawList.IndexPtr + 3] = DrawList.VtxCurrentIdx;
                       DrawList.IndexBuffer[DrawList.IndexPtr + 4] = DrawList.VtxCurrentIdx + 2;
                       DrawList.IndexBuffer[DrawList.IndexPtr + 5] = DrawList.VtxCurrentIdx + 3;
                       DrawList.VertexPtr += 4;
                       DrawList.VtxCurrentIdx += 4;
                       DrawList.IndexPtr += 6;
                   }
               }
               X += charWidth;
           }
           //DrawList.VertexBuffer.Resize(DrawList.VertexBuffer.Count - (DrawList.VertexBuffer.Count -DrawList.VertexPtr));
           //DrawList.IndexBuffer.Resize(DrawList.IndexBuffer.Count - (DrawList.IndexBuffer.Count - DrawList.IndexPtr));
           ImmuiDrawCommand DCmd = DrawList.Commands[^1];
           DCmd.Count -= idxExpectedSize -  DrawList.IndexBuffer.Count;
           DrawList.Commands[^1] = DCmd;
        }
        //WARN : Method broken
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
