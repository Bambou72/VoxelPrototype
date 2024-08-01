using OpenTK.Mathematics;
using VoxelPrototype.client.ui.elements.container;
using VoxelPrototype.client.ui.elements;
using VoxelPrototype.server;
using VoxelPrototype.client.ui.renderer;

namespace VoxelPrototype.client.ui.screens
{
    internal class OptionsScreen : UIScreen
    {
        TabManager TabManager;
        //VideoSettings
        TabPage VideoSettingsPage;
        Panel VideoSettingsVerticalPanel;
        //
        TabPage SoundsSettingsPage;
        TabPage ControlsSettingsPage;
        TabPage LanguageSettingsPage;
        TabPage ResourcePacksSettingsPage;
        public OptionsScreen(UIScreen Parent)
        {
            this.Parent = Parent;
            TabManager = new TabManager();
            Children.Add(TabManager);
            VideoSettingsPage = new TabPage()
            {
                Name = "Video Settings"
            };
            VideoSettingsVerticalPanel = new Panel(new(20,20));
            VideoSettingsVerticalPanel.Children.Add(new SliderInt("Render Distance", SetRenderDistance, GetRenderDistance, 2, 32) { ParentSizing = false});
            VideoSettingsVerticalPanel.Children.Add(new CheckBox("Fullscreen", Client.TheClient.SetFullscreen, Client.TheClient.GetFullscreen) { ParentSizing = false });
            VideoSettingsVerticalPanel.Children.Add(new SliderFloat("Fov", SetFov, GetFov, 1, 110, 0.5f) { ParentSizing = false });
            VideoSettingsPage.Children.Add(VideoSettingsVerticalPanel);
            TabManager.AddTab(VideoSettingsPage);
            SoundsSettingsPage = new TabPage()
            {
                Name = "Sounds"
            };
            TabManager.AddTab(SoundsSettingsPage);
            ControlsSettingsPage = new TabPage()
            {
                Name = "Controls"
            };
            TabManager.AddTab(ControlsSettingsPage);
            LanguageSettingsPage = new TabPage()
            {
                Name = "Language"
            };
            TabManager.AddTab(LanguageSettingsPage);
            ResourcePacksSettingsPage = new TabPage()
            {
                Name = "ResourcePacks"
            };
            TabManager.AddTab(ResourcePacksSettingsPage);
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.PartFrame);
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }
        private float GetFov()
        {
            if (Client.TheClient.World.IsLocalPlayerExist())
            {
                return Client.TheClient.World.GetLocalPlayerCamera().Fov;
            }
            return 70f;
        }
        private void SetRenderDistance(int Value)
        {
            if (Client.TheClient.World.IsLocalPlayerExist())
            {
                Client.TheClient.World.RenderDistance = Value;
                if (Client.TheClient.EmbedderServer != null && Client.TheClient.EmbedderServer.IsRunning())
                {
                    Server.TheServer.World.LoadDistance = Client.TheClient.World.RenderDistance + 1;
                }
            }
        }
        private int GetRenderDistance()
        {
            if (Client.TheClient.World.IsLocalPlayerExist())
            {
                return Client.TheClient.World.RenderDistance;
            }
            return 12;
        }
        private void SetFov(float Value)
        {
            if (Client.TheClient.World.IsLocalPlayerExist())
            {
                Client.TheClient.World.GetLocalPlayerCamera().Fov = Value;
            }
        }
        public override void ComputeLayout()
        {
            TabManager.Position = new Vector2i(Position.X);
            TabManager.Size = new Vector2i(Size.X, Size.Y);
            base.ComputeLayout();
        }
    }
}
