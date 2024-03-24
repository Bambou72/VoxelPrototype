using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace VoxelPrototype.client.Render.Components
{
    internal struct DebugBox
    {
        public int VAO, VBO, EBO;
        public Vector4 Color;
        private void GenVAO(float[] vertices, uint[] indices)
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count() * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * 24, indices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
        }
        private (float[], uint[]) GenVertices(Vector3d BoxSize)
        {
            float[] vertices = new float[]
            {
                        0,0, 0,
                        (float)BoxSize.X,0,0,
                        (float)BoxSize.X,(float)BoxSize.Y,0,
                        0,(float)BoxSize.Y,0,
                        0,0,(float)BoxSize.Z,
                        (float)BoxSize.X,0,(float)BoxSize.Z,
                        (float)BoxSize.X,(float)BoxSize.Y,(float)BoxSize.Z,
                        0,(float)BoxSize.Y,(float)BoxSize.Z
            };
            uint[] indices = new uint[] {
                        0, 1, 1, 2, 2, 3, 3, 0, // Lignes du plan z = aabb.Min.Z
                        4, 5, 5, 6, 6, 7, 7, 4, // Lignes du plan z = aabb.Max.Z
                        0, 4, 1, 5, 2, 6, 3, 7  // Lignes entre les plans
                    };
            return (vertices, indices);
        }
        private void CreateMesh(Vector3d BoxSize)
        {
            (float[] vertices, uint[] indices) = GenVertices(BoxSize);
            GenVAO(vertices, indices);
        }
        public DebugBox(Vector3d BoxSize, Vector4 color)
        {
            Color = color;
            CreateMesh(BoxSize);
        }
    }
}
