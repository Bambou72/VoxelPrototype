using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.api.Items
{
    public static class ItemRegister
    {
        public static Dictionary<string, Item> Items = new Dictionary<string, Item>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static Texture GetTexture(string id)
        {
            Items.TryGetValue(id, out var item);
            if (item != null)
            {
                GetTexture(item.texture);
            }
            return null;
        }
        public static string GetItemID(string ModName, string ItemName)
        {
            return ModName + "@" + ItemName;
        }
        public static bool RegisterItem(string Id, Item block)
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
