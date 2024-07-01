using OpenTK.Mathematics;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.Render.GUI.Prototype.Layouts
{
    internal class VerticalStackPanel : Layout
    {
        int Offset;

        public VerticalStackPanel(int offset)
        {
            Offset = offset;
        }
        public override void Render(UIRenderer Renderer)
        {
            base.Render(Renderer);
            //Renderer.RenderTextureQuad(Position,  Size, new Vector4(1,0,0,1));
        }
        public override void Compose(Vector2i ScreenSize)
        {
            float CurrentYPos = Position.Y;
            foreach (UIElement Element in Elements)
            {
                Element.Position.X = Position.X;
                Element.Position.Y = CurrentYPos;
                Element.Size.X = Size.X;
                CurrentYPos += Element.Size.Y + Offset;

            }
            base.Compose(ScreenSize);
        }
    }
}
