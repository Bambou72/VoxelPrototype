using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.common.Items
{
    public class ItemRegister
    {
        public Dictionary<string, Item> Items = new Dictionary<string, Item>();
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Texture GetTexture(string id)
        {
            Items.TryGetValue(id, out var item);
            if (item != null)
            {
                GetTexture(item.texture);
            }
            return null;
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
