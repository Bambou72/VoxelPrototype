using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Text;
using VoxelPrototype.utils;

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
            int index = Size * VERTEX_SIZE;
            Vertices[index] = x + Scale * Character.Size.X; Vertices[index + 1] = y;
            Vertices[index + 2] = Color.X; Vertices[index + 3] = Color.Y; Vertices[index + 4] = Color.Z; Vertices[index + 5] = Color.W;
            Vertices[index + 6] = Character.AtlasEnd.X; Vertices[index + 7] = Character.AtlasEnd.Y;
            index += VERTEX_SIZE;
            Vertices[index] = x + Scale * Character.Size.X; Vertices[index + 1] = y + Scale * Character.Size.Y; 
            Vertices[index + 2] = Color.X; Vertices[index + 3] = Color.Y; Vertices[index + 4] = Color.Z; Vertices[index + 5] = Color.W;
            Vertices[index + 6] = Character.AtlasEnd.X; Vertices[index + 7] = Character.AtlasStart.Y;

            index += VERTEX_SIZE;
            Vertices[index] = x; Vertices[index + 1] = y + Scale * Character.Size.Y;
            Vertices[index + 2] = Color.X; Vertices[index + 3] = Color.Y; Vertices[index + 4] = Color.Z; Vertices[index + 5] = Color.W;
            Vertices[index + 6] = Character.AtlasStart.X; ; Vertices[index + 7] = Character.AtlasStart.Y;

            index += VERTEX_SIZE;
            Vertices[index] = x; Vertices[index + 1] = y;
            Vertices[index + 2] = Color.X; Vertices[index + 3] = Color.Y; Vertices[index + 4] = Color.Z; Vertices[index + 5] = Color.W;
            Vertices[index + 6] = Character.AtlasStart.X; ; Vertices[index + 7] = Character.AtlasEnd.Y;
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
                var shader = Client.TheClient.ShaderManager.GetShader(new ResourceID("shaders/text"));
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
