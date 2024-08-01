using OpenTK.Mathematics;
using VoxelPrototype.client.ui.elements;
using VoxelPrototype.client.ui.elements.container;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui.screens
{
    internal class WorldSelectionScreen : UIScreen
    {
        Panel WorldScrollPanel;

        public WorldSelectionScreen(UIScreen Parent)
        {
            this.Parent = Parent;
            WorldScrollPanel = new(new(0, 20), true);
            for (int i = 0; i < 10; i++)
            {
                WorldScrollPanel.Children.Add(new Button("Test" + i, () => { }) { Size = new(600, 100) });
            }
            Children.Add(WorldScrollPanel);

        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.PartFrame);
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }

        public override void ComputeLayout()
        {
            WorldScrollPanel.Position = Position + new Vector2i(0,50);
            WorldScrollPanel.Size = Size - new Vector2i(0,200);

            base.ComputeLayout();
        }
    }
}
