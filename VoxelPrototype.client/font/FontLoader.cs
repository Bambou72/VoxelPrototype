using OpenTK.Mathematics;
using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.utils.StbImage;
using VoxelPrototype.client.utils.StbTrueType;
namespace VoxelPrototype.client.font
{
    public unsafe static class FontLoader
    {
        public static Font LoadFromMemory(byte[] Data)
        {
            StbTrueType.stbtt_fontinfo Info = StbTrueType.CreateFont(Data, 0);
            float Scale = StbTrueType.stbtt_ScaleForMappingEmToPixels(Info, 20);
            Font FT = new();
            int Ascent, Descent, LineGap;
            StbTrueType.stbtt_GetFontVMetrics(Info, &Ascent, &Descent, &LineGap);
            FT.Height = 20;
            FT.Ascent = (int)Math.Round(Ascent * Scale);
            FT.Descent = (int)Math.Round(Descent * Scale);
            FT.LineHeight = FT.Ascent - FT.Descent + FT.LineGap;
            FT.LineGap = (int)Math.Round(LineGap * Scale);
            ImageResult IR = GenerateAtlas(FT, Info, Scale, 64);
            Texture FontAtlas = TextureLoader.LoadFromImageResult(IR, GenerateMipmap: false);
            FT.Atlas = FontAtlas;
            return FT;
        }
        public static Font LoadFromFile(string Path)
        {

            StbTrueType.stbtt_fontinfo Info = StbTrueType.CreateFont(File.ReadAllBytes(Path), 0);
            float Scale = StbTrueType.stbtt_ScaleForMappingEmToPixels(Info, 20);
            Font FT = new();
            int Ascent, Descent, LineGap;
            StbTrueType.stbtt_GetFontVMetrics(Info, &Ascent, &Descent, &LineGap);
            FT.Height = 20;
            FT.Ascent = (int)Math.Round(Ascent * Scale);
            FT.Descent = (int)Math.Round(Descent * Scale);
            FT.LineHeight = FT.Ascent - FT.Descent + FT.LineGap;
            FT.LineGap = (int)Math.Round(LineGap * Scale);
            ImageResult IR = GenerateAtlas(FT, Info, Scale, 64);
            Texture FontAtlas = TextureLoader.LoadFromImageResult(IR, GenerateMipmap: false);
            FT.Atlas = FontAtlas;
            return FT;
        }
        const uint MAX_CODEPOINT = 65536;
        public static ImageResult GenerateAtlas(Font Font, StbTrueType.stbtt_fontinfo Info, float Scale, int TextureSize)
        {
            int offsetX = 1, offsetY = 1, MaxHeight = 0;
            byte[] image = new byte[TextureSize * TextureSize * 4];
            image[0] = 255;
            image[1] = 255;
            image[2] = 255;
            image[3] = 255;

            for (int codepoint = 0; codepoint <= MAX_CODEPOINT; codepoint++)
            {
                int GlyphIndex = StbTrueType.stbtt_FindGlyphIndex(Info, codepoint);

                if (GlyphIndex != 0)
                {
                    int Advance, LeftSideBearing, Width, Height, xoff, yoff;
                    StbTrueType.stbtt_GetGlyphHMetrics(Info, GlyphIndex, &Advance, &LeftSideBearing);
                    Advance = (int)Math.Round(Advance * Scale);
                    LeftSideBearing = (int)Math.Round(LeftSideBearing * Scale);
                    byte* Data = StbTrueType.stbtt_GetGlyphBitmap(Info, Scale, Scale, GlyphIndex, &Width, &Height, &xoff, &yoff);
                    int X0, X1, Y0, Y1;
                    StbTrueType.stbtt_GetGlyphBitmapBox(Info, GlyphIndex, Scale, Scale, &X0, &Y0, &X1, &Y1);
                    Character character = new Character
                    {
                        Size = new(Width, Height),
                        Bearing = new(X0, Y1),
                        Advance = (uint)Advance,
                        Rendarable = Width == 0 || Height == 0 ? false : true,
                    };
                    MaxHeight = Math.Max(MaxHeight, Height);
                    if (offsetX + Width > TextureSize)
                    {
                        offsetX = 1;
                        offsetY += MaxHeight;
                        MaxHeight = 0;
                    }
                    if (offsetY + Height > TextureSize)
                    {
                        Font.Characters.Clear();
                        return GenerateAtlas(Font, Info, Scale, TextureSize + 32);
                    }

                    character.UV0 = new Vector2(offsetX, offsetY);
                    character.UV1 = new Vector2(offsetX + Width, offsetY + Height);
                    character.UV0 /= new Vector2(TextureSize);
                    character.UV1 /= new Vector2(TextureSize);
                    Font.Characters.Add((char)codepoint, character);
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int pixelX = offsetX + x;
                            int pixelY = offsetY + y;
                            byte pixelValue = Data[y * character.Size.X + x];
                            int index = (pixelY * TextureSize + pixelX) * 4;
                            image[index] = pixelValue;
                            image[index + 1] = pixelValue;
                            image[index + 2] = pixelValue;
                            image[index + 3] = pixelValue;
                        }
                    }
                    offsetX += Width;
                    StbTrueType.stbtt_FreeBitmap(Data);
                    //Marshal.FreeHGlobal((IntPtr)Data);
                }

            }
            return new ImageResult
            {
                Data = image,
                Width = TextureSize,
                Height = TextureSize,
                Comp = ColorComponents.RedGreenBlueAlpha
            };
        }
    }
}
