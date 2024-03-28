using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.API.Blocks.state;
using VoxelPrototype.common.API.Blocks.State;
using VoxelPrototype.common.RessourceManager.data;
namespace VoxelPrototype.common.API.Blocks
{
    public class Block
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BlockStateHolder StateHolder;
        private BlockState Default;
        internal string ID;
        public string Model;
        public string Collider;
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
        public virtual void OnInteract(Vector3i Pos,BlockState State)
        {

        }
        public BlockState GetDefaultState()
        {
            return Default;
        }
        internal float[] GetMesh( int Face)
        {
            try
            {
                var modelData = ClientRessourcePackManager.GetRessourcePackManager().GetBlockMesh(Model);
                return modelData.GetMesh()[Face];

            }catch (Exception e)
            {
                Logger.Error(e);
                return Array.Empty<float>();

            }
        }
        internal  float[] GetMeshTextureCoordinates( int Face)
        {
            try
            {
                var modelData = ClientRessourcePackManager.GetRessourcePackManager().GetBlockMesh(Model);
                return modelData.GetUV()[Face];
            }
            catch(Exception e) 
            {
                Logger.Error(e);
                return Array.Empty<float>();
            }
        }
        internal float[] GetTextureCoordinates(int Face,BlockState State)
        {
            try
            {
                BlockStateData StateData = ClientRessourcePackManager.GetRessourcePackManager().GetBlockStateData(this.Data);
                BlockData Data;
                if(StateHolder.GetStates().Count == 1)
                {
                    Data = StateData.variants[""];
                }else
                {
                    if(StateData.variants.ContainsKey(State.ToString()))
                    {
                        Data = StateData.variants[State.ToString()];
                    }else
                    {
                        Data = StateData.variants[""];
                    }
                }
                if (Data.textures.all != null)
                {
                    return ClientRessourcePackManager.GetRessourcePackManager().GetAtlasTexturesCoord(Data.textures.all);
                }
                else
                {
                    return ClientRessourcePackManager.GetRessourcePackManager().GetAtlasTexturesCoord(Data.textures.textures[Face]);
                }
            }
            catch (Exception e) 
            {
                Logger.Error(e);
                return ClientRessourcePackManager.GetRessourcePackManager().GetAtlasTexturesCoord("unknow");
            }
        }
    }
    public enum BlockRenderType
    {
        Cube,
        Custom,
    }
}
