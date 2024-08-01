using System.Text.Json;
using VoxelPrototype.client.rendering.model;
using VoxelPrototype.utils;

namespace VoxelPrototype.client.Resources.Managers
{
    internal class ModelManager : IReloadableResourceManager
    {
        //
        private Dictionary<string, BlockMesh> BlockMeshs = new Dictionary<string, BlockMesh>();
        private  Dictionary<string, Model> EntitiesMesh = new Dictionary<string, Model>();

        public BlockMesh GetBlockMesh(string resourceLocation)
        {
            if(BlockMeshs.TryGetValue(resourceLocation, out BlockMesh blockMesh))
            {
                return blockMesh;
            }
            throw new Exception("Can't find block mesh");
        }
        public Model GetEntityModel(string resourceLocation)
        {
            if (EntitiesMesh.TryGetValue(resourceLocation, out Model blockMesh))
            {
                return blockMesh;
            }
            throw new Exception("Can't find entity model");
        }
        public void Clean()
        {
            BlockMeshs.Clear();
            foreach (var mesh in EntitiesMesh.Values)
            {
                mesh.Clean();
            }
            EntitiesMesh.Clear();
        }
        public void Reload(ResourcesManager Manager)
        {
            Clean();
            //Block Mesh
            var BlocksMesh = Manager.ListResources("models/block", path => path.EndsWith(".json"));
            foreach (var mesh in BlocksMesh)
            {
                mesh.Value.Open();
                TextReader TempTextReader = new StreamReader(mesh.Value.GetStream());
                var data = JsonSerializer.Deserialize<BlockMeshData>(TempTextReader.ReadToEnd());
                mesh.Value.Close();

                BlockMeshs.Add(mesh.Key, new BlockMesh(data.Vertex, data.Uv));
            }
            //Entities Mesh
            var EntitiesMeshs = Manager.ListResources("models/entity", path => path.EndsWith(".json"));
            foreach (var mesh in EntitiesMeshs)
            {
                mesh.Value.Open();

                TextReader TempTextReader = new StreamReader(mesh.Value.GetStream());
                var data = JsonSerializer.Deserialize<MeshData>(TempTextReader.ReadToEnd());
                mesh.Value.Close();
                EntitiesMesh.Add(mesh.Key, new Model(data.Model));
            }
        }
    }
    internal class BlockMeshData
    {
        public float[][] Vertex { get; set; }
        public float[][] Uv { get; set; }
    }
    internal class MeshData
    {
        public float[] Model { get; set; }
    }

}
