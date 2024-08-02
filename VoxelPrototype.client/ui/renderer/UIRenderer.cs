using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.client.ui.renderer.batch;
using VoxelPrototype.client.ui.utils;
namespace VoxelPrototype.client.ui.renderer
{
    public class UIRenderer
    {
        private static string BaseFontID = "voxelprototype:font/default";
        private static string BlankResourceID = "voxelprototype:textures/gui/blank";
        private readonly Vertex[] QuadVertices = new Vertex[4];
        TextBatch TextBatch;
        UIBatch UIBatch;
        internal UIRenderer()
        {
                TextBatch = new();
                UIBatch = new();
                TextBatch.InitBatch();
        }
        public void RenderText(string Text, Vector2i Pos, int Scale = 2,uint Color = 0xFFFFFFFF ,bool Shadow = false)
        {
            Font Font = Client.TheClient.FontManager.GetFont(BaseFontID);
            if(Shadow)
            {
                TextBatch.AddText(Font, Text, Pos + new Vector2i(Scale), Scale, ReduceColorByPercentage(Color, 0.3f));
            }
            TextBatch.AddText(Font ,Text, Pos, Scale, Color);
            TextBatch.FlushBatch(Font);
        }
        public void RenderTextCentered(string Text, Vector2i Pos, int Scale = 2, uint Color = 0xFFFFFFFF, bool Shadow = false)
        {
            Font Font = Client.TheClient.FontManager.GetFont(BaseFontID);
            Pos.X -= TextSizeCalculator.CalculateSize(Text, Scale) / 2;
            if (Shadow)
            {
                TextBatch.AddText(Font, Text, Pos + new Vector2i(Scale), Scale, ReduceColorByPercentage(Color, 0.3f));
            }
            TextBatch.AddText(Font, Text, Pos, Scale, Color);
            TextBatch.FlushBatch(Font);
        }
        public void RenderTextCentered(string FontRessourceID, string Text, Vector2i Pos, int Scale = 2, uint Color = 0xFFFFFFFF, bool Shadow = false)
        {
            Font Font = Client.TheClient.FontManager.GetFont(FontRessourceID);
            Pos.X -= TextSizeCalculator.CalculateSize(Text, Scale) / 2;
            if (Shadow)
            {
                TextBatch.AddText(Font, Text, Pos + new Vector2i(Scale), Scale, ReduceColorByPercentage(Color, 0.3f));
            }
            TextBatch.AddText(Font, Text, Pos, Scale, Color);
            TextBatch.FlushBatch(Font);
        }
        public void StartScissor(Vector2i Position, Vector2i Size)
        {
            GL.Scissor(Position.X,Client.TheClient.ClientSize.Y - Size.Y-Position.Y,Size.X,Size.Y);
            GL.Enable(EnableCap.ScissorTest);
        }
        public void EndScissor()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
        public void RenderText(string FontRessourceID  , string Text, Vector2i Pos, int Scale = 2, uint Color =0xFFFFFFFF, bool Shadow = false)
        {
            Font Font = Client.TheClient.FontManager.GetFont(FontRessourceID);
            if (Shadow)
            {
                TextBatch.AddText(Font, Text, Pos + new Vector2i(Scale), Scale, ReduceColorByPercentage(Color, 0.35f));
            }
            TextBatch.AddText(Font, Text, Pos, Scale, Color);
            TextBatch.FlushBatch(Font);
        }
        static uint ReduceColorByPercentage(uint color, float percentage)
        {
            // Extract each component
            uint red = (color >> 24) & 0xFF;
            uint green = (color >> 16) & 0xFF;
            uint blue = (color >> 8) & 0xFF;
            uint alpha = color & 0xFF;
            red = (uint)(red * percentage);
            green = (uint)(green * percentage);
            blue = (uint)(blue * percentage);
            return (red << 24) | (green << 16) | (blue << 8) | alpha;
        }
        public void RenderTextureQuad(Vector2i Position, Vector2i Size, uint Color, string Texture = null, Vector2 TextureStart = default, Vector2 TextureEnd = default)
        {
            Texture Tex = null;

            if (Texture == null)
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(BlankResourceID);
            }
            else
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(Texture);
            }
            QuadVertices[0].Position = Position;
            QuadVertices[0].TexCoords = new Vector2(TextureStart.X, TextureEnd.Y);
            QuadVertices[0].Color = Color;
            //
            QuadVertices[1].Position = new(Position.X + Size.X,Position.Y);
            QuadVertices[1].TexCoords = new Vector2(TextureEnd.X, TextureEnd.Y);
            QuadVertices[1].Color = Color;
            //
            QuadVertices[2].Position = new(Position.X,Position.Y +Size.Y);
            QuadVertices[2].TexCoords = new Vector2(TextureStart.X, TextureStart.Y);
            QuadVertices[2].Color = Color;
            //
            QuadVertices[3].Position = new(Position.X + Size.X,Position.Y + Size.Y);
            QuadVertices[3].TexCoords = new Vector2(TextureEnd.X, TextureStart.Y);
            QuadVertices[3].Color = Color;
            UIBatch.RenderPolygone(QuadVertices,Tex);
        }
    }
}
