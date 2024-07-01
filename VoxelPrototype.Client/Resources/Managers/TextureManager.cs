using StbImageSharp;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Utils;

namespace VoxelPrototype.client.Resources.Managers
{
    internal class TextureManager : IReloadableResourceManager
    {
        Dictionary<ResourceID, ITexture> Textures = new Dictionary<ResourceID, ITexture>();
        public void Clean()
        {
            foreach (var texture in Textures.Values)
            {
                texture.Clean();
            }
            Textures.Clear();
        }
        public ITexture GetTexture(ResourceID resourceID)
        {
            if (Textures.TryGetValue(resourceID, out var texture))
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
                Textures.Add(texture.Key, TextureLoader.LoadFromStream(texture.Value.GetStream()));
                texture.Value.Close();
            }
            //Entity Texture
            var EntitiesTextures = Manager.ListResources("textures/entity", path => path.EndsWith(".png"));
            foreach (var texture in EntitiesTextures)
            {
                Textures.Add(texture.Key, TextureLoader.LoadFromStream(texture.Value.GetStream()));
                texture.Value.Close();
            }
            //Block textures
            Dictionary< ResourceID, ImageResult> TempLoadedTextures = new();
            var BlockTextures = Manager.ListResources("textures/block", path => path.EndsWith(".png"));
            foreach (var texture in BlockTextures)
            {
                StbImage.stbi_set_flip_vertically_on_load(0);
                ImageResult Texture = ImageResult.FromStream(texture.Value.GetStream(), ColorComponents.RedGreenBlueAlpha);
                TempLoadedTextures.Add(texture.Key, Texture);
                texture.Value.Close();
            }
            BuildBlockAtlas(TempLoadedTextures);
        }
        public void BuildBlockAtlas(Dictionary<ResourceID, ImageResult> LoadedTextures)
        {
            int numTextures = LoadedTextures.Count;
            int maxTextureWidth = LoadedTextures.Values.Max(t => t.Width);
            int maxTextureHeight = LoadedTextures.Values.Max(t => t.Height);
            int atlasSize = Math.Max(maxTextureWidth, maxTextureHeight) * (int)Math.Ceiling(Math.Sqrt(numTextures));
            var atlasImage = new byte[atlasSize * atlasSize * 4];
            int currentX = 0;
            int currentY = 0;
            TextureAtlas Atlas = new();
            foreach (ResourceID key in LoadedTextures.Keys)
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
            if (!Directory.Exists("debug/atlas"))
            {
                Directory.CreateDirectory("debug/atlas");
            }
            var AtlasImage = new ImageResult
            {
                Data = atlasImage,
                Width = atlasSize,
                Height = atlasSize,
                Comp = ColorComponents.RedGreenBlueAlpha,
            };

            ImageSaver.SaveAsPNG("debug/atlas/block.png", AtlasImage);
            Atlas.SetTexture(AtlasImage);
            Textures.Add(new ResourceID("voxelprototype", "textures/block/atlas"),Atlas);
        }
    }
}
