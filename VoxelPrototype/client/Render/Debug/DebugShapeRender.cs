using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.client.Render.Debug
{
    public static class DebugShapeRenderer
    {
        static Shader DebugShader = new(DebugShaders.DebugVert, DebugShaders.DebugFrag, true);
        static Shader DebugLineShader = new(DebugShaders.DebugLineVert, DebugShaders.DebugLineFrag, true);
        internal static void RenderDebug(float[] vertices, uint[] indices, Matrix4 Model, Vector4 Color)
        {
            int CVAO = GL.GenVertexArray();
            DebugShader.SetMatrix4("model", Model);
            DebugShader.SetVector4("colors", Color);
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();
            GL.BindVertexArray(CVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count() * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(CVAO);
            GL.DrawElements(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(CVAO);
        }
        internal static void RenderDebugLine(float[] vertices, Vector3 Position)
        {
            int CVAO = GL.GenVertexArray();
            GL.BindVertexArray(CVAO);
            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            DebugLineShader.SetMatrix4("model", Matrix4.CreateTranslation(Position));
            DebugLineShader.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length);
            GL.BindVertexArray(0);
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(CVAO);
        }
        internal static void InternalRenderDebugBox(DebugBox Box)
        {
            float[] vertices = new float[]
            {
                        0,0,0,
                        (float)(Box.Size.X),0, 0,
                        (float)(Box.Size.X),(float)(Box.Size.Y), 0,
                        0,(float)(Box.Size.Y), 0,
                        0,0,(float)(Box.Size.Z),
                        (float)(Box.Size.X),0,(float)(Box.Size.Z),
                        (float)(Box.Size.X),(float)(Box.Size.Y),(float)(Box.Size.Z),
                        0,(float)(Box.Size.Y),(float)(Box.Size.Z)
            };
            uint[] indices = new uint[] {
                        0, 1, 1, 2, 2, 3, 3, 0, // Lignes du plan z = aabb.Min.Z
                        4, 5, 5, 6, 6, 7, 7, 4, // Lignes du plan z = aabb.Max.Z
                        0, 4, 1, 5, 2, 6, 3, 7  // Lignes entre les plans
                };
            RenderDebug(vertices, indices, Matrix4.CreateFromQuaternion(Box.Rotation) * Matrix4.CreateTranslation(Box.Position), Box.Color);
        }
        internal static void Render(Matrix4 View, Matrix4 Projection)
        {
            DebugShader.Use();
            DebugShader.SetMatrix4("view", View);
            DebugShader.SetMatrix4("projection", Projection);
            DebugLineShader.SetMatrix4("view", View);
            DebugLineShader.SetMatrix4("projection", Projection);
            //Render DebugBoxes
            int BoxCount = DebugBoxes.Count;
            for (int i = 0; i < BoxCount; i++)
            {
                DebugBox box = DebugBoxes.Pop();
                InternalRenderDebugBox(box);
            }
            /*
            //Render DebugFrustums
            DebugBoxShader.Use();
            DebugBoxShader.SetMatrix4("view", Luxon.Camera ? Luxon.MainCamera.GetViewMatrix() : Luxon.View);
            DebugBoxShader.SetMatrix4("projection", Luxon.Camera ? Luxon.MainCamera.GetProjectionMatrix() : Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70f), Luxon.Aspect, 0.1f, 100));
            int FrustumCount = DebugFrustums.Count;
            for (int i = 0; i < FrustumCount; i++)
            {
                Frustum frust = DebugFrustums.Pop();
                DebugBoxShader.SetMatrix4("model", Matrix4.Identity);
                DebugBoxShader.SetVector4("colors", frust.color);
                GL.BindVertexArray(frust.VAO);
                GL.DrawElements(PrimitiveType.Lines, 24, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                frust.Delete();
            }*/
            //Render DebugLines
            List<float> Lines = new();
            while (DebugLines.Count > 0)
            {
                DebugLine debugline = DebugLines.Pop();
                float[] vertices = new float[]
                {
                    debugline.Start.X, debugline.Start.Y, debugline.Start.Z, debugline.Color.X,debugline.Color.Y,debugline.Color.Z,debugline.Color.W,
                    debugline.End.X, debugline.End.Y, debugline.End.Z,debugline.Color.X, debugline.Color.Y,debugline.Color.Z, debugline.Color.W
                };
                Lines.AddRange(vertices.ToList());
            }
            //RenderDebugLine(vertices, debugline.Position);
            RenderDebugLine(Lines.ToArray(), Vector3.Zero);
        }
        //
        //DEBUGBOX
        //
        static Stack<DebugBox> DebugBoxes = new();
        public static void RenderDebugBox(DebugBox box)
        {
            // Envoi des matrices view et projection au shader
            DebugBoxes.Push(box);
        }
        //
        //DEBUGFrustum
        //
        static Stack<Frustum> DebugFrustums = new();
        public static void RenderDebugFrustums(Frustum frustum)
        {
            // Envoi des matrices view et projection au shader
            DebugFrustums.Push(frustum);
        }
        //
        //DEBUG LINE
        //
        static Stack<DebugLine> DebugLines = new();
        public static void RenderLine(Vector3 pos1, Vector3 pos2, Vector4 color, Vector3 Position)
        {
            DebugLine debugLine = new DebugLine() { Start = pos1, End = pos2, Color = color, Position = Position };
            DebugLines.Push(debugLine);
        }
        //
        //DEBUG Axes
        //
        public static void RenderAxe(Quaternion Rotation, Vector3 Position)
        {
            RenderLine(Position, Position + Vector3.Transform(Vector3.UnitX, Rotation) * 0.0625f, new Vector4(1, 0, 0, 1), Vector3.Zero);
            RenderLine(Position, Position + Vector3.Transform(Vector3.UnitY, Rotation) * 0.0625f, new Vector4(0, 1, 0, 1), Vector3.Zero);
            RenderLine(Position, Position + Vector3.Transform(Vector3.UnitZ, Rotation) * 0.0625f, new Vector4(0, 0, 1, 1), Vector3.Zero);
        }
    }
    public static class DebugShaders
    {
        public static string DebugVert = @"
            #version 330 core
            uniform mat4 view;
            uniform mat4 projection;
            uniform mat4 model;
            uniform vec4 colors;
            layout (location = 0) in vec3 position;
            out vec4 Color;
            void main()
            {
                Color = colors;
                gl_Position = vec4(position, 1.0f) * model* view * projection;
            }";
        public static string DebugFrag = @"
            #version 330 core
            layout (location = 0) out vec4 FragColor;
            layout (location = 1) out vec4 BrightColor;
            in vec4 Color;
            void main()
            {
                FragColor = Color;
                BrightColor = vec4(0,0,0,0);
            }";
        public static string DebugLineVert = @"
            #version 330 core
            uniform mat4 view;
            uniform mat4 projection;
            uniform mat4 model;
            layout (location = 0) in vec3 position;
            layout (location = 1) in vec4 color;
            out vec4 Color;
            void main()
            {
                Color = color;
                gl_Position = vec4(position, 1.0f) * model* view * projection ;
            }";
        public static string DebugLineFrag = @"
            #version 330 core
            in vec4 Color;
            layout (location = 0) out vec4 FragColor;
            layout (location = 1) out vec4 BrightColor;
            void main()
            {
                FragColor = Color;
                BrightColor = vec4(0,0,0,0);
            }";
    }
}
