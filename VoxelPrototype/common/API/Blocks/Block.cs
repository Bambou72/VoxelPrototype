using VoxelPrototype.client;
using VoxelPrototype.common.API.Blocks.state;
using VoxelPrototype.common.RessourceManager.data;
namespace VoxelPrototype.common.API.Blocks
{
    public class Block
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BlockStateHolder StateHolder;
        private BlockState Default;
        internal string Id;
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
            StateHolder = Builder.build();
            Default = StateHolder.GetBaseState();
        }
        public virtual void RegisterProperties(BlockStateBuilder Builder)
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
        internal float[] GetTextureCoordinates(int Face)
        {
            try
            {
                BlockData Data = ClientRessourcePackManager.GetRessourcePackManager().GetBlockData(this.Data);
                if (Data.Textures.All != null)
                {
                    return ClientRessourcePackManager.GetRessourcePackManager().GetAtlasTexturesCoord(Data.Textures.All);
                }
                else
                {
                    return ClientRessourcePackManager.GetRessourcePackManager().GetAtlasTexturesCoord(Data.Textures.Textures[Face]);
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
