using OpenTK.Graphics.OpenGL;
using VoxelPrototype.utils;
namespace VoxelPrototype.client.Render.Components
{
    public struct Model
    {
        internal int Vao;
        internal int Vbo;
        internal float[] _Vertices;
        public Model(float[] Vertices)
        {
            _Vertices = Vertices;
            Vao = GL.GenVertexArray();
            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(Vao);
            var vertexLocation = Client.TheClient.ShaderManager.GetShader(new ResourceID("shaders/entity")).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLocation = Client.TheClient.ShaderManager.GetShader(new ResourceID("shaders/entity")).GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);
        }
        public void Clean()
        {
            GL.DeleteBuffer(Vbo);
            GL.DeleteVertexArray(Vao);
        }
    }
}
