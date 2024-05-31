using Newtonsoft.Json.Bson;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.client.Render.World
{
    internal class SubMesh : IDestroyable
    {
        int VAO, VBO, EBO;
        int verticesCount = new();
        int indicesCount = new();
        public SubMesh()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            //VBO config
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<SectionVertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<SectionVertex>(), Marshal.OffsetOf<SectionVertex>("Uv"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.Int, Unsafe.SizeOf<SectionVertex>(), Marshal.OffsetOf<SectionVertex>("AO"));
            //EBO config
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
        }
        public int GetVAO()
        {
            return VAO;
        }
        public int GetVerticesCount()
        {
            return verticesCount;
        }
        public int GetIndicesCount()
        {
            return indicesCount;
        }
        public void SetupData(List<SectionVertex> vertices, List<uint> indices)
        {
            verticesCount = vertices.Count;
            indicesCount = indices.Count;
            //VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Marshal.SizeOf<SectionVertex>(), vertices.ToArray(), BufferUsageHint.StaticDraw);
            //EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            //Clean
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Destroy()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
        }
    }
}
