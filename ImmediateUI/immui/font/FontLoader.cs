using FreeTypeSharp;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.utils.StbImage;
using static FreeTypeSharp.FT;
namespace ImmediateUI.immui.font
{

    internal unsafe static class FontLoader
    {
        internal static FreeTypeLibrary Lib = new FreeTypeLibrary();
        public static Font LoadFromFile(string Path)
        {
            FT_FaceRec_* face;
            FT_New_Face(Lib.Native, (byte*)Marshal.StringToHGlobalAnsi(Path), 0, &face);
            FT_Set_Pixel_Sizes(face, 0, 50);
            Font FT = new();
            FT.Height = 50;
            ImageResult IR = GenerateAtlas(FT, face, 256);
            Texture FontAtlas = TextureLoader.LoadFromImageResult(IR);
            FT.Atlas = FontAtlas;
            return FT;
        }
        public static Font LoadFromMemory(byte[] Data)
        {
            FT_FaceRec_* face;
            fixed (byte* pData = Data)
            {
                FT_New_Memory_Face(Lib.Native, pData, Data.Length, 0, &face);
            }
            FT_Set_Pixel_Sizes(face, 0, 50);
            Font FT = new();
            FT.Height = 50;

            ImageResult IR = GenerateAtlas(FT, face, 256);
            Texture FontAtlas = TextureLoader.LoadFromImageResult(IR);
            FT.Atlas = FontAtlas;
            return FT;
        }
        public static ImageResult GenerateAtlas(Font Font, FT_FaceRec_* face, int TextureSize)
        {
            int offsetX = 0, offsetY = 0, MaxHeight = 0;
            byte[] image = new byte[TextureSize * TextureSize * 4];
            uint Index;
            uint c = (uint)FT_Get_First_Char(face, &Index);
            while (Index != 0)
            {
                FT_Load_Char(face, c, FT_LOAD.FT_LOAD_RENDER);

                Character character = new Character
                {
                    Size = new((int)face->glyph->bitmap.width, (int)face->glyph->bitmap.rows),
                    Bearing = new(face->glyph->bitmap_left, face->glyph->bitmap_top),
                    Advance = (uint)face->glyph->advance.x,
                    Rendarable = c < 33 ? false : true,
                };
                MaxHeight = Math.Max(MaxHeight, (int)face->glyph->bitmap.rows);
                if (offsetX + face->glyph->bitmap.width > TextureSize)
                {
                    offsetX = 0;
                    offsetY += MaxHeight + 2;
                    MaxHeight = 0;
                }
                if (offsetY + face->glyph->bitmap.rows > TextureSize)
                {
                    Font.Characters.Clear();
                    return GenerateAtlas(Font, face, TextureSize * 2);
                }
                character.UV0 = new Vector2(offsetX, offsetY);
                character.UV1 = new Vector2(offsetX + face->glyph->bitmap.width, offsetY + face->glyph->bitmap.rows);
                // Normalize atlas coordinates
                character.UV0 /= new Vector2(TextureSize);
                character.UV1 /= new Vector2(TextureSize);
                // Store atlas coordinates of the character
                Font.Characters.Add((char)c, character);
                for (int y = 0; y < face->glyph->bitmap.rows; y++)
                {
                    for (int x = 0; x < face->glyph->bitmap.width; x++)
                    {
                        int pixelX = offsetX + x;
                        int pixelY = offsetY + y;
                        byte pixelValue = face->glyph->bitmap.buffer[y * character.Size.X + x];
                        int index = (pixelY * TextureSize + pixelX) * 4;
                        image[index] = pixelValue;
                        image[index + 1] = pixelValue;
                        image[index + 2] = pixelValue;
                        image[index + 3] = pixelValue;
                    }
                }
                offsetX += (int)face->glyph->bitmap.width + 2;
                c = (uint)FT_Get_Next_Char(face, c, &Index);
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
