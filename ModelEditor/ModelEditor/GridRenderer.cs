using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace ModelEditor.ModelEditor
{
    internal static class GridRenderer
    {
        static int VAO;
        static int VBO;
        static string VertShader = @"
                    #version 330 core
                    layout (location = 0) in vec3 aPosition;
                    uniform mat4 model;
                    uniform mat4 view;
                    uniform mat4 projection;
                    void main()
                    {
                        gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                    }
                    ";
        static string FragShader = @"
                    #version 330 core
                    out vec4 FragColor;
                    void main()
                    {
                        FragColor = vec4(0.2, 0.2, 0.2, 1.0);
                    }
                    ";
        private static Shader shader = new Shader(VertShader, FragShader);
        internal static void GenerateGridVertices()
        {
            float[] vertices = new float[80 * 3]; // 80 points, 3 coordonnées par point (x, y, z)
            int index = 0;
            for (float i = -5.0f; i <= 5.0f; i += 1.0f)
            {
                // Lignes horizontales
                vertices[index++] = -5.0f;
                vertices[index++] = 0.0f; // Y reste constant pour les lignes horizontales
                vertices[index++] = i;
                vertices[index++] = 5.0f;
                vertices[index++] = 0.0f;
                vertices[index++] = i;
                // Lignes verticales
                vertices[index++] = i;
                vertices[index++] = 0.0f;
                vertices[index++] = -5.0f;
                vertices[index++] = i;
                vertices[index++] = 0.0f;
                vertices[index++] = 5.0f;
            }
            // Création du Vertex Array Object (VAO)
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            // Création du Vertex Buffer Object (VBO)
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            // Configuration de l'attribut de position
            int positionLocation = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);
        }
        internal static void Render(Matrix4 View, Matrix4 Projection)
        {
            shader.Use();
            shader.SetMatrix4("view", View);
            shader.SetMatrix4("projection", Projection);
            shader.SetMatrix4("model", Matrix4.CreateTranslation(0, 0, 0));
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(2.0f);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Lines, 0, 80);
            GL.BindVertexArray(0);
            GL.Disable(EnableCap.LineSmooth);
        }
    }
}
