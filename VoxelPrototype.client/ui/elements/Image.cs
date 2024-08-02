using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui.elements
{
    internal class Image : Element
    {
        string ImageResourceID;
        Vector2 TexCoordsStart = Vector2.Zero;
        Vector2 TexCoordsEnd = Vector2.One;
        bool KeepAspectRatio = false;
        float AspectRatio = 0;
        public Image(string imageResourceID,bool keepAspect =false, float aspectRatio = 0)
        {
            ImageResourceID = imageResourceID;
            KeepAspectRatio = keepAspect;
            AspectRatio = aspectRatio;
        }
        public Image(string imageResourceID, Vector2 CoordsStart, Vector2 CoordsEnd, bool keepAspect = false, float aspectRatio = 0)
        {
            ImageResourceID = imageResourceID;
            TexCoordsStart = CoordsStart;
            TexCoordsEnd = CoordsEnd;
            KeepAspectRatio = keepAspect;
            AspectRatio = aspectRatio;

        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            Size.Y =(int)( Size.X / AspectRatio);
            base.Update(MState, KSate);
        }

        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, 0xFFFFFFFF, ImageResourceID, TexCoordsStart, TexCoordsEnd);
        }
    }
}
