using OpenTK.Mathematics;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.client;
using VoxelPrototype.client.Resources.data;
using VoxelPrototype.common.Physics;
namespace VoxelPrototype.api.Blocks
{
    public class Block
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BlockStateHolder StateHolder;
        private BlockState Default;
        internal string ID;
        public string Model;
        public string Data;
        public float Friction = 8;
        public int BreakingTime = 20;
        public BlockRenderType RenderType;
        public bool Transparency = false;
        internal void Gernerate()
        {
            BlockStateBuilder Builder = new BlockStateBuilder(this);
            RegisterProperties(Builder);
            StateHolder = Builder.Build();
            Default = StateHolder.GetBaseState();
        }
        public virtual void RegisterProperties(BlockStateBuilder Builder)
        {
        }

        public BlockState GetDefaultState()
        {
            return Default;
        }
        internal float[] GetMesh(int Face)
        {
            try
            {
                var modelData = Client.TheClient.ResourcePackManager.GetBlockMesh(Model);
                return modelData.GetMesh()[Face];

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Array.Empty<float>();

            }
        }
        internal float[] GetMeshTextureCoordinates(int Face)
        {
            try
            {
                var modelData = Client.TheClient.ResourcePackManager.GetBlockMesh(Model);
                return modelData.GetUV()[Face];
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Array.Empty<float>();
            }
        }
        internal float[] GetTextureCoordinates(int Face, BlockState State)
        {
            try
            {
                BlockStateData StateData = Client.TheClient.ResourcePackManager.GetBlockStateData(this.Data);
                BlockData Data;
                if (StateHolder.GetStates().Count == 1)
                {
                    Data = StateData.variants[""];
                }
                else
                {
                    if (StateData.variants.ContainsKey(State.ToString()))
                    {
                        Data = StateData.variants[State.ToString()];
                    }
                    else
                    {
                        Data = StateData.variants[""];
                    }
                }
                if (Data.textures.all != null)
                {
                    return Client.TheClient.ResourcePackManager.GetAtlasTexturesCoord(Data.textures.all);
                }
                else
                {
                    return Client.TheClient.ResourcePackManager.GetAtlasTexturesCoord(Data.textures.textures[Face]);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Client.TheClient.ResourcePackManager.GetAtlasTexturesCoord("unknow");
            }
        }
        public virtual Collider[] GetColliders()
        {
            return new Collider[] { new Collider(Vector3d.Zero, Vector3d.One) };
        }
        public virtual void OnInteract(Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }
        public virtual void OnPlaced(Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }
        public virtual void OnBreaked(Vector3i BlockPosition, BlockState State, bool ServerSide)
        {

        }

    }
    public enum BlockRenderType
    {
        Cube,
        Custom,
    }
}
