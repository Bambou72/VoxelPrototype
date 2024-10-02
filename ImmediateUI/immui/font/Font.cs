using ImmediateUI.immui.math;
using ImmediateUI.immui.rendering;
using OpenTK.Mathematics;
using VoxelPrototype.client.rendering.texture;
namespace ImmediateUI.immui.font
{
    public class Font
    {
        internal Dictionary<char, Character> Characters = new();
        internal ITexture Atlas;
        internal int Height;
        static public bool IsCharBlank(char c)
        {
            return c == ' ' || c == '\t' || c == 0x3000;
        }
        public Vector2 CalcTextSize( float size, string Text, float wrap_width, float max_width = -1)
        {

            float line_height = 0;
            float scale = size / Height;

            Vector2 text_size = Vector2.Zero;
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
                        word_wrap_eol = CalcWordWrapPositionA( scale, Text, s, Text.Length, wrap_width - line_width);
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
        public int CalcWordWrapPositionA(float Scale, string TextString, int Text, int TextEnd, float WrapWidth)
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
                float char_width = (Characters[c].Advance >> 6);
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
