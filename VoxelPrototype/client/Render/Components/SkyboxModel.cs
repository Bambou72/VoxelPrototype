using OpenTK.Graphics.OpenGL4;
namespace VoxelPrototype.client.Render.Components
{
    internal struct SkyBoxModel
    {
        internal int Vao;
        internal float[] _Vertices;
        public SkyBoxModel(float[] Vertices)
        {
            _Vertices = Vertices;
            Vao = GL.GenVertexArray();
            int Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(Vao);
            var vertexLocation = ClientRessourcePackManager.GetRessourcePackManager().GetShader("Voxel@Shaders/Cubemap").GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }
    }
}
