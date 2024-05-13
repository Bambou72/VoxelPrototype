using Crecerelle;
using Crecerelle.Elements;
using Crecerelle.Renderer;
using Crecerelle.Utils;
namespace VoxelPrototype.client.UI.Elements
{
    public class Label : UIElement
    {
        public string Text;
        public float Scale;
        public string Font;
        public Color Color;

        public Label(string text, float scale, string font, Color color)
        {
            LockInput = false;
            Text = text;
            Scale = scale;
            Font = font;
            Color = color;
        }

        public override void Render(IUIRenderer Renderer)
        {
            base.Render(Renderer);
            Size = new OpenTK.Mathematics.Vector2i(Renderer.GetTextSize(Font, Scale, Text), Renderer.GetVerticalTextSize(Font, Scale, Text) - (int)Scale);

            Renderer.RenderText(Font, Text, new OpenTK.Mathematics.Vector3(Position.X, Position.Y, ZOrder), Scale, Color.RGB);
        }
        public override void Update(UIManager Manager)
        {
            base.Update(Manager);
        }
    }
}
