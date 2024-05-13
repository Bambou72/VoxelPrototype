using Crecerelle;
using Crecerelle.Constraints;
using Crecerelle.Elements;
using Crecerelle.Renderer;
using Crecerelle.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.client.UI.Elements
{
    internal class Button : UIElement
    {
        public Action OnClick;
        public string Text ="";
        public Color CurrentColor;

        public Button(Action onClick, string text)
        {
            OnClick = onClick;
            Text = text;
        }

        public override void Update(UIManager Manager)
        {
            CurrentColor = UIStyle.ButtonColor;
            base.Update(Manager);
        }
        public override void Render(IUIRenderer Renderer)
        {
            base.Render(Renderer);
            float Scale =  Size.Y *0.5f;
            Renderer.RenderQuad(new Vector3(Position.X, Position.Y, ZOrder),Size,CurrentColor);
            Renderer.RenderText(UIStyle.Font,
                Text,
                new Vector3(Position.X + (Size.X/2)- (Renderer.GetTextSize(UIStyle.Font, Scale, Text) /2),
                Position.Y + (Size.Y / 2) - (Renderer.GetVerticalTextSize(UIStyle.Font, Scale, Text) / 2) + Scale
                , ZOrder + 0.00009f),Scale);
        }
        public override void OnHovered(UIManager Manager)
        {
            if(InputSystem.MousePressed(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
            {
                CurrentColor = UIStyle.ClickedButtonColor;
                if(OnClick != null)
                {
                    OnClick();
                }
            }
            else
            {
                CurrentColor = UIStyle.HoveredButtonColor;

            }
        }
    }
}
