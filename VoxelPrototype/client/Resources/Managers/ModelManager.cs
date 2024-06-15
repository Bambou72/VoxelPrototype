using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.Components;

namespace VoxelPrototype.client.Resources.Managers
{
    internal class ModelManager : IReloadableResourceManager
    {

        //
        private Dictionary<ResourceID, BlockMesh> BlockMeshs = new Dictionary<ResourceID, BlockMesh>();
        private  Dictionary<ResourceID, Model> EntitiesMesh = new Dictionary<ResourceID, Model>();

        public BlockMesh GetBlockMesh(ResourceID resourceID)
        {
            if(BlockMeshs.TryGetValue(resourceID, out BlockMesh blockMesh))
            {
                return blockMesh;
            }
            throw new Exception("Can't find block mesh");
        }
        public Model GetEntityModel(ResourceID resourceID)
        {
            if (EntitiesMesh.TryGetValue(resourceID, out Model blockMesh))
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
                TextReader TempTextReader = new StreamReader(mesh.Value.GetStream());
                var data = JsonConvert.DeserializeObject<BlockMeshData>(TempTextReader.ReadToEnd());
                mesh.Value.Close();

                BlockMeshs.Add(mesh.Key, new BlockMesh(data.Vertex, data.Uv));
            }
            //Entities Mesh
            var EntitiesMeshs = Manager.ListResources("models/entity", path => path.EndsWith(".json"));
            foreach (var mesh in EntitiesMeshs)
            {
                TextReader TempTextReader = new StreamReader(mesh.Value.GetStream());
                var data = JsonConvert.DeserializeObject<MeshData>(TempTextReader.ReadToEnd());
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
