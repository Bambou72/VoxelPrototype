using OpenTK.Mathematics;
using VoxelPrototype.client.ui.elements;
using VoxelPrototype.client.ui.elements.container;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui.screens
{
    /*
    internal class MainScreen : UIScreen
    {

        static string LogoResourceID = "voxelprototype:textures/gui/title";
        Panel MainPanel;
        Button SingleplayerButton;
        Button MultiplayerButton;
        Button ModsButton;
        Button OptionButton;
        Button CreditsButton;
        Button QuitButton;
        Image Logo;
        public MainScreen()
        {
            MainPanel = new();
            Logo = new Image(LogoResourceID,true, 3.585714f) { Size = new Vector2i(0, 100) };
            MainPanel.Children.Add(Logo);
            MainPanel.Children.Add(new Spacing(20));
            SingleplayerButton = new Button("Singleplayer", () =>
            {
                Client.TheClient.UIManager.SetCurrentScreen(new WorldSelectionScreen(this));
            }){ Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(SingleplayerButton);
            MultiplayerButton = new Button("Multiplayer", () =>
            {
                Client.TheClient.UIManager.SetCurrentScreen(new MultiplayerScreen(this));
            }){ Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(MultiplayerButton);
            ModsButton = new Button("Mods", () =>
            {
                Client.TheClient.UIManager.SetCurrentScreen(new ModsScreen(this));
            }){ Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(ModsButton);
            OptionButton = new Button("Options", () =>
            {
                Client.TheClient.UIManager.SetCurrentScreen(new OptionsScreen(this));
            }){ Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(OptionButton); 
            CreditsButton = new Button("Credits", () =>
            {
                Client.TheClient.UIManager.SetCurrentScreen(new CreditsScreen(this));
            })
            { Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(CreditsButton); 
            QuitButton = new Button("Quit", Client.TheClient.Close) { Size = new Vector2i(0, 50) };
            MainPanel.Children.Add(QuitButton);
            Children.Add(MainPanel);
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position,Size,0xFFFFFFFF,"voxelprototype:textures/gui/temp",new Vector2(0,0),new(1,1));
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
            Renderer.RenderTextCentered(EngineVersion.Version.ToString(), new Vector2i(ScreenSize.X/2, Size.Y - 10));
        }
        public override void ComputeLayout()
        {
            int YPosition = 60;
            MainPanel.Position = new(Client.TheClient.ClientSize.X / 2 -( (int)(Client.TheClient.ClientSize.X * 0.30f) /2), YPosition);
            MainPanel.Size = new((int)(Client.TheClient.ClientSize.X * 0.30f), Client.TheClient.ClientSize.Y);
            base.ComputeLayout();         
        }
    }*/
}
