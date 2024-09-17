
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
        }
    }
}
