using OpenTK.Mathematics;
using VoxelPrototype.client.GUI.Prototype.Elements;
using VoxelPrototype.client.GUI.Prototype.Layouts;

namespace VoxelPrototype.client.GUI.Prototype.Screens.Options
{
    internal class VideoSettingsScreen : Screen
    {
        ScrollPanel ScrollPanel;

        public VideoSettingsScreen()
        {
            ScrollPanel = new ScrollPanel(20);
            ScrollPanel.AddElement(new Button("Test", null));
            ScrollPanel.AddElement(new CheckBox());

            AddElement(ScrollPanel);
            Parent = new OptionScreen();
        }
        public override void Compose(Vector2i ScreenSize)
        {
            base.Compose(ScreenSize);
            ScrollPanel.Size = new Vector2(ScreenSize.X * 0.5f, ScreenSize.Y * 0.8f);
            ScrollPanel.Position = new Vector2(ScreenSize.X / 2 - ScrollPanel.Size.X / 2, ScreenSize.Y * 0.1f);
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
