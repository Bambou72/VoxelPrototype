using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
using VoxelPrototype.client.ui.utils;
namespace VoxelPrototype.client.ui.elements
{
    internal class Button : Element
    {
        string Text = "";
        Action Callback;
        bool Clicked;
        public Button(string text, Action callback)
        {
            Size = new Vector2i(0, 75);
            Text = text;
            Callback = callback;
        }

        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, IsHovered ? UIStyle.HoveredButton : UIStyle.BaseButton);
            Renderer.RenderTextCentered(Text, Position + new Vector2i(Size.X / 2, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Text) / 2));
        }
        public override void Update(MouseState MSate, KeyboardState KState)
        {
            if (!IsHovered)
            {
                Clicked = false;
            }
            else if (MSate.IsButtonPressed(MouseButton.Left))
            {
                Clicked = true;
                if (Callback != null)
                {
                    Callback();
                }
            }
            else
            {
                Clicked = false;
            }
        }
    }
}
