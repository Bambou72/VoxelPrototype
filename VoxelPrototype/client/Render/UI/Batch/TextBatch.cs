using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Text;

namespace VoxelPrototype.client.Render.UI.Batch
{
    internal class TextBatch
    {
        private int[] Indices = new int[6]
        {
            0,1,3,
            1,2,3
        };
        public static int BATCH_SIZE = 500;
        public static int VERTEX_SIZE = 8;
        public float[] Vertices = new float[BATCH_SIZE * VERTEX_SIZE];
        public int Size;
        public int VAO;
        public int VBO;
        public Font Font;
        public void GenerateEBO()
        {
            int ElementSize = BATCH_SIZE * 3;
            int[] ElementBuffer = new int[ElementSize];
            for (int i = 0; i < ElementSize; i++)
            {
                ElementBuffer[i] = Indices[i % 6] + i / 6 * 4;
            }
            int EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBuffer.Length * sizeof(int), ElementBuffer, BufferUsageHint.StaticDraw);
        }
        public void InitBatch()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * BATCH_SIZE * VERTEX_SIZE, nint.Zero, BufferUsageHint.DynamicDraw);
            GenerateEBO();
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, VERTEX_SIZE * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE * sizeof(float), 6 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void AddText(string Text, Vector2 Position, float Scale, Vector4 Color)
        {
            Scale = Scale / Font.FontSize;
            float xcopy = Position.X;
            Position.Y = Client.TheClient.ClientSize.Y - Position.Y;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                Character Char = Font.GetCharacter(c);
                if (c == '\n')
                {
                    Position.Y -= Char.Size.Y * 1.3f * Scale;
                    Position.X = xcopy;
                }
                else
                {
                    float XPos = Position.X + Char.Bearing.X * Scale;
                    float YPos = Position.Y - (Char.Size.Y - Char.Bearing.Y) * Scale;
                    if (c != ' ')
                    {
                        AddCharacter(XPos, YPos, Scale, Char, Color);
                    }
                    Position.X += (Char.Advance >> 6) * Scale;
                }
            }
        }
        public void AddCharacter(float x, float y, float Scale, Character Character, Vector4 Color)
        {
            if (Size > BATCH_SIZE - 4)
            {
                FlushBatch();
            }
            float r = Color.X;
            float g = Color.Y;
            float b = Color.Z;
            float a = Color.W;
            float x0 = x;
            float y0 = y;
            float x1 = x + Scale * Character.Size.X;
            float y1 = y + Scale * Character.Size.Y;
            float ux0 = Character.AtlasStart.X;
            float uy0 = Character.AtlasStart.Y;
            float ux1 = Character.AtlasEnd.X;
            float uy1 = Character.AtlasEnd.Y;
            int index = Size * VERTEX_SIZE;
            Vertices[index] = x1; Vertices[index + 1] = y0;
            Vertices[index + 2] = r; Vertices[index + 3] = g; Vertices[index + 4] = b; Vertices[index + 5] = a;
            Vertices[index + 6] = ux1; Vertices[index + 7] = uy1;
            index += VERTEX_SIZE;
            Vertices[index] = x1; Vertices[index + 1] = y1; 
            Vertices[index + 2] = r; Vertices[index + 3] = g; Vertices[index + 4] = b; Vertices[index + 5] = a;
            Vertices[index + 6] = ux1; Vertices[index + 7] = uy0;

            index += VERTEX_SIZE;
            Vertices[index] = x0; Vertices[index + 1] = y1;
            Vertices[index + 2] = r; Vertices[index + 3] = g; Vertices[index + 4] = b; Vertices[index + 5] = b;
            Vertices[index + 6] = ux0; Vertices[index + 7] = uy0;

            index += VERTEX_SIZE;
            Vertices[index] = x0; Vertices[index + 1] = y0;
            Vertices[index + 2] = r; Vertices[index + 3] = g; Vertices[index + 4] = b; Vertices[index + 5] = b;
            Vertices[index + 6] = ux0; Vertices[index + 7] = uy1;
            Size += 4;
        }
        public void FlushBatch()
        {
            if(Size >0)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * BATCH_SIZE * VERTEX_SIZE, nint.Zero, BufferUsageHint.DynamicDraw);
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * Vertices.Length, Vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                var shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/text"));
                shader.Use();
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                shader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0, ClientAPI.WindowWidth(), 0, ClientAPI.WindowHeight(), 0, 100));
                Font.FontAtlas.Use(TextureUnit.Texture0);
                GL.BindVertexArray(VAO);
                GL.DrawElements(PrimitiveType.Triangles, Size * 6, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
                Size = 0;
                Vertices = new float[BATCH_SIZE * VERTEX_SIZE];
            }
        }
    }
}
