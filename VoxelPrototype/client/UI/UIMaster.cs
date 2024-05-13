using Crecerelle.Utils;
using VoxelPrototype.client.UI.GUI;

namespace VoxelPrototype.client.UI
{
    internal class UIMaster
    {
        MainScreen MainScreen;
        OptionMenu OptionMenu;
        public UIMaster()
        {
            MainScreen = new MainScreen();
            OptionMenu = new OptionMenu();
            Client.TheClient.UIManager.AddUI(MainScreen);
            Client.TheClient.UIManager.AddUI(OptionMenu);
        }
        public static void OptionCallback()
        {
            Client.TheClient.UIMaster.MainScreen.Active =false;
            Client.TheClient.UIMaster.OptionMenu.Active = true;
        }
    }
    internal static class UIStyle
    {
        internal static string Font = "Voxel@opensans";
        internal static Color ButtonColor = new(0.4070806205272675f, 0.4172268807888031f, 0.4291845560073853f, 0.4000000059604645f); 
        internal static Color HoveredButtonColor = new(0.2505663931369781f, 0.2557240426540375f, 0.2618025541305542f, 1.0f); 
        internal static Color ClickedButtonColor = new(0.1931883096694946f, 0.1953609734773636f, 0.1974248886108398f, 1.0f); 
    }
}
