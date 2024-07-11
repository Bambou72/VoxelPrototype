using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace ModelEditor.ModelEditor
{
    internal static class ModelRenderer
    {
        static int fbo;
        static int vao;
        static int ebo;
        static int vbo;
        static int rbo;
        internal static int texture;
        internal static ArcBallCamera Camera = new();
        static string VertShader = @"
                    #version 460 core
                    //Set vertices data
                    layout(location = 0) in vec3 Vertex;
                    layout(location = 1) in vec2 Texture;
                    layout(location = 2) in float Selected;
                    out vec2 TextureCoordinates;
                    out float selected;
                    uniform mat4 model;
                    uniform mat4 view;
                    uniform mat4 projection;
                    void main(void)
                    {
                        TextureCoordinates = Texture;
                        selected = Selected;
                        gl_Position = vec4(Vertex, 1.0) * model * view * projection;
                    }
                    ";
        static string FragShader = @"
                    #version 460 
                    in vec2 TextureCoordinates;
                    in float selected;
                    out vec4 outputColor;
                    uniform sampler2D TextureAtlas;
                    void main()
                    {
                        if(int(selected) == 1)
                        {
                            outputColor = mix(texture(TextureAtlas,TextureCoordinates) , vec4(0.5,0,0,1),0.5);
                        }else
                        {
                            outputColor = texture(TextureAtlas,TextureCoordinates);
                        }
                        if(texture(TextureAtlas,TextureCoordinates).a == 0.0)
                        {
                            discard;
                        }
                    }
                    ";
        private static Shader shader = new Shader(VertShader, FragShader);
        internal static float[] BlockMesh = new float[1];
        internal static uint[] BlockIndices = new uint[1];
        internal static void Initialize()
        {
            GL.Enable(EnableCap.DepthTest);
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1920, 1080, 0, PixelFormat.Rgb, PixelType.UnsignedByte, nint.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, 1920, 1080);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GridRenderer.GenerateGridVertices();
        }
        internal static void Render()
        {
            Matrix4 Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)16 / 9, 0.1f, 100.0f);
            Matrix4 View = Camera.GetViewMatrix();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, BlockMesh.Length * sizeof(float), BlockMesh, BufferUsageHint.StaticDraw);
            var vertexLocation = shader.GetAttribLocation("Vertex");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var texCoordLocation = shader.GetAttribLocation("Texture");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            var SelLocation = shader.GetAttribLocation("Selected");
            GL.EnableVertexAttribArray(SelLocation);
            GL.VertexAttribPointer(SelLocation, 1, VertexAttribPointerType.Float, false, 6 * sizeof(float), 5 * sizeof(float));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, BlockIndices.Length * sizeof(uint), BlockIndices, BufferUsageHint.StaticDraw);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // we're not using the stencil buffer now
            GridRenderer.Render(View, Projection);
            shader.Use();
            GL.BindVertexArray(vao);
            //TextureManager.VoxelAtlas.Use(TextureUnit.Texture0);
            shader.SetMatrix4("view", View);
            shader.SetMatrix4("projection", Projection);
            shader.SetMatrix4("model", Matrix4.CreateTranslation(0, 0, 0));
            GL.DrawElements(PrimitiveType.Triangles, BlockIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        private static void RenderGrid()
        {
        }
        private static Vector3 get_arcball_vector(int x, int y)
        {
            Vector3 P = new Vector3(1.0f * x / 1280f * 2 - 1.0f, 1.0f * y / 720f * 2 - 1.0f, 0);
            P.Y = -P.Y;
            float OP_squared = P.X * P.X + P.Y * P.Y;
            if (OP_squared <= 1 * 1)
                P.Z = (float)Math.Sqrt(1 * 1 - OP_squared);  // Pythagoras
            else
                P = Vector3.Normalize(P);  // nearest point
            return P;
        }
    }
}
