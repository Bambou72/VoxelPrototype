using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.resources.managers;
using VoxelPrototype.client.utils.StbImage;
using VoxelPrototype.client.utils.StbImageSharp;
using VoxelPrototype.client.Utils;
namespace VoxelPrototype.client.Resources.Managers
{
    internal class TextureManager : IReloadableResourceManager
    {
        Dictionary<string, ITexture> Textures = new Dictionary<string, ITexture>();
        static string BlockAltasResourceID = "engine:block_atlas";
        public void Clean()
        {
            foreach (var texture in Textures.Values)
            {
                texture.Clean();
            }
            Textures.Clear();
        }
        public ITexture GetTexture(string resourceLocation)
        {
            if (Textures.TryGetValue(resourceLocation, out var texture))
            {
                return texture;
            }
            throw new Exception("Can't find requested texture");
        }
        public void Reload(ResourcesManager Manager)
        {
            Clean();
            //UI
            var UITextures = Manager.ListResources("textures/gui", path => path.EndsWith(".png"));
            foreach (var texture in UITextures)
            {
                texture.Value.Open();
                Textures.Add(texture.Key, TextureLoader.LoadFromStream(texture.Value.GetStream()));
                texture.Value.Close();
            }
            //Entity Texture
            var EntitiesTextures = Manager.ListResources("textures/entity", path => path.EndsWith(".png"));
            foreach (var texture in EntitiesTextures)
            {
                texture.Value.Open();
                Textures.Add(texture.Key, TextureLoader.LoadFromStream(texture.Value.GetStream()));
                texture.Value.Close();
            }
            //Block textures
            Dictionary<string, ImageResult> TempLoadedTextures = new();
            var BlockTextures = Manager.ListResources("textures/block", path => path.EndsWith(".png"));
            foreach (var texture in BlockTextures)
            {
                texture.Value.Open();
                StbImage.stbi_set_flip_vertically_on_load(0);
                ImageResult Texture = ImageResult.FromStream(texture.Value.GetStream(), ColorComponents.RedGreenBlueAlpha);
                TempLoadedTextures.Add(texture.Key, Texture);
                texture.Value.Close();
            }
            BuildBlockAtlas(TempLoadedTextures);
            //Font Texture
            var FontTexture = Manager.ListResources("textures/font", path => path.EndsWith(".png"));
            foreach (var texture in FontTexture)
            {
                texture.Value.Open();
                StbImage.stbi_set_flip_vertically_on_load(0);
                ImageResult Texture = ImageResult.FromStream(texture.Value.GetStream(), ColorComponents.RedGreenBlueAlpha);
                Textures.Add(texture.Key,new StoreTexture(Texture));
                texture.Value.Close();
            }
        }
        public void BuildBlockAtlas(Dictionary<string, ImageResult> LoadedTextures)
        {
            int numTextures = LoadedTextures.Count;
            int maxTextureWidth = LoadedTextures.Values.Max(t => t.Width);
            int maxTextureHeight = LoadedTextures.Values.Max(t => t.Height);
            int atlasSize = Math.Max(maxTextureWidth, maxTextureHeight) * (int)Math.Ceiling(Math.Sqrt(numTextures));
            var atlasImage = new byte[atlasSize * atlasSize * 4];
            int currentX = 0;
            int currentY = 0;
            TextureAtlas Atlas = new();
            foreach (string key in LoadedTextures.Keys)
            {
                var texture = LoadedTextures[key];
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
                Atlas.AddCoordinates(key, coordinates);
                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        int index = ((currentY + y) * atlasSize + currentX + x) * 4;

                        for (int c = 0; c < 4; c++)
                        {
                            int lindex = (y * texture.Width + x) * 4 + c;
                            atlasImage[index + c] = texture.Data[lindex];
                        }
                    }
                }
                currentX += maxTextureWidth;
            }
            if (!Directory.Exists("temp/debug/atlas"))
            {
                Directory.CreateDirectory("temp/debug/atlas");
            }
            var AtlasImage = new ImageResult
            {
                Data = atlasImage,
                Width = atlasSize,
                Height = atlasSize,
                Comp = ColorComponents.RedGreenBlueAlpha,
            };

            ImageSaver.SaveAsPNG("temp/debug/atlas/block.png", AtlasImage);
            Atlas.SetTexture(AtlasImage);
            Textures.Add(BlockAltasResourceID, Atlas);
        }
        public ITexture GetBlockAtlasTexture()
        {
            return Textures[BlockAltasResourceID];
        }
    }
}
