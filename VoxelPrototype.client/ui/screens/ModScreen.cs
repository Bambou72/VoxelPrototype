using OpenTK.Mathematics;
using VoxelPrototype.client.ui.renderer;

namespace VoxelPrototype.client.ui.screens
{
    internal class ModsScreen : UIScreen
    {
        public ModsScreen(UIScreen Parent)
        {
            this.Parent = Parent;
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.PartFrame);
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }

    }
}
