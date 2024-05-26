using Crecerelle.Renderer;
using Crecerelle.Text;
using Crecerelle.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Text;
using VoxelPrototype.client.Render.UI.Batch;
using VoxelPrototype.client.Render.UI.CMD;
namespace VoxelPrototype.client.Render.UI
{
    public class UIRenderer : IUIRenderer
    {
        TextBatch TextBatch = new();
        UIBatch UIBatch = new();
        List<TextCMD> TextsCommands = new();
        List<QUADCMD> UICommands = new();
        internal UIRenderer()
        {
            TextBatch.InitBatch();
            TextBatch.Font = Client.TheClient.FontManager.GetFont( new ResourceID("font/opensans"));
        }
        public void RenderText(string Font, string Text, Vector3 Pos, float Scale = 25, Vector3 Color = default, AlignementType AlignType = AlignementType.Left)
        {
            Font FO = Client.TheClient.ResourcePackManager.GetFont(Font);
            if (AlignType == AlignementType.Center)
            {
                Pos.X -= TextSizeCalculator.CalculateSize(FO, Scale, Text) / 2;
            }
            TextsCommands.Add(new TextCMD
            {
                Text = Text,
                Color = Color,
                Position = Pos,
                Scale = Scale,
                Font = FO
            });
        }
        public void RenderTextureQuad(Vector3 Pos, Vector2i Size, Color Color, string Texture = null, Vector2 TextureStart = default,Vector2 TextureEnd = default)
        {
            Texture Tex = null;
            if (Texture == null)
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(new ResourceID("textures/ui/blank"));
            }
            else
            {
                Tex = (Texture)Client.TheClient.TextureManager.GetTexture(Texture);
            }
            UICommands.Add(new QUADCMD
            {
                Position = Pos,
                Size = Size,
                Color = Color,
                Texture = Tex,
                TextBegin = TextureStart == default ? new Vector2(0, 0) : TextureStart,
                TextEnd = TextureStart == default ? new Vector2(1, 1) : TextureEnd
            });
        }

        public void RenderQuad(Vector3 Pos, Vector2i Size, Color Color)
        {
            Texture Texture = Client.TheClient.ResourcePackManager.GetUITexture("Voxel@ui/blank");
            UICommands.Add(new QUADCMD
            {
                Position = Pos,
                Size = Size,
                Color = Color,
                Texture = Texture,
                TextBegin = new Vector2(0, 0),
                TextEnd = new Vector2(1, 1)
            });
        }

        public void Render()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            #region UI Rendering
            var UICMDByTexture = UICommands.GroupBy(s => s.Texture);

            List<List<QUADCMD>> UICMDLISTS = new List<List<QUADCMD>>();
            foreach (var groupe in UICMDByTexture)
            {
                UICMDLISTS.Add(groupe.ToList());
            }
            foreach (var groupe in UICMDLISTS)
            {
                UIBatch.CurrentTexture = groupe[0].Texture;
                UIBatch.TestSize(groupe.Count * 4);
                foreach (var ui in groupe)
                {
                    Vertex[] Quad =
                    {
                        new Vertex{ Position = ui.Position ,TexCoords = new Vector2( ui.TextBegin.X,ui.TextEnd.Y),Color = ui.Color },
                        new Vertex{ Position = ui.Position + new Vector3(ui.Size.X,0,0) ,TexCoords = new Vector2(ui.TextEnd.X,ui.TextEnd.Y),Color = ui.Color },
                        new Vertex{ Position = ui.Position + new Vector3(0,ui.Size.Y,0) ,TexCoords = new Vector2(ui.TextBegin.X,ui.TextBegin.Y),Color = ui.Color },
                        new Vertex{ Position = ui.Position + new Vector3(ui.Size.X,ui.Size.Y,0) ,TexCoords = new Vector2( ui.TextEnd.X,ui.TextBegin.Y),Color = ui.Color },
                    };
                    UIBatch.AddQuad(Quad);
                }
                UIBatch.Flush();
            }
            #endregion
            #region Text Rendering
            //Text
            //Regroup text cmd by fonts
            var TextCMDByFont = TextsCommands.GroupBy(s => s.Font);
            List<List<TextCMD>> TextCMDLISTS = new List<List<TextCMD>>();
            foreach (var groupe in TextCMDByFont)
            {
                TextCMDLISTS.Add(groupe.ToList());
            }
            //Render each text cmd by fonts
            foreach (var groupe in TextCMDLISTS)
            {
                TextBatch.Font = groupe[0].Font;
                foreach (var text in groupe)
                {
                    TextBatch.AddText(text.Text, text.Position, text.Scale, text.Color);
                }
                TextBatch.FlushBatch();
            }
            #endregion
            UICommands.Clear();
            TextsCommands.Clear();
        }

        public int GetTextSize(string Font,float Scale, string Text)
        {
            Font font =Client.TheClient.ResourcePackManager.GetFont(Font);
            return (int)TextSizeCalculator.CalculateSize(font, Scale, Text);
        }

        public int GetVerticalTextSize(string Font, float Scale, string Text)
        {
            Font font = Client.TheClient.ResourcePackManager.GetFont(Font);
            return (int)TextSizeCalculator.CalculateVerticalSize(font, Scale, Text);
        }
    }
}
