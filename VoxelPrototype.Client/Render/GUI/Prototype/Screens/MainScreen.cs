using OpenTK.Mathematics;
using System.Reflection;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.GUI.Prototype.Elements;
using VoxelPrototype.client.Render.GUI.Prototype.Layouts;
using VoxelPrototype.client.Render.GUI.Prototype.Utils;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.utils;

namespace VoxelPrototype.client.Render.GUI.Prototype.Screens
{
    internal class MainScreen : Screen
    {
        VerticalStackPanel VerticalStackPanel;
        HorizontalStackPanel HorizontalStackPanel;
        public MainScreen()
        {
            VerticalStackPanel = new(20);
            HorizontalStackPanel = new(20) { Size = new Vector2(0, 75) };
            VerticalStackPanel.AddElement(new Button("Singleplayer", () => { System.Console.WriteLine("Test"); }) { Size = new Vector2(0, 75) });
            VerticalStackPanel.AddElement(new Button("Multiplayer", () => { System.Console.WriteLine("Test1"); }) { Size = new Vector2(0, 75) });
            VerticalStackPanel.AddElement(new Button("Mods", () => { System.Console.WriteLine("Test1"); }) { Size = new Vector2(0, 75) });
            //HorizontalStackPanel.AddElement(new Button("Options", () => { Client.TheClient.UIManager.SetCurrentScreen(new OptionScreen()); }) { Size = new Vector2(0, 75) });
            HorizontalStackPanel.AddElement(new Button("Quit", Client.TheClient.Close) { Size = new Vector2(0, 75) });
            VerticalStackPanel.AddElement(HorizontalStackPanel);
            AddElement(VerticalStackPanel);

        }
        internal override void Render(UIRenderer Renderer)
        {

            Renderer.RenderTextureQuad(new Vector2(Size.X / 2 - Size.X * 0.4f / 2, (int)(Size.Y * 0.05f)),
              new Vector2(Size.X * 0.4f, Size.X * 0.4f / 3.49f), Vector4.One, new ResourceID("textures/gui/title"), TextureStart: Vector2.Zero, TextureEnd: Vector2.One);
            base.Render(Renderer);
            Renderer.RenderText(new ResourceID("font/opensans"), "V" + EngineVersion.Version.ToString()
                , new Vector2(10, Size.Y - 10), 22);
            Renderer.RenderText(new ResourceID("font/opensans"), "©Florian Pfeiffer"
                , new Vector2(Size.X - TextSizeCalculator.CalculateSize(new ResourceID("font/opensans"), 22, "©Florian Pfeiffer") - 10, Size.Y - 10), 22);
        }
        public override void Compose(Vector2i ScreenSize)
        {
            //Singleplayerbutton
            VerticalStackPanel.Size = new Vector2((int)(ScreenSize.X * 0.3f), 1000);
            VerticalStackPanel.Position = new Vector2(ScreenSize.X / 2 - VerticalStackPanel.Size.X / 2, (int)(Size.Y * 0.05f) + Size.X * 0.4f / 3.49f + 20);
            base.Compose(ScreenSize);
        }
    }
}
