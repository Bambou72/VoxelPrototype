using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoxelPrototype.client.rendering.texture;
using VoxelPrototype.client.ui.prototype.renderer;
namespace VoxelPrototype.client.ui.prototype.renderer.batch
{
    internal class UIBatch
    {
        private static uint[] Indices = [
            0,1,2,
            1,2,3
        ];
        public int BATCH_SIZE = 4;
        public int VAO;
        public int VBO;
        public int EBO = GL.GenBuffer();
        public UIBatch()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, BATCH_SIZE * Unsafe.SizeOf<Vertex>(), nint.Zero, BufferUsageHint.DynamicDraw);
            //
            GL.BindVertexArray(VAO);
            int ElementSize = BATCH_SIZE / 4 * 6;
            uint[] ElementBuffer = new uint[ElementSize];
            for (int i = 0; i < ElementSize; i++)
            {
                ElementBuffer[i] = (uint)(Indices[i % 6] + i / 6 * 4);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ElementBuffer.Length * sizeof(uint), ElementBuffer, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
            //
            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("TexCoords"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void RenderPolygone(Vertex[] vertices, Texture Texture)
        {
            //FLUSH
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, Unsafe.SizeOf<Vertex>() * vertices.Length, vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            var shader = Client.TheClient.ShaderManager.GetShader("voxelprototype:shaders/ui");
            shader.Use();
            shader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0, Client.TheClient.ClientSize.X, Client.TheClient.ClientSize.Y, 0, -1, 1));
            Texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, vertices.Length / 4 * 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
