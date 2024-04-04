using OpenTK.Mathematics;
using SharpFont;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.Components;

namespace VoxelPrototype.client.Text
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
        public Font(string FilePath, uint FontSize)
        {
            this.FontSize = FontSize;
            Library library = new Library();
            Face = new Face(library, FilePath);
            Face.SetPixelSizes(0, FontSize);
            CharacterMap = new Dictionary<char, Character>();
            Image<Rgba32> Atlas = GenerateAtlas();
            FontAtlas = Texture.LoadFromData(Atlas);
            if (!Directory.Exists("debug/atlas/fonts"))
            {
                Directory.CreateDirectory("debug/atlas/fonts");
            }
            Atlas.Save("debug/atlas/fonts/"+Path.GetFileNameWithoutExtension(FilePath)+".png");
        }
        public Character GetCharacter(char character)
        {
            return CharacterMap[character];
        }

        public Image<Rgba32> GenerateAtlas()
        {
            int offsetX = 0;
            int offsetY = 0;
            Image<Rgba32> image = new Image<Rgba32>(8192, 128); // Initialize with a large size

            for (char c = char.MinValue; c < 256; c++)
            {
                Face.LoadChar(c, LoadFlags.Render,LoadTarget.Normal);

                Character character = new Character
                {
                    Size = new Vector2i(Face.Glyph.Bitmap.Width, Face.Glyph.Bitmap.Rows),
                    Bearing = new Vector2i(Face.Glyph.BitmapLeft, Face.Glyph.BitmapTop),
                    Advance = (uint)(Face.Glyph.Advance.X.Value),
                    AtlasStart = new Vector2(offsetX, offsetY),
                    AtlasEnd = new Vector2(offsetX + Face.Glyph.Bitmap.Width, offsetY + Face.Glyph.Bitmap.Rows)

                };
                // Normalize atlas coordinates
                character.AtlasStart /= new Vector2(image.Width, image.Height);
                character.AtlasEnd /= new Vector2(image.Width, image.Height);
                // Store atlas coordinates of the character

                CharacterMap.Add(c, character);

                // Copy bitmap data to the atlas image
                for (int y = 0; y < character.Size.Y; y++)
                {
                    for (int x = 0; x < character.Size.X; x++)
                    {
                        int pixelX = offsetX + x;
                        int pixelY = offsetY + y;
                        byte pixelValue = Face.Glyph.Bitmap.BufferData[y * character.Size.X + x];
                        image[pixelX, pixelY] = new Rgba32(pixelValue, pixelValue, pixelValue,pixelValue);
                    }
                }

                // Update offset for the next character
                offsetX += Face.Glyph.Bitmap.Width + 1; // Add some padding between characters
            }

            return image;
        }
    }
}
