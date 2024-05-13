using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
using SharpFont;
using StbImageSharp;
using StbImageWriteSharp;
using VoxelPrototype.client.Utils;
namespace VoxelPrototype.client.Render.Text
{
    public struct Character
    {
        public Vector2i Size;
        public Vector2i Bearing;
        public uint Advance;
        public Vector2 AtlasStart;
        public Vector2 AtlasEnd;
    }
    public class Font
    {
        private Face Face;
        private Dictionary<char, Character> CharacterMap;
        internal float FontSize;
        internal Texture FontAtlas { get; private set; }
        public Font(string FilePath)
        {
            FontSize = 50;
            Library library = new Library();
            Face = new Face(library, FilePath);
            Face.SetPixelSizes(0, (uint)FontSize);
            
            CharacterMap = new Dictionary<char, Character>();
            ImageResult Atlas = GenerateAtlas(256);
            byte[] OpenGLAtlasDATA = new byte[Atlas.Width* Atlas.Height];
            for (int y = 0; y < Atlas.Height; y++)
            {
                for (int x =0; x < Atlas.Width; x++)
                {
                
                    byte pixelValue = Atlas.Data[(y * Atlas.Width + x )*4];
                    int index = (y * Atlas.Width + x);
                    OpenGLAtlasDATA[index] = pixelValue;
                }
            }
            ImageResult OpenGLAtlas = new()
            {
                Data = OpenGLAtlasDATA,
                Width = Atlas.Width,
                Height = Atlas.Height,
                Comp = StbImageSharp.ColorComponents.Grey
            };


            FontAtlas = Texture.LoadFromDataFont(OpenGLAtlas,OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear,false);
            if (!Directory.Exists("debug/atlas/fonts"))
            {
                Directory.CreateDirectory("debug/atlas/fonts");
            }
            ImageSaver.SaveAsPNG("debug/atlas/fonts/" + Path.GetFileNameWithoutExtension(FilePath) + ".png", Atlas);
            
        }
        public Character GetCharacter(char character)
        {
            return CharacterMap[character];
        }
        public List<char> GetCharSet()
        {
            List<char> CharSet = new();
            for (int i = 0;i<256;i++)
            {
                CharSet.Add((char)i);
            }
            return CharSet;
        }
        public ImageResult GenerateAtlas(int TextureSize)
        { 
            int offsetX = 0, offsetY = 0, MaxHeight = 0;
            byte[] image = new byte[TextureSize * TextureSize * 4];
            foreach(char c in GetCharSet())
            {
                if(Face.GetCharIndex(c)!= 0)
                {
                    Face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                    Character character = new Character
                    {
                        Size = new Vector2i(Face.Glyph.Bitmap.Width, Face.Glyph.Bitmap.Rows),
                        Bearing = new Vector2i(Face.Glyph.BitmapLeft, Face.Glyph.BitmapTop),
                        Advance = (uint)(Face.Glyph.Advance.X.Value),
                    };
                    MaxHeight = Math.Max(MaxHeight, Face.Glyph.Bitmap.Rows);
                    if (offsetX + Face.Glyph.Bitmap.Width > TextureSize)
                    {
                        offsetX = 0;
                        offsetY += (int)MaxHeight + 2;
                        MaxHeight = 0;
                    }
                    if (offsetY + Face.Glyph.Bitmap.Rows > TextureSize)
                    {
                        CharacterMap.Clear();
                        return GenerateAtlas(TextureSize * 2);
                    }
                    character.AtlasStart = new Vector2(offsetX, offsetY);
                    character.AtlasEnd = new Vector2(offsetX + Face.Glyph.Bitmap.Width, offsetY + Face.Glyph.Bitmap.Rows);
                    // Normalize atlas coordinates
                    character.AtlasStart /= new Vector2(TextureSize);
                    character.AtlasEnd /= new Vector2(TextureSize);
                    // Store atlas coordinates of the character
                    CharacterMap.Add(c, character);
                    for (int y = 0; y < Face.Glyph.Bitmap.Rows; y++)
                    {
                        for (int x = 0; x < Face.Glyph.Bitmap.Width; x++)
                        {
                            int pixelX = offsetX + x;
                            int pixelY = offsetY + y;
                            byte pixelValue = Face.Glyph.Bitmap.BufferData[y * character.Size.X + x];
                            int index = (pixelY * TextureSize + pixelX) * 4;
                            image[index] = pixelValue;
                            image[index + 1] = pixelValue;
                            image[index + 2] = pixelValue;
                            image[index + 3] = pixelValue;
                        }
                    }
                    offsetX += (int)Face.Glyph.Bitmap.Width + 2;

                }
            }
            return new ImageResult
            {
                Data = image,
                Width = TextureSize,
                Height = TextureSize,
                Comp = StbImageSharp.ColorComponents.RedGreenBlueAlpha
                
            };
        }
        //public Font(string FilePath)
        //{
        //    Library library = new Library();
        //    Face = new Face(library, FilePath);
        //    Face.SetPixelSizes(0, 50);
        //    FontSize = 50;
        //    CharacterMap = new Dictionary<char, Character>();
        //    Image<Rgba32> Atlas = GenerateAtlas(1024);
        //    FontAtlas = Texture.LoadFromData(Atlas, OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear);
        //    if (!Directory.Exists("debug/atlas/fonts"))
        //    {
        //        Directory.CreateDirectory("debug/atlas/fonts");
        //    }
        //    Atlas.Save("debug/atlas/fonts/" + Path.GetFileNameWithoutExtension(FilePath) + ".png");
        //}
        //public Character GetCharacter(char character)
        //{
        //    return CharacterMap[character];
        //}

        //public Image<Rgba32> GenerateAtlas(int TextureSize)
        //{
        //    int offsetX = 0, offsetY = 0, MaxHeight = 0;
        //    Image<Rgba32> image = new(TextureSize, TextureSize);
        //    uint GIndex = 1;
        //    Face.GetFirstChar(out uint charcode);
        //    while (GIndex != 0)
        //    {
        //        Face.LoadChar(charcode, LoadFlags.Render, LoadTarget.Normal);
        //        Character character = new Character
        //        {
        //            Size = new Vector2i(Face.Glyph.Bitmap.Width, Face.Glyph.Bitmap.Rows),
        //            Bearing = new Vector2i(Face.Glyph.BitmapLeft, Face.Glyph.BitmapTop),
        //            Advance = (uint)(Face.Glyph.Advance.X.Value),
        //        };
        //        MaxHeight = Math.Max(MaxHeight, Face.Glyph.Bitmap.Rows);
        //        if (offsetX + Face.Glyph.Bitmap.Width > TextureSize)
        //        {
        //            offsetX = 0;
        //            offsetY += (int)MaxHeight + 2;
        //            MaxHeight = 0;
        //        }
        //        if (offsetY + Face.Glyph.Bitmap.Rows > TextureSize)
        //        {
        //            CharacterMap.Clear();
        //            return GenerateAtlas(TextureSize * 2);
        //        }
        //        character.AtlasStart = new Vector2(offsetX, offsetY);
        //        character.AtlasEnd = new Vector2(offsetX + Face.Glyph.Bitmap.Width, offsetY + Face.Glyph.Bitmap.Rows);
        //        // Normalize atlas coordinates
        //        character.AtlasStart /= new Vector2(TextureSize);
        //        character.AtlasEnd /= new Vector2(TextureSize);
        //        // Store atlas coordinates of the character
        //        CharacterMap.Add((char)charcode, character);
        //        for (int y = 0; y < Face.Glyph.Bitmap.Rows; y++)
        //        {
        //            for (int x = 0; x < Face.Glyph.Bitmap.Width; x++)
        //            {
        //                int pixelX = offsetX + x;
        //                int pixelY = offsetY + y;
        //                byte pixelValue = Face.Glyph.Bitmap.BufferData[y * character.Size.X + x];
        //                image[pixelX, pixelY] = new Rgba32(pixelValue, pixelValue, pixelValue, pixelValue);
        //            }
        //        }
        //        offsetX += (int)Face.Glyph.Bitmap.Width + 2;
        //        charcode = Face.GetNextChar(charcode, out GIndex);
        //    }
        //    return image;
        //}
    }
}
