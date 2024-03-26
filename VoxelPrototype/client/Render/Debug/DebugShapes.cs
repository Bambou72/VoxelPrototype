using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace VoxelPrototype.client.Render.Debug
{
    public struct DebugBox
    {
        public Vector3 Position;
        public Vector3d Size;
        public Quaternion Rotation;
        public Vector4 Color;
    }
    public struct DebugLine
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector4 Color;
        public Vector3 Position;
    }
    public struct Frustum
    {
        public int VAO, VBO, EBO;
        public int count;
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
        private (float[], uint[]) GenVertices(Vector3[] corners)
        {
            float[] vertices = new float[]
            {
                        corners[0].X,corners[0].Y, corners[0].Z,
                        corners[1].X,corners[1].Y, corners[1].Z,
                        corners[2].X,corners[2].Y, corners[2].Z,
                        corners[3].X,corners[3].Y, corners[3].Z,
                        corners[4].X,corners[4].Y, corners[4].Z,
                        corners[5].X,corners[5].Y, corners[5].Z,
                        corners[6].X,corners[6].Y, corners[6].Z,
                        corners[7].X,corners[7].Y, corners[7].Z,
            };
            uint[] indices = new uint[] {
                        0, 1, 1, 2, 2, 3, 3, 0, // Lignes du plan z = aabb.Min.Z
                        4, 5, 5, 6, 6, 7, 7, 4, // Lignes du plan z = aabb.Max.Z
                        0, 4, 1, 5, 2, 6, 3, 7  // Lignes entre les plans
                    };
            return (vertices, indices);
        }
        private void CreateMesh(Vector3[] corners)
        {
            (float[] vertices, uint[] indices) = GenVertices(corners);
            GenVAO(vertices, indices);
        }
        public void Delete()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }
        public Vector4 color;
        public Frustum(Vector3[] corners, Vector4 Color)
        {
            CreateMesh(corners);
            color = Color;
        }
    }
}
