using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.client.ui.prototype.renderer;
namespace VoxelPrototype.client.ui.prototype.renderer.batch
{
    internal class TextBatch
    {
        internal static string ShaderResourceID = "voxelprototype:shaders/ui";
        private int[] Indices = [
            0,1,2,
            1,2,3
        ];
        public static int BATCH_SIZE = 1600;
        public Vertex[] Vertices = new Vertex[BATCH_SIZE];
        public int Size;
        public int VAO;
        public int VBO;
        public void InitBatch()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, BATCH_SIZE * Unsafe.SizeOf<Vertex>(), nint.Zero, BufferUsageHint.DynamicDraw);
            //Generate EBO
            {

                int ElementSize = BATCH_SIZE / 4 * 6;
                int[] ElementBuffer = new int[ElementSize];
                for (int i = 0; i < ElementSize; i++)
                {
                    ElementBuffer[i] = Indices[i % 6] + i / 6 * 4;
                }
                int EBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBuffer.Length * sizeof(int), ElementBuffer, BufferUsageHint.StaticDraw);
            }
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("TexCoords"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void AddText(Font Font, string Text, Vector2i Position, int Scale, uint Color)
        {

            int xcopy = Position.X;
            Position.Y = Client.TheClient.ClientSize.Y - Position.Y;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                Character Char = Font.GetChar(c);
                if (c == '\n')
                {
                    Position.Y -= (Char.Height + 1) * Scale;
                    Position.X = xcopy;
                }
                else
                {
                    int XPos = Position.X;
                    int YPos = Position.Y - (Char.Height - Char.Ascender) * Scale;
                    if (c != ' ')
                    {
                        if (Size > BATCH_SIZE - 4)
                        {
                            FlushBatch(Font);
                        }
                        AddCharacter(XPos, YPos, Scale, Char, Color);
                    }
                    Position.X += (Char.Width + 1) * Scale;

                }
            }

        }
        public void AddCharacter(int x, int y, int Scale, Character Character, uint Color)
        {


            int index = Size;
            Vertices[index].Position = new(x + Scale * Character.Width, y + Scale * Character.Height);
            Vertices[index].TexCoords = new Vector2(Character.UV1.X, Character.UV0.Y);
            Vertices[index].Color = Color;
            index += 1;
            Vertices[index].Position = new(x, y + Scale * Character.Height);
            Vertices[index].TexCoords = Character.UV0;
            Vertices[index].Color = Color;

            index += 1;
            Vertices[index].Position = new(x + Scale * Character.Width, y);
            Vertices[index].TexCoords = Character.UV1;
            Vertices[index].Color = Color;
            index += 1;

            Vertices[index].Position = new(x, y);
            Vertices[index].TexCoords = new Vector2(Character.UV0.X, Character.UV1.Y);
            Vertices[index].Color = Color;

            Size += 4;
        }
        public void FlushBatch(Font Font)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, BATCH_SIZE * Unsafe.SizeOf<Vertex>(), nint.Zero, BufferUsageHint.DynamicDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, Unsafe.SizeOf<Vertex>() * Vertices.Length, Vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            var shader = Client.TheClient.ShaderManager.GetShader(ShaderResourceID);
            shader.Use();
            shader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0, Client.TheClient.ClientSize.X, 0, Client.TheClient.ClientSize.Y, -1, 1));
            Font.FontTexture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Size / 4 * 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            Size = 0;
        }
    }
}
