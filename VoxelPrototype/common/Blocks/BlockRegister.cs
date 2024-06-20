using VoxelPrototype.common.Blocks.State;

namespace VoxelPrototype.common.Blocks
{
    public class BlockRegister
    {
        internal Dictionary<string, Block> Blocks = new Dictionary<string, Block>();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public BlockState Air;
        public int RegisteredBlockCounter;
        public BlockRegister()
        {
            RegisterBlock("air", new Block() { Transparent = true });
            Air = GetBlock("air").GetDefaultState();

        }
        public string GetBlockID(string ModName, string BlockName)
        {
            return ModName + ":" + BlockName;
        }
        public bool RegisterBlock(string Id, Block block)
        {
            try
            {
                block.ID = Id;
                block.Gernerate();
                Blocks.Add(Id, block);
                //Logger.Info($"New block : {Id}");
                RegisteredBlockCounter++;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void Finalize()
        {
            Logger.Info(RegisteredBlockCounter + " blocks have been loaded.");
        }
        public Block GetBlock(string Id)
        {
            return Blocks[Id];
        }
        internal byte GetTransForAO(BlockState ID)
        {
            if (ID == Air)
            {
                return 1;
            }
            else if (Blocks.TryGetValue(ID.Block.ID, out var temp))
            {
                if (temp.Transparent)
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
