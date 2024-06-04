using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.UI;

namespace VoxelPrototype.client.Render.GUI.Prototype.Layouts
{
    internal class HorizontalStackPanel : Layout
    {
        int Offset;

        public HorizontalStackPanel(int offset)
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

            float ElementSize = Size.X / Elements.Count - Offset / 2;
            float CurrentXPos = Position.X;
            foreach (UIElement Element in Elements)
            {
                Element.Position.X = CurrentXPos;
                Element.Position.Y = Position.Y;
                Element.Size.X = ElementSize;
                CurrentXPos += ElementSize + Offset;

            }
            base.Compose(ScreenSize);
        }
    }
}
