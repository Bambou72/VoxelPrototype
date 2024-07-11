using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ModelEditor.ModelEditor
{
    internal static class TextureManager
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        internal static Texture VoxelAtlas;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        private static Dictionary<string, float[]> BlockAtlasTexture = new Dictionary<string, float[]>();
        public static float[] GetAtlasTextures(string Name)
        {
            if (BlockAtlasTexture.TryGetValue(Name, out var Texture))
            {
                return Texture;
            }
            else
            {
                return BlockAtlasTexture["unknow"];
            }
        }
        internal static void LoadTextures()
        {
            BlockAtlasTexture.Clear();
            List<Image<Rgba32>> loadedTextures = new List<Image<Rgba32>>();
            List<string> textureNames = new List<string>();
            ///string[] blockTextures = Directory.GetFiles("textures/", "*.png");
            /*foreach (string filePath in blockTextures)
            {
                if (Path.GetFileNameWithoutExtension(filePath) != "atlas")
                {
                    using var stream = File.OpenRead(filePath);
                    var texture = Image.Load<Rgba32>(stream);
                    loadedTextures.Add(texture);
                    textureNames.Add(Path.GetFileNameWithoutExtension(filePath));
                }
            }*/
            int numTextures = loadedTextures.Count;
            int maxTextureWidth = loadedTextures.Max(t => t.Width);
            int maxTextureHeight = loadedTextures.Max(t => t.Height);
            int atlasSize = Math.Max(maxTextureWidth, maxTextureHeight) * (int)Math.Ceiling(Math.Sqrt(numTextures));
            var atlasImage = new Image<Rgba32>(atlasSize, atlasSize);
            int currentX = 0;
            int currentY = 0;
            for (int i = 0; i < numTextures; i++)
            {
                var texture = loadedTextures[i];
                if (currentX + texture.Width > atlasSize)
                {
                    currentX = 0;
                    currentY += maxTextureHeight;
                }
                // Create texture coordinates for the current texture
                float left = (float)currentX / atlasSize;
                float right = (float)(currentX + texture.Width) / atlasSize;
                float top = (float)currentY / atlasSize;
                float bottom = (float)(currentY + texture.Height) / atlasSize;
                float[] coordinates = new float[8];
                coordinates[0] = left;
                coordinates[1] = bottom;
                coordinates[2] = left;
                coordinates[3] = top;
                coordinates[4] = right;
                coordinates[5] = top;
                coordinates[6] = right;
                coordinates[7] = bottom;
                // Add texture coordinates to the dictionary with the file name as the key
                BlockAtlasTexture.Add(textureNames[i], coordinates);
                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        var pixel = texture[x, y];
                        atlasImage[currentX + x, currentY + y] = pixel;
                    }
                }
                currentX += maxTextureWidth;
            }
            VoxelAtlas = Texture.LoadFromData(atlasImage);
        }
    }
}
