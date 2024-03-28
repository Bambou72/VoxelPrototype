using VoxelPrototype.common.API.Blocks.State;
namespace VoxelPrototype.common.API.Blocks
{
    public static class BlockRegister
    {
        internal static Dictionary<string, Block> Blocks = new Dictionary<string, Block>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static BlockState Air;
        internal static void Initialize()
        {
            RegisterBlock("air", new Block() { Transparency = true });
            Air =GetBlock("air").GetDefaultState();
        }
        public static string GetBlockID(string ModName, string BlockName)
        {
            return ModName + "@" + BlockName;
        }
        public static bool RegisterBlock(string Id, Block block)
        {
            try
            {
                block.ID = Id;
                block.Gernerate();
                Blocks.Add(Id, block);
                Logger.Info($"New block : {Id}");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Block GetBlock(string Id)
        {
            return Blocks[Id];
        }
        internal static int GetTransForAO(BlockState ID)
        {
            if (ID == BlockRegister.Air)
            {
                return 1;
            }
            else if (Blocks.TryGetValue(ID.Block.ID, out var temp))
            {
                if (temp.Transparency)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }


    }
}
