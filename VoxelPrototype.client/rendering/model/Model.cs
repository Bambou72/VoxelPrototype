using OpenTK.Graphics.OpenGL4;
using VoxelPrototype.utils;
namespace VoxelPrototype.client.rendering.model
{
    public struct Model
    {
        internal int Vao;
        internal int Vbo;
        internal float[] _Vertices;
        static string ModelShaderResourceID = "voxelprototype:shaders/entity";
        public Model(float[] Vertices)
        {
            _Vertices = Vertices;
            Vao = GL.GenVertexArray();
            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(Vao);
            var vertexLocation = Client.TheClient.ShaderManager.GetShader(ModelShaderResourceID).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLocation = Client.TheClient.ShaderManager.GetShader(ModelShaderResourceID).GetAttribLocation("aTexCoord");
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
