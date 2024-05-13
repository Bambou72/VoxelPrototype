using Crecerelle.Elements;
using Crecerelle.Renderer;
using Crecerelle.Utils;
using OpenTK.Mathematics;

namespace VoxelPrototype.client.UI.Elements
{
    internal class Title : UIElement
    {
        public string Texture;

        public Title(string texture)
        {
            LockInput = false;

            Texture = texture;
        }
        public override void Render(IUIRenderer Renderer)
        {
            base.Render(Renderer);
            Renderer.RenderTextureQuad(new Vector3(Position.X, Position.Y, ZOrder), Size,Color.White,Texture);
        }
    }
}
