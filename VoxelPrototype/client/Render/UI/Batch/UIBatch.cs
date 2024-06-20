using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.client.Render.UI.Batch
{
    internal class UIBatch
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private int[] Indices = {
            0,1,2,
            1,2,3
        };
        public int BATCH_SIZE = 100;
        public Vertex[] Vertices;
        public int Pos;
        public int VAO;
        public int VBO;
        public int EBO = GL.GenBuffer();
        public Texture CurrentTexture;
        public UIBatch()
        {
            Vertices = new Vertex[BATCH_SIZE];
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, BATCH_SIZE * Unsafe.SizeOf<Vertex>(), nint.Zero, BufferUsageHint.DynamicDraw);
            GenerateEBO();
            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("TexCoords"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);
        }
        public void AddQuad(Vertex[] vertices)
        {
            if (Pos > BATCH_SIZE)
            {
                throw new Exception("UI to render to high");
            }
            for (int i = 0; i < 4; i++)
            {
                Vertices[Pos + i] = vertices[i];
            }
            Pos += 4;
        }
        public void GenerateEBO()
        {
            GL.BindVertexArray(VAO);
            int ElementSize = BATCH_SIZE * 3;
            int[] ElementBuffer = new int[ElementSize];
            for (int i = 0; i < ElementSize; i++)
            {
                ElementBuffer[i] = Indices[i % 6] + i / 6 * 4;
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBuffer.Length * sizeof(int), ElementBuffer, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
        }
        /*
        public void TestSize(int VertexSize)
        {
            if (VertexSize > BATCH_SIZE)
            {
                int NewSize = (int)Math.Max(BATCH_SIZE * 1.5f, VertexSize);
                Resize(NewSize);
            }
        }*/
        /*
        public void Resize(int Size)
        {
            Logger.Info($"Resize UI batch to a size of {Size} vertices");
            BATCH_SIZE = Size;
            Vertices = new Vertex[BATCH_SIZE];
            GL.BufferData(BufferTarget.ArrayBuffer, BATCH_SIZE * Unsafe.SizeOf<Vertex>(), nint.Zero, BufferUsageHint.DynamicDraw);
            GenerateEBO();
        }*/
        public void Flush()
        {
            if(Pos>0)
            {

                if(CurrentTexture !=null)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, 0, Unsafe.SizeOf<Vertex>() * Vertices.Length, Vertices);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    var shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/ui"));
                    shader.Use();
                    shader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0, ClientAPI.WindowWidth(), ClientAPI.WindowHeight(), 0, 0, 100));
                    CurrentTexture.Use(TextureUnit.Texture0);
                    GL.BindVertexArray(VAO);
                    GL.DrawElements(PrimitiveType.Triangles, Pos * 6, DrawElementsType.UnsignedInt, 0);
                    GL.BindVertexArray(0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    Pos = 0;
                    Vertices = new Vertex[BATCH_SIZE];

                }
            }
        }
    }
}
