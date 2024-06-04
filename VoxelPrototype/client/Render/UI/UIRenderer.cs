using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.GUI.Prototype.Text;
using VoxelPrototype.client.Render.GUI.Prototype.Utils;
using VoxelPrototype.client.Render.Text;
using VoxelPrototype.client.Render.UI.Batch;
using VoxelPrototype.client.Resources;
namespace VoxelPrototype.client.Render.UI
{
    public class UIRenderer
    {
        TextBatch TextBatch = new();
        UIBatch UIBatch = new();
        internal UIRenderer()
        {
            TextBatch.InitBatch();
            TextBatch.Font = Client.TheClient.FontManager.GetFont( new ResourceID("font/opensans"));
        }
        public void RenderText(ResourceID Font, string Text, Vector2 Pos, float Scale = 25, Vector4 Color = default, AlignementType AlignType = AlignementType.Left)
        {
           
            UIBatch.Flush();
            Font FO = Client.TheClient.FontManager.GetFont(Font);
            if(TextBatch.Font != FO)
            {
                TextBatch.FlushBatch();
                TextBatch.Font = FO;
            }
            if (AlignType == AlignementType.Center)
            {
                Pos.X -= TextSizeCalculator.CalculateSize(Font, Scale, Text) / 2;
            }
            TextBatch.AddText(Text,Pos,Scale,Color);
        }
        public void RenderTextureQuad(Vector2 Pos, Vector2 Size, Vector4 Color, ResourceID Texture = null, Vector2 TextureStart = default, Vector2 TextureEnd = default)
        {
            TextBatch.FlushBatch();
            Texture Tex = null;
            
            if (Texture == null)
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(new ResourceID("textures/ui/blank"));
            }
            else
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(Texture);
            }
            if(UIBatch.CurrentTexture != Tex)
            {
                UIBatch.Flush();
                UIBatch.CurrentTexture = Tex;
            }
            Vertex[] Quad =
                    {
                        new Vertex{ Position = Pos ,TexCoords = new Vector2( TextureStart.X,TextureEnd.Y),Color = Color },
                        new Vertex{ Position = Pos + new Vector2(Size.X,0) ,TexCoords = new Vector2(TextureEnd.X,TextureEnd.Y),Color =Color },
                        new Vertex{ Position = Pos + new Vector2(0,Size.Y) ,TexCoords = new Vector2(TextureStart.X,TextureStart.Y),Color =Color },
                        new Vertex{ Position = Pos + new Vector2(Size.X,Size.Y) ,TexCoords = new Vector2( TextureEnd.X,TextureStart.Y),Color =Color },
                    };
            UIBatch.AddQuad(Quad);
        }
        public void Flush()
        {
            TextBatch.FlushBatch();
            UIBatch.Flush();

        }
        public void FinishRendering()
        {
            TextBatch.FlushBatch();
            UIBatch.Flush();
        }
    }
}
