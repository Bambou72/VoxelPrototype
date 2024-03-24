using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections;
using System.Text;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
namespace VoxelPrototype.client
{
    public unsafe class GLFWWindow
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        Window* Window;
        KeyboardState KeyboardState = new();
        MouseState MouseState = new();
        List<Action<int, int>> ResizeCalbacks = new();
        List<Action<string>> TextInputCalbacks = new();
        private WindowState _windowState = WindowState.Normal;

        /// <summary>
        /// Gets or sets the <see cref="WindowState" /> for this window.
        /// </summary>
        public unsafe WindowState WindowState
        {
            get => _windowState;

            set
            {
                _windowState = value;
                switch (value)
                {
                    case WindowState.Normal:
                        GLFW.RestoreWindow(Window);
                        break;

                    case WindowState.Minimized:
                        GLFW.IconifyWindow(Window);
                        break;

                    case WindowState.Maximized:
                        GLFW.MaximizeWindow(Window);
                        break;

                }
            }
        }
        public unsafe CursorState CursorState
        {
            get
            {
                CursorModeValue inputMode = GLFW.GetInputMode(Window, CursorStateAttribute.Cursor);
                switch (inputMode)
                {
                    case CursorModeValue.CursorNormal:
                        return CursorState.Normal;
                    case CursorModeValue.CursorHidden:
                        return CursorState.Hidden;
                    case CursorModeValue.CursorDisabled:
                        return CursorState.Grabbed;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                CursorModeValue inputMode;
                switch (value)
                {
                    case CursorState.Normal:
                        inputMode = CursorModeValue.CursorNormal;
                        break;
                    case CursorState.Hidden:
                        inputMode = CursorModeValue.CursorHidden;
                        break;
                    case CursorState.Grabbed:
                        inputMode = CursorModeValue.CursorDisabled;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                GLFW.SetInputMode(Window, CursorStateAttribute.Cursor, inputMode);
            }
        }
        public long TimerFrequency { get { return GLFW.GetTimerFrequency(); } }
        public GLFWWindow(string Name, int Width, int Height)
        {
            // Initialiser GLFW
            if (!GLFW.Init())
            {
                return;
            }
            // Créer une fenêtre GLFW
            ///GLFW.WindowHint(WindowHintInt.Samples, 4);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintInt.Samples, 0);
            GLFW.SwapInterval(1);
            Window = GLFW.CreateWindow(Width, Height, Name,
                (OpenTK.Windowing.GraphicsLibraryFramework.Monitor*)nint.Zero, (Window*)nint.Zero);
            if (Window == (Window*)nint.Zero)
            {
                GLFW.Terminate();
                return;
            }

            // Définir le contexte OpenGL
            GLFW.MakeContextCurrent(Window);
            GL.LoadBindings(new GLFWBindingsContext()); // Charger les liaisons OpenGL pour GLFW
            _KeyCallback = KeyCallback;
            GLFW.SetKeyCallback(Window, _KeyCallback);
            _MouseButtonCallback = MouseButtonCallback;
            GLFW.SetMouseButtonCallback(Window, _MouseButtonCallback);
            _WindowSizeCallback = WindowSizeCallback;
            GLFW.SetWindowSizeCallback(Window, _WindowSizeCallback);
            _ScrollCallback = ScrollCallback;
            GLFW.SetScrollCallback(Window,_ScrollCallback );
            _CharCallback = CharCallback;
            GLFW.SetCharCallback(Window, _CharCallback);
            _ErrorCallback = ErrorCallback;
            GLFW.SetErrorCallback(_ErrorCallback);
        }
        GLFWCallbacks.MouseButtonCallback _MouseButtonCallback;
        GLFWCallbacks.KeyCallback _KeyCallback;
        GLFWCallbacks.WindowSizeCallback _WindowSizeCallback;
        GLFWCallbacks.ScrollCallback _ScrollCallback;
        GLFWCallbacks.CharCallback _CharCallback;
        GLFWCallbacks.ErrorCallback _ErrorCallback;
        public void SwapBuffer()
        {
            GLFW.SwapBuffers(Window);
        }
        public void NewInputFrame()
        {
            MouseState.NewFrame(Window);

            KeyboardState.NewFrame();

        }
        public  void ProcessWindowEvents()
        {
            GLFW.PollEvents();
        }
        public void Destroy()
        {
            GLFW.DestroyWindow(Window);
            GLFW.Terminate();
        }
        public void RegisterResizeCallback(Action<int, int> Callback)
        {
            ResizeCalbacks.Add(Callback);
        }
        public void RegisterTextInputCallback(Action<string> Callback)
        {
            TextInputCalbacks.Add(Callback);
        }
        public bool ShouldClose()
        {
            return GLFW.WindowShouldClose(Window);
        }
        public Vector2i GetWindowSize()
        {
            GLFW.GetWindowSize(Window, out var x, out var y);
            return new Vector2i(x, y);
        }
        public KeyboardState GetKeyboardState()
        {
            return KeyboardState;
        }
        public MouseState GetMouseState()
        {
            return MouseState;
        }
        public void ErrorCallback(ErrorCode Error, string Decsription)
        {
            Logger.Error("Error code: " + (int)Error + ", Description: " + Decsription);
        }
        private unsafe void KeyCallback(Window* window, Keys key, int scancode, InputAction action, KeyModifiers mods)
        {
            if (action == InputAction.Release)
            {
                if (key != Keys.Unknown)
                {
                    KeyboardState.SetKeyState(key, false);
                }

            }
            else
            {
                if (key != Keys.Unknown)
                {
                    KeyboardState.SetKeyState(key, true);
                }

            }
        }
        private unsafe void MouseButtonCallback(Window* window, MouseButton button, InputAction action, KeyModifiers mods)
        {
            if (action == InputAction.Release)
            {
                MouseState[button] = false;
            }
            else
            {
                MouseState[button] = true;
            }
        }
        private unsafe void ScrollCallback(Window* window, double offsetX, double offsetY)
        {
            var offset = new Vector2((float)offsetX, (float)offsetY);
            MouseState.Scroll += offset;
        }
        private unsafe void WindowSizeCallback(Window* window, int width, int height)
        {
            foreach (var Callback in ResizeCalbacks)
            {
                Callback.Invoke(width, height);
            }
        }
        private unsafe void CharCallback(Window* window, uint codepoint)
        {
            foreach (var Callback in TextInputCalbacks)
            {
                Callback.Invoke(char.ConvertFromUtf32((int)codepoint));
            }
        }
    }
    // Copyright (C) 2018 OpenTK
    //
    // This software may be modified and distributed under the terms
    // of the MIT license. See the LICENSE file for details.
    //
    /// <summary>
    /// Encapsulates the state of a Keyboard device.
    /// </summary>
    public class KeyboardState
    {
        // These arrays will mostly be empty since the last integer used is 384. That's only 48 bytes though.
        private readonly BitArray _keys = new BitArray((int)Keys.LastKey + 1);
        private readonly BitArray _keysPrevious = new BitArray((int)Keys.LastKey + 1);

        private KeyboardState(KeyboardState source)
        {
            _keys = (BitArray)source._keys.Clone();
            _keysPrevious = (BitArray)source._keysPrevious.Clone();
        }

        internal KeyboardState()
        {
        }

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether the specified <see cref="Keys" /> is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Keys">key</see> to check.</param>
        /// <returns><c>true</c> if key is down; <c>false</c> otherwise.</returns>
        public bool this[Keys key]
        {
            get => IsKeyDown(key);
            private set => SetKeyState(key, value);
        }

        /// <summary>
        /// Gets a value indicating whether any key is currently down.
        /// </summary>
        /// <value><c>true</c> if any key is down; otherwise, <c>false</c>.</value>
        public bool IsAnyKeyDown
        {
            get
            {
                for (var i = 0; i < _keys.Length; ++i)
                {
                    if (_keys[i])
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Sets the key state of the <paramref name="key"/> depending on the given <paramref name="down"/> value.
        /// </summary>
        /// <param name="key">The <see cref="Keys">key</see> which state should be changed.</param>
        /// <param name="down">The new state the key should be changed to.</param>
        internal void SetKeyState(Keys key, bool down)
        {
            _keys[(int)key] = down;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('{');
            var first = true;

            for (Keys key = 0; key <= Keys.LastKey; key++)
            {
                if (IsKeyDown(key))
                {
                    builder.AppendFormat("{0}{1}", key, !first ? ", " : string.Empty);
                    first = false;
                }
            }

            builder.Append('}');

            return builder.ToString();
        }

        internal void NewFrame()
        {
            _keysPrevious.SetAll(false);
            _keysPrevious.Or(_keys);
        }

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this key is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Keys">key</see> to check.</param>
        /// <returns><c>true</c> if <paramref name="key"/> is in the down state; otherwise, <c>false</c>.</returns>
        public bool IsKeyDown(Keys key)
        {
            return _keys[(int)key];
        }

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this key was down in the previous frame.
        /// </summary>
        /// <param name="key">The <see cref="Keys" /> to check.</param>
        /// <returns><c>true</c> if <paramref name="key"/> was in the down state; otherwise, <c>false</c>.</returns>
        public bool WasKeyDown(Keys key)
        {
            return _keysPrevious[(int)key];
        }

        /// <summary>
        ///     Gets whether the specified key is pressed in the current frame but released in the previous frame.
        /// </summary>
        /// <remarks>
        ///     "Frame" refers to invocations of <see cref="NativeWindow.ProcessEvents(double)"/>  (<see cref="NativeWindow.ProcessInputEvents()"/> more precisely) here.
        /// </remarks>
        /// <param name="key">The <see cref="Keys">key</see> to check.</param>
        /// <returns>True if the key is pressed in this frame, but not the last frame.</returns>
        public bool IsKeyPressed(Keys key)
        {
            return _keys[(int)key] && !_keysPrevious[(int)key];
        }

        /// <summary>
        ///     Gets whether the specified key is released in the current frame but pressed in the previous frame.
        /// </summary>
        /// <remarks>
        ///     "Frame" refers to invocations of <see cref="NativeWindow.ProcessEvents(double)"/>  (<see cref="NativeWindow.ProcessInputEvents()"/> more precisely) here.
        /// </remarks>
        /// <param name="key">The <see cref="Keys">key</see> to check.</param>
        /// <returns>True if the key is released in this frame, but pressed the last frame.</returns>
        public bool IsKeyReleased(Keys key)
        {
            return !_keys[(int)key] && _keysPrevious[(int)key];
        }

        /// <summary>
        /// Gets an immutable snapshot of this KeyboardState.
        /// This can be used to save the current keyboard state for comparison later on.
        /// </summary>
        /// <returns>Returns an immutable snapshot of this KeyboardState.</returns>
        public KeyboardState GetSnapshot() => new KeyboardState(this);
    }
    /// <summary>
    /// Encapsulates the state of a mouse device.
    /// </summary>
    public class MouseState
    {
        /// <summary>
        /// The maximum number of buttons a <see cref="MouseState"/> can represent.
        /// </summary>
        internal const int MaxButtons = 16;

        private readonly BitArray _buttons = new BitArray(MaxButtons);
        private readonly BitArray _buttonsPrevious = new BitArray(MaxButtons);

        internal MouseState()
        {
        }

        private MouseState(MouseState source)
        {
            // Vector2 is a struct, so these should be value copies
            Position = source.Position;
            PreviousPosition = source.PreviousPosition;

            Scroll = source.Scroll;
            PreviousScroll = source.PreviousScroll;

            _buttons = (BitArray)source._buttons.Clone();
            _buttonsPrevious = (BitArray)source._buttonsPrevious.Clone();
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> representing the absolute position of the pointer
        /// in the current frame, relative to the top-left corner of the contents of the window.
        /// </summary>
        public Vector2 Position { get; internal set; }

        /// <summary>
        /// Gets a <see cref="Vector2"/> representing the absolute position of the pointer
        /// in the previous frame, relative to the top-left corner of the contents of the window.
        /// </summary>
        public Vector2 PreviousPosition { get; internal set; }

        /// <summary>
        /// Gets a <see cref="Vector2"/> representing the amount that the mouse moved since the last frame.
        /// This does not necessarily correspond to pixels, for example in the case of raw input.
        /// </summary>
        public Vector2 Delta => Position - PreviousPosition;

        /// <summary>
        /// Get a Vector2 representing the position of the mouse wheel.
        /// </summary>
        public Vector2 Scroll { get; internal set; }

        /// <summary>
        /// Get a Vector2 representing the position of the mouse wheel.
        /// </summary>
        public Vector2 PreviousScroll { get; internal set; }

        /// <summary>
        /// Get a Vector2 representing the amount that the mouse wheel moved since the last frame.
        /// </summary>
        public Vector2 ScrollDelta => Scroll - PreviousScroll;

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether the specified
        ///  <see cref="MouseButton" /> is pressed.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton" /> to check.</param>
        /// <returns><c>true</c> if key is pressed; <c>false</c> otherwise.</returns>
        public bool this[MouseButton button]
        {
            get => _buttons[(int)button];
            internal set { _buttons[(int)button] = value; }
        }

        /// <summary>
        /// Gets an integer representing the absolute x position of the pointer, in window pixel coordinates.
        /// </summary>
        public float X => Position.X;

        /// <summary>
        /// Gets an integer representing the absolute y position of the pointer, in window pixel coordinates.
        /// </summary>
        public float Y => Position.Y;

        /// <summary>
        /// Gets an integer representing the absolute x position of the pointer, in window pixel coordinates.
        /// </summary>
        public float PreviousX => PreviousPosition.X;

        /// <summary>
        /// Gets an integer representing the absolute y position of the pointer, in window pixel coordinates.
        /// </summary>
        public float PreviousY => PreviousPosition.Y;

        /// <summary>
        /// Gets a value indicating whether any button is down.
        /// </summary>
        /// <value><c>true</c> if any button is down; otherwise, <c>false</c>.</value>
        public bool IsAnyButtonDown
        {
            get
            {
                for (int i = 0; i < MaxButtons; i++)
                {
                    if (_buttons[i])
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="MouseState" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="MouseState" />.</returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < MaxButtons; i++)
            {
                b.Append(_buttons[i] ? "1" : "0");
            }

            return $"[X={X}, Y={Y}, Buttons={b}]";
        }

        internal unsafe void NewFrame(Window* Window)
        {
            _buttonsPrevious.SetAll(false);
            _buttonsPrevious.Or(_buttons);

            PreviousPosition = Position;
            PreviousScroll = Scroll;

            GLFW.GetCursorPos(Window, out var x, out var y);
            Position = new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this button is down.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton" /> to check.</param>
        /// <returns><c>true</c> if the <paramref name="button"/> is down, otherwise <c>false</c>.</returns>
        public bool IsButtonDown(MouseButton button)
        {
            return _buttons[(int)button];
        }

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this button was down in the previous frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton" /> to check.</param>
        /// <returns><c>true</c> if the <paramref name="button"/> is down, otherwise <c>false</c>.</returns>
        public bool WasButtonDown(MouseButton button)
        {
            return _buttonsPrevious[(int)button];
        }

        /// <summary>
        /// Gets whether the specified mouse button is pressed in the current frame but released in the previous frame.
        /// </summary>
        /// <remarks>
        ///     "Frame" refers to invocations of <see cref="NativeWindow.ProcessEvents(double)"/> (<see cref="NativeWindow.ProcessInputEvents()"/> more precisely) here.
        /// </remarks>
        /// <param name="button">The <see cref="MouseButton">mouse button</see> to check.</param>
        /// <returns>True if the mouse button is pressed in this frame, but not the last frame.</returns>
        public bool IsButtonPressed(MouseButton button)
        {
            return _buttons[(int)button] && !_buttonsPrevious[(int)button];
        }

        /// <summary>
        /// Gets whether the specified mouse button is released in the current frame but pressed in the previous frame.
        /// </summary>
        /// <remarks>
        /// "Frame" refers to invocations of <see cref="NativeWindow.ProcessEvents(double)"/> (<see cref="NativeWindow.ProcessInputEvents()"/> more precisely) here.
        /// </remarks>
        /// <param name="button">The <see cref="MouseButton">mouse button</see> to check.</param>
        /// <returns>True if the mouse button is released in this frame, but pressed the last frame.</returns>
        public bool IsButtonReleased(MouseButton button)
        {
            return !_buttons[(int)button] && _buttonsPrevious[(int)button];
        }

        /// <summary>
        /// Gets an immutable snapshot of this MouseState.
        /// This can be used to save the current mouse state for comparison later on.
        /// </summary>
        /// <returns>Returns an immutable snapshot of this MouseState.</returns>
        public MouseState GetSnapshot() => new MouseState(this);
    }
}
