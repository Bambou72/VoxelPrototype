﻿/**
 * Dear Imgui Implemtation for Opentk
 * Author NogginBops
 * */
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.CompilerServices;
namespace VoxelPrototype.client
{
    public class ImGuiController : IDisposable
    {
        private bool _frameBegun;
        private int _vertexArray;
        private int _vertexBuffer;
        private int _vertexBufferSize;
        private int _indexBuffer;
        private int _indexBufferSize;
        private int _fontTexture;
        private int _shader;
        private int _shaderFontTextureLocation;
        private int _shaderProjectionMatrixLocation;
        private int _windowWidth;
        private int _windowHeight;
        private Vector2 _scaleFactor = Vector2.One;
        private static bool KHRDebugAvailable = false;
        private int GLVersion;
        private bool CompatibilityProfile;
        /// <summary>
        /// Constructs a new ImGuiController.
        /// </summary>
        public ImGuiController(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
            int major = GL.GetInteger(GetPName.MajorVersion);
            int minor = GL.GetInteger(GetPName.MinorVersion);
            GLVersion = major * 100 + minor * 10;
            KHRDebugAvailable = major == 4 && minor >= 3 || IsExtensionSupported("KHR_debug");
            CompatibilityProfile = (GL.GetInteger((GetPName)All.ContextProfileMask) & (int)All.ContextCompatibilityProfileBit) != 0;
            nint context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset | ImGuiBackendFlags.PlatformHasViewports;
            io.Fonts.Build();
            CreateDeviceResources();
            SetPerFrameImGuiData(1f / 60f);
            ImGui.NewFrame();
            ImGui.GetMainViewport();
            _frameBegun = true;
        }
        public void WindowResized(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }
        public void DestroyDeviceObjects()
        {
            Dispose();
        }
        public void CreateDeviceResources()
        {
            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            _vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArray);
            LabelObject(ObjectLabelIdentifier.VertexArray, _vertexArray, "ImGui");
            _vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, _vertexBuffer, "VBO: ImGui");
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, nint.Zero, BufferUsageHint.DynamicDraw);
            _indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, _indexBuffer, "EBO: ImGui");
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, nint.Zero, BufferUsageHint.DynamicDraw);
            RecreateFontDeviceTexture();
            string VertexSource = @"#version 330 core
uniform mat4 projection_matrix;
layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;
out vec4 color;
out vec2 texCoord;
void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
            string FragmentSource = @"#version 330 core
uniform sampler2D in_fontTexture;
in vec4 color;
in vec2 texCoord;
out vec4 outputColor;
void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";
            _shader = CreateProgram("ImGui", VertexSource, FragmentSource);
            _shaderProjectionMatrixLocation = GL.GetUniformLocation(_shader, "projection_matrix");
            _shaderFontTextureLocation = GL.GetUniformLocation(_shader, "in_fontTexture");
            int stride = Unsafe.SizeOf<ImDrawVert>();
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.BindVertexArray(prevVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            CheckGLError("End of ImGui setup");
        }
        /// <summary>
        /// Recreates the device texture used to render text.
        /// </summary>
        public void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out nint pixels, out int width, out int height, out int bytesPerPixel);
            int mips = (int)Math.Floor(Math.Log(Math.Max(width, height), 2));
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
            _fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
            GL.TexStorage2D(TextureTarget2d.Texture2D, mips, SizedInternalFormat.Rgba8, width, height);
            LabelObject(ObjectLabelIdentifier.Texture, _fontTexture, "ImGui Text Atlas");
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            // Restore state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            io.Fonts.SetTexID(_fontTexture);
            io.Fonts.ClearTexData();
        }
        /// <summary>
        /// Renders the ImGui draw list data.
        /// </summary>
        public void Render()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }
        /// <summary>
        /// Updates ImGui InputSystem and IO configuration state.
        /// </summary>
        public void Update(GameWindow wnd, double deltaSeconds)
        {
            if (_frameBegun)
            {
                ImGui.Render();
            }
            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInputSystem(wnd);
            _frameBegun = true;
            ImGui.NewFrame();
        }
        /// <summary>
        /// Sets per-frame data based on the associated window.
        /// This is called by Update(float).
        /// </summary>
        private void SetPerFrameImGuiData(double deltaSeconds)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                _windowWidth / _scaleFactor.X,
                _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(_scaleFactor.X, _scaleFactor.Y);
            io.DeltaTime = (float)deltaSeconds; // DeltaTime is in seconds.
        }
        readonly List<char> PressedChars = new List<char>();
        private ImGuiKey TryMapKey(Keys key)
        {
            ImGuiKey result = ImGuiKey.None;
            switch (key)
            {
                case Keys.Left:
                    result = ImGuiKey.LeftArrow;
                    break;
                case Keys.Up:
                    result = ImGuiKey.UpArrow;
                    break;
                case Keys.Right:
                    result = ImGuiKey.RightArrow;
                    break;
                case Keys.Down:
                    result = ImGuiKey.DownArrow;
                    break;
                case Keys.Tab:
                    result = ImGuiKey.DownArrow;
                    break;
                case Keys.PageUp:
                    result = ImGuiKey.PageUp;
                    break;
                case Keys.PageDown:
                    result = ImGuiKey.PageDown;
                    break;
                case Keys.Home:
                    result = ImGuiKey.Home;
                    break;
                case Keys.End:
                    result = ImGuiKey.End;
                    break;
                case Keys.Delete:
                    result = ImGuiKey.Delete;
                    break;
                case Keys.Backspace:
                    result = ImGuiKey.Backspace;
                    break;
                case Keys.Enter:
                    result = ImGuiKey.Enter;
                    break;
                case Keys.Escape:
                    result = ImGuiKey.Enter;
                    break;
            }
            return result;
        }
        private void UpdateImGuiInputSystem(GameWindow wnd)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            MouseState MouseState = wnd.MouseState;
            KeyboardState KeyboardState = wnd.KeyboardState;
            io.MouseDown[0] = MouseState[MouseButton.Left];
            io.MouseDown[1] = MouseState[MouseButton.Right];
            io.MouseDown[2] = MouseState[MouseButton.Middle];
            io.MouseDown[3] = MouseState[MouseButton.Button4];
            io.MouseDown[4] = MouseState[MouseButton.Button5];
            var screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
            var point = screenPoint;//wnd.PointToClient(screenPoint);
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);
            io.AddMouseWheelEvent(MouseState.ScrollDelta.X, MouseState.ScrollDelta.Y);
            var test = KeyboardState[Keys.E];
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown)
                {
                    continue;
                }
                var ImKey = TryMapKey(key);
                if (ImKey != ImGuiKey.None)
                {
                    if (KeyboardState[key])
                    {
                        io.AddKeyEvent(ImKey, true);
                    }
                    else
                    {
                        io.AddKeyEvent(ImKey, false);
                    }
                }
            }
            foreach (var c in PressedChars)
            {
                io.AddInputCharacter(c);
            }
            PressedChars.Clear();
            io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
            io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
            io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);
        }
        public void PressChar(char keyChar)
        {
            PressedChars.Add(keyChar);
        }
        public void MouseScroll(Vector2 offset)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }
        private void RenderImDrawData(ImDrawDataPtr draw_data)
        {
            if (draw_data.CmdListsCount == 0)
            {
                return;
            }
            // Get intial state.
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            int prevProgram = GL.GetInteger(GetPName.CurrentProgram);
            bool prevBlendEnabled = GL.GetBoolean(GetPName.Blend);
            bool prevScissorTestEnabled = GL.GetBoolean(GetPName.ScissorTest);
            int prevBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
            int prevBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
            int prevBlendFuncSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb);
            int prevBlendFuncSrcAlpha = GL.GetInteger(GetPName.BlendSrcAlpha);
            int prevBlendFuncDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
            int prevBlendFuncDstAlpha = GL.GetInteger(GetPName.BlendDstAlpha);
            bool prevCullFaceEnabled = GL.GetBoolean(GetPName.CullFace);
            bool prevDepthTestEnabled = GL.GetBoolean(GetPName.DepthTest);
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
            Span<int> prevScissorBox = stackalloc int[4];
            unsafe
            {
                fixed (int* iptr = &prevScissorBox[0])
                {
                    GL.GetInteger(GetPName.ScissorBox, iptr);
                }
            }
            Span<int> prevPolygonMode = stackalloc int[2];
            unsafe
            {
                fixed (int* iptr = &prevPolygonMode[0])
                {
                    GL.GetInteger(GetPName.PolygonMode, iptr);
                }
            }
            if (GLVersion <= 310 || CompatibilityProfile)
            {
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            // Bind the element buffer (thru the VAO) so that we can resize it.
            GL.BindVertexArray(_vertexArray);
            // Bind the vertex buffer so that we can resize it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                //ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];
                ImDrawListPtr cmd_list = draw_data.CmdLists[i];
                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > _vertexBufferSize)
                {
                    int newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);
                    GL.BufferData(BufferTarget.ArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw);
                    _vertexBufferSize = newSize;
                    //Console.WriteLine($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
                }
                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > _indexBufferSize)
                {
                    int newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, nint.Zero, BufferUsageHint.DynamicDraw);
                    _indexBufferSize = newSize;
                    //Console.WriteLine($"Resized dear imgui index buffer to new size {_indexBufferSize}");
                }
            }
            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.UseProgram(_shader);
            GL.UniformMatrix4(_shaderProjectionMatrixLocation, false, ref mvp);
            GL.Uniform1(_shaderFontTextureLocation, 0);
            CheckGLError("Projection");
            GL.BindVertexArray(_vertexArray);
            CheckGLError("VAO");
            draw_data.ScaleClipRects(io.DisplayFramebufferScale);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            // Render command lists
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                //ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];
                GL.BufferSubData(BufferTarget.ArrayBuffer, nint.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                CheckGLError($"Data Vert {n}");
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, nint.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);
                CheckGLError($"Data Idx {n}");
                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != nint.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                        CheckGLError("Texture");
                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                        CheckGLError("Scissor");
                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (nint)(pcmd.IdxOffset * sizeof(ushort)), unchecked((int)pcmd.VtxOffset));
                        }
                        else
                        {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                        CheckGLError("Draw");
                    }
                }
            }
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            // Reset state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.UseProgram(prevProgram);
            GL.BindVertexArray(prevVAO);
            GL.Scissor(prevScissorBox[0], prevScissorBox[1], prevScissorBox[2], prevScissorBox[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            GL.BlendEquationSeparate((BlendEquationMode)prevBlendEquationRgb, (BlendEquationMode)prevBlendEquationAlpha);
            GL.BlendFuncSeparate(
                (BlendingFactorSrc)prevBlendFuncSrcRgb,
                (BlendingFactorDest)prevBlendFuncDstRgb,
                (BlendingFactorSrc)prevBlendFuncSrcAlpha,
                (BlendingFactorDest)prevBlendFuncDstAlpha);
            if (prevBlendEnabled) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);
            if (prevDepthTestEnabled) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
            if (prevCullFaceEnabled) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);
            if (prevScissorTestEnabled) GL.Enable(EnableCap.ScissorTest); else GL.Disable(EnableCap.ScissorTest);
            if (GLVersion <= 310 || CompatibilityProfile)
            {
                GL.PolygonMode(MaterialFace.Front, (PolygonMode)prevPolygonMode[0]);
                GL.PolygonMode(MaterialFace.Back, (PolygonMode)prevPolygonMode[1]);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)prevPolygonMode[0]);
            }
        }
        /// <summary>
        /// Frees all graphics resources used by the Luxon.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteVertexArray(_vertexArray);
            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteBuffer(_indexBuffer);
            GL.DeleteTexture(_fontTexture);
            GL.DeleteProgram(_shader);
        }
        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
        {
            if (KHRDebugAvailable)
                GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
        }
        static bool IsExtensionSupported(string name)
        {
            int n = GL.GetInteger(GetPName.NumExtensions);
            for (int i = 0; i < n; i++)
            {
                string extension = GL.GetString(StringNameIndexed.Extensions, i);
                if (extension == name) return true;
            }
            return false;
        }
        public static int CreateProgram(string name, string vertexSource, string fragmentSoruce)
        {
            int program = GL.CreateProgram();
            LabelObject(ObjectLabelIdentifier.Program, program, $"Program: {name}");
            int vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
            int fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSoruce);
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
            }
            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            return program;
        }
        private static int CompileShader(string name, ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            LabelObject(ObjectLabelIdentifier.Shader, shader, $"Shader: {name}");
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
            }
            return shader;
        }
        public static void CheckGLError(string title)
        {
        }
    }
}