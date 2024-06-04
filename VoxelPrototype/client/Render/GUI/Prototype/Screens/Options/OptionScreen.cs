using OpenTK.Mathematics;
using System.Drawing;
using System.Reflection;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.GUI.Prototype.Elements;
using VoxelPrototype.client.Render.GUI.Prototype.Layouts;
using VoxelPrototype.client.Render.GUI.Prototype.Utils;
using VoxelPrototype.client.Render.UI;

namespace VoxelPrototype.client.Render.GUI.Prototype.Screens.Options
{
    internal class OptionScreen : Screen

    {
        VerticalStackPanel MainStackPanel;
        VerticalStackPanel VerticalStackPanelLeft;
        VerticalStackPanel VerticalStackPanelRight;
        HorizontalStackPanel HorizontalStackPanel;
        internal override void Render(UIRenderer Renderer)
        {
            base.Render(Renderer);
            string Version = "V" + EngineVersion.Version.ToString();
            Renderer.RenderText(new Resources.ResourceID("font/opensans"), Version
    , new Vector2(Size.X / 2 - TextSizeCalculator.CalculateSize(new Resources.ResourceID("font/opensans"), 20, Version) / 2, Size.Y - 10), 20);

        }
        public OptionScreen()
        {
            MainStackPanel = new(20);
            VerticalStackPanelLeft = new(20) { Size = new Vector2(0, 1000) };
            VerticalStackPanelRight = new(20) { Size = new Vector2(0, 1000) };

            //VerticalStackPanelLeft.AddElement(new Button("Video Settings", () => { Client.TheClient.UIManager.SetCurrentScreen(new VideoSettingsScreen()); }) { Size = new Vector2(0, 75) });
            VerticalStackPanelRight.AddElement(new Button("Debug Settings", null) { Size = new Vector2(0, 75) });



            HorizontalStackPanel = new(20) { Size = new(0, 500) };
            HorizontalStackPanel.AddElement(VerticalStackPanelLeft);
            HorizontalStackPanel.AddElement(VerticalStackPanelRight);
            MainStackPanel.AddElement(HorizontalStackPanel);
            MainStackPanel.AddElement(new Button("Finish", Close) { Size = new Vector2(0, 75) });
            AddElement(MainStackPanel);
            Parent = new MainScreen();
        }
        public override void Compose(Vector2i ScreenSize)
        {
            MainStackPanel.Size = new Vector2((int)(ScreenSize.X * 0.5f), 1000);
            HorizontalStackPanel.Size.Y = ScreenSize.Y * 0.5f;
            MainStackPanel.Position = new Vector2(ScreenSize.X / 2 - HorizontalStackPanel.Size.X / 2, ScreenSize.Y * 0.2f);

            base.Compose(ScreenSize);
        }
        public override bool OnKeyPressed(KeyPressedEvent Event)
        {
            if (Event.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)
            {
                Close();
                return true;
            }
            return false;
        }
    }
}
