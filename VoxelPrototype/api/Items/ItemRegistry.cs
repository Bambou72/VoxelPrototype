namespace VoxelPrototype.api.Items
{
    public class ItemRegistry
    {
        private static ItemRegistry Instance;
        public static ItemRegistry GetInstance()
        {
            return Instance;
        }
        public Dictionary<string, Item> Items = new Dictionary<string, Item>();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ItemRegistry()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new InvalidOperationException("You can't instanciate more than 1 instance of singleton");
            }

        }

        

        public bool RegisterItem(string Id, Item block)
        {
            try
            {
                Items.Add(Id, block);
                Logger.Info($"New item : {Id}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
