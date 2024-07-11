using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.client.Render.GUI.Prototype.Utils;
using VoxelPrototype.utils;

namespace VoxelPrototype.client.Render.GUI.Prototype.Elements
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
                Renderer.RenderTextureQuad(Position, Size, Vector4.One, new ResourceID("textures/gui/icons"), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f));
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
