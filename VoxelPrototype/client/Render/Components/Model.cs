using OpenTK.Graphics.OpenGL4;
namespace VoxelPrototype.client.Render.Components
{
    public struct Model
    {
        internal int Vao;
        internal float[] _Vertices;
        public Model(float[] Vertices)
        {
            _Vertices = Vertices;
            Vao = GL.GenVertexArray();
            int Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(Vao);
            var vertexLocation = ClientRessourcePackManager.GetRessourcePackManager().GetShader("Voxel@entity").GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLocation = ClientRessourcePackManager.GetRessourcePackManager().GetShader("Voxel@entity").GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);
        }
    }
}
