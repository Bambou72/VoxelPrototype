using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.client.GUI.Prototype.Utils;

namespace VoxelPrototype.client.GUI.Prototype.Elements
{
    internal class CheckBox : UIElement
    {
        bool Value;

        public CheckBox()
        {
            Size = new Vector2(50, 50);
        }

        public override void Render(UIRenderer Renderer)
        {
            if (Value)
            {
                Renderer.RenderTextureQuad(Position, Size, Vector4.One, "Voxel@ui/icons", new Vector2(0f, 1f), new Vector2(0.5f, 0.5f));
            }
            Renderer.RenderTextureQuad(Position, Size, UIStyle.ButtonColor);
        }
        public override bool OnMouseClicked(MouseClickedEvent Event)
        {
            if (MouseCheck.IsHovering(Event.MousePosition, Position, Position + Size))
            {
                Value = !Value;
                return true;
            }
            return false;
        }

    }
}
