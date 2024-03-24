using Newtonsoft.Json;
namespace ModelEditor.ModelEditor.Block
{
    internal static class BlockDeserializer
    {
        internal static List<Face> Deserialize(string path)
        {
            string json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<BlockModelData>(json);
            List<Face> result = new List<Face>();
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.
            for (int i = 0; i < data.Vertex.Length; i++)
            {
                Face face = new Face();
                for (int j = 0; j < 4; j++)
                {
                    face.data[j] = new System.Numerics.Vector3(data.Vertex[i][j * 3], data.Vertex[i][j * 3 + 1], data.Vertex[i][j * 3 + 2]);
                    face.uv[j] = new System.Numerics.Vector2(data.Uv[i][j * 2], data.Uv[i][j * 2 + 1]);
                }
                result.Add(face);
            }
#pragma warning restore CS8602 // Déréférencement d'une éventuelle référence null.
            return result;
        }
        internal static void Serialize(List<Face> data, string Name, string path)
        {
            var Data = new BlockModelData();
            Data.Name = Name;
            Data.Vertex = new float[data.Count][];
            Data.Uv = new float[data.Count][];
            for (int i = 0; i < data.Count; i++)
            {
                Data.Vertex[i] = new float[12];
                for (int j = 0; j < 4; j++)
                {
                    Data.Vertex[i][j * 3] = data[i].data[j].X;
                    Data.Vertex[i][j * 3 + 1] = data[i].data[j].Y;
                    Data.Vertex[i][j * 3 + 2] = data[i].data[j].Z;
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                Data.Uv[i] = new float[8];
                for (int j = 0; j < 4; j++)
                {
                    Data.Uv[i][j * 2] = data[i].uv[j].X;
                    Data.Uv[i][j * 2 + 1] = data[i].uv[j].Y;
                }
            }
            string json = JsonConvert.SerializeObject(Data);
            File.WriteAllText(path, json);
        }
        internal static void SerializeTextures(List<Face> data, string Name, string ModelName, string path)
        {
            var Data = new ModelTextures();
            Data.Name = Name;
            Data.ModelName = ModelName;
            Data.TexturesList = new string[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                Data.TexturesList[i] = data[i].texture;
            }
            string json = JsonConvert.SerializeObject(Data);
            File.WriteAllText(path, json);
        }
    }
    class BlockModelData
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public float[][] Vertex { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public float[][] Uv { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    }
    public class ModelTextures
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public string Name;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public string ModelName;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public string[] TexturesList;
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
    }
}
