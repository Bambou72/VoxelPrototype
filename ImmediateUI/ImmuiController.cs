using ImmediateUI.immui;
using ImmediateUI.immui.drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ImmediateUI
{
    internal class ImmuiController
    {
        int VAO, VBO, EBO;
        int VertexBufferSize,IndexBufferSize;
        public ImmuiController()
        {
            VertexBufferSize = 10000;
            IndexBufferSize = 2000;
            GenerateGraphicObjects();
        }
        public void Update(GameWindow GW)
        {
            Context CurContext = Immui.GetContext();
            CurContext.MousePosition = (Vector2i)GW.MousePosition;
        }
        public void GenerateGraphicObjects()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("UV"));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>("Color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void Render()
        {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            var DrawData = Immui.GetDrawData();
            for (int i = 0; i < DrawData.CmdList.Count; i++)
            {
                ImmuiDrawList cmd_list = DrawData.CmdList[i];

                int vertexSize = cmd_list.VertexBuffer.Size * Unsafe.SizeOf<Vertex>();
                if (vertexSize > VertexBufferSize)
                {
                    int newSize = (int)Math.Max(VertexBufferSize * 1.5f, vertexSize);
                    GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    VertexBufferSize = newSize;
                }

                int indexSize = cmd_list.IndexBuffer.Size * sizeof(uint);
                if (indexSize > IndexBufferSize)
                {
                    int newSize = (int)Math.Max(IndexBufferSize * 1.5f, indexSize);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    IndexBufferSize = newSize;
                }
            }
            Matrix4 ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Immui.GetScreenSize().X, Immui.GetScreenSize().Y, 0,-1f,1f);
            Window.UiShader.Use();
            Window.UiShader.SetMatrix4("projection", ProjectionMatrix);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            for (int i = 0; i < DrawData.CmdList.Count; i++)
            {
                ImmuiDrawList cmd_list = DrawData.CmdList[i];
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, cmd_list.VertexBuffer.Size * Unsafe.SizeOf<Vertex>(), cmd_list.VertexBuffer.Data);
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, 0, cmd_list.IndexBuffer.Size * sizeof(uint), cmd_list.IndexBuffer.Data);
                for (int cmd_i = 0; cmd_i < cmd_list.Commands.Count; cmd_i++)
                {
                    ImmuiDrawCommand pcmd = cmd_list.Commands[cmd_i];
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, pcmd.TextureID);
                    var clip = pcmd.ClipRect;
                    GL.Scissor((int)clip.X,(int)Immui.GetScreenSize().Y  - (int)clip.YH, clip.W , clip.H);
                    GL.DrawElements(BeginMode.Triangles, (int)pcmd.Count, DrawElementsType.UnsignedInt, (int)pcmd.Offset * sizeof(uint));                                
                }
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
