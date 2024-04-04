using OpenTK.Graphics.OpenGL4;
using SharpFont;
using System;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPrototype.client.Text
{
    internal  class TextRenderer
    {
        int VAO;
        int VBO;

        internal void Init()
        {
            VAO =  GL.GenVertexArray();
            VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 24, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void RenderText(string text, float x, float y, float scale,Font font,Vector3 color)
        {
            scale = scale / font.FontSize;
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
            // activate corresponding render state	
            var shader = ClientRessourcePackManager.RessourcePackManager.GetShader("Voxel@text");
            shader.Use();
            shader.SetVector3("Color", color);
            shader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0,API.ClientAPI.WindowWidth(),0, API.ClientAPI.WindowHeight(), -1, 1));
            font.FontAtlas.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            foreach (char c in text)
            {
                Character ch = font.GetCharacter(c);

                float xpos = x + ch.Bearing.X * scale;
                float ypos = y - (ch.Size.Y - ch.Bearing.Y) * scale;

                float w = ch.Size.X * scale;
                float h = ch.Size.Y * scale;
                // update VBO for each character
                float[] vertices = new float[24]{
                     xpos,     ypos + h, ch.AtlasStart.X, ch.AtlasStart.Y ,            
            xpos,     ypos,       ch.AtlasStart.X,ch.AtlasEnd.Y,
             xpos + w, ypos,       ch.AtlasEnd.X,ch.AtlasEnd.Y ,

            xpos,     ypos + h,   ch.AtlasStart.X, ch.AtlasStart.Y,
            xpos + w, ypos,       ch.AtlasEnd.X, ch.AtlasEnd.Y ,
            xpos + w, ypos + h,   ch.AtlasEnd.X,  ch.AtlasStart.Y
                };
                // render glyph texture over quad
                // update content of VBO memory
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * vertices.Length, vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                // render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                // now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                x += (ch.Advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64)
            }
            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Blend);

        }
    }
}
