using VoxelPrototype.common.RessourceManager;
namespace VoxelPrototype.client
{
    internal static class ClientRessourcePackManager
    {
        internal static RessourcePackManager RessourcePackManager = new RessourcePackManager();
        public static RessourcePackManager GetRessourcePackManager()
        {
            return RessourcePackManager;
        }
    }
}
