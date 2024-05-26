using OpenTK.Compute.OpenCL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using VoxelPrototype.client.GUI.Prototype.Text;
using VoxelPrototype.client.GUI.Prototype.Utils;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.GUI.Prototype.Elements
{
    internal class Button : UIElement
    {
        string Text = "";
        Action Callback;
        Vector4 Color = UIStyle.ButtonColor;
        bool Clicked;
        public Button(string text, Action callback)
        {
            Size = new Vector2(0, 75);
            Text = text;
            Callback = callback;
        }

        public override void Render(UIRenderer Renderer)
        {
            Renderer.RenderTextureQuad(Position, Size, Active ? Color : UIStyle.ButtonColor);
            Renderer.RenderText(new Resources.ResourceID("font/opensans"), Text, Position +
                new Vector2(Size.X / 2, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(new Resources.ResourceID("font/opensans"), 24, Text) / 2),
                24, AlignType: AlignementType.Center);
        }
        public override bool OnMouseClicked(MouseClickedEvent Event)
        {
            if (Event.Button == MouseButton.Left && MouseCheck.IsHovering(Event.MousePosition, Position, Position + Size))
            {
                Clicked = true;
                Color = UIStyle.ClickedButtonColor;
                return true;
            }
            return false;
        }
        public override bool OnMouseUp(MouseUpEvent Event)
        {
            if (Clicked && MouseCheck.IsHovering(Event.MousePosition, Position, Position + Size))
            {
                if (Callback != null)
                {
                    Callback();

                }
                Clicked = false;
                return true;
            }
            return false;
        }
        public override void Update(Vector2 MousePos)
        {
            if (!Clicked)
            {
                if (MouseCheck.IsHovering(MousePos, Position, Position + Size))
                {
                    Color = UIStyle.HoveredButtonColor;
                }
                else
                {
                    Color = UIStyle.ButtonColor;
                }
            }
            else
            {
                if (!MouseCheck.IsHovering(MousePos, Position, Position + Size))
                {
                    Clicked = false;
                }
            }
        }
    }
}
