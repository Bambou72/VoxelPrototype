using Crecerelle.Text;
using Crecerelle.Utils;
using OpenTK.Mathematics;

namespace Crecerelle.Renderer
{
    public interface IUIRenderer
    {
        public void RenderText(string Font, string Text, Vector3 Pos, float Scale = 25, Vector3 Color = default, AlignementType AlignType = AlignementType.Left);

        public void RenderTextureQuad(Vector3 Pos, Vector2i Size, Color Color, string Texture = null, Vector2 TextureStart = default, Vector2 TextureEnd = default);
        public void RenderQuad(Vector3 Pos, Vector2i Size, Color Color);
        public int GetTextSize(string Font, float Scale, string Text);
        public int GetVerticalTextSize(string Font, float Scale, string Text);
        public void Render();
    }
}
