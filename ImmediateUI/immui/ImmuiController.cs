using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ImmediateUI.immui
{
    internal class ImmuiController
    {
        int VAO, VBO, EBO;
        int VertexBufferSize, IndexBufferSize;
        public ImmuiController()
        {
            VertexBufferSize = 10000;
            IndexBufferSize = 2000;
            GenerateGraphicObjects();
        }
        public void Update(Context Ctx, GameWindow GW)
        {
            Ctx.MousePosition = (Vector2i)GW.MousePosition;
            Ctx.ScrollDelta = (Vector2i)GW.MouseState.ScrollDelta;
        }
        public void GenerateGraphicObjects()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexBufferSize, nint.Zero, BufferUsageHint.DynamicDraw);
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexBufferSize, nint.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("UV"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void Prepare(Renderer renderer)
        {
            int vertexSize = renderer.VertexBuffer.Size * Unsafe.SizeOf<Vertex>();
            if (vertexSize > VertexBufferSize)
            {
                int newSize = (int)Math.Max(VertexBufferSize * 1.5f, vertexSize);
                GL.BufferData(BufferTarget.ArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw);
                VertexBufferSize = newSize;
            }

            int indexSize = renderer.IndexBuffer.Size * sizeof(uint);
            if (indexSize > IndexBufferSize)
            {
                int newSize = (int)Math.Max(IndexBufferSize * 1.5f, indexSize);
                GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw);
                IndexBufferSize = newSize;
            }

        }
        public void Setup(Context Ctx)
        {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            Matrix4 ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Ctx.GetScreenSize().X, Ctx.GetScreenSize().Y, 0, -1, 10f);
            Window.UiShader.Use();
            Window.UiShader.SetMatrix4("projection", ProjectionMatrix);
        }
        public void ResetRenderState()
        {
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.CullFace);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DepthFunc(DepthFunction.Less);


        }
        public void Render(Context Ctx)
        {
            Setup(Ctx);
            var renderer = Ctx.GetRenderer();
            Prepare(renderer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, renderer.VertexBuffer.Size * Unsafe.SizeOf<Vertex>(), renderer.VertexBuffer.Data);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, 0, renderer.IndexBuffer.Size * sizeof(uint), renderer.IndexBuffer.Data);
            for (int CmdIndex = 0; CmdIndex < renderer.Commands.Count; CmdIndex++)
            {
                Command cmd = renderer.Commands[CmdIndex];
                if (cmd.Count != 0)
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, cmd.TextureID);
                    var clip = cmd.ClipRect;
                    GL.Scissor(clip.X, (int)Ctx.GetScreenSize().Y - (clip.Y + clip.H), clip.W, clip.H);
                    GL.DrawElements(BeginMode.Triangles, cmd.Count, DrawElementsType.UnsignedInt, cmd.Offset * sizeof(uint));
                }
            }
            ResetRenderState();

        }
    }
}
