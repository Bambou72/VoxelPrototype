using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using Monitor = OpenTK.Windowing.GraphicsLibraryFramework.Monitor;
namespace Client
{
    public enum API
    {
        OpenGL,
    }
    internal unsafe class ClientWindow
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _title;
        private GLFWCallbacks.KeyCallback _keyCallback;
        private GLFWCallbacks.MouseButtonCallback _buttonCallback;
        private GLFWCallbacks.CursorPosCallback _mouseposCallback;
        private GLFWCallbacks.ScrollCallback _scrollCallback;
        private GLFWCallbacks.WindowCloseCallback _closeCallback;
        private GLFWCallbacks.WindowSizeCallback _sizeCallback;
        private GLFWCallbacks.CharCallback _charCallback;
        internal MouseListener MouseListener = new();
        internal KeyboardListener KeyboardListener = new();
        internal List<Action<Vector2i, Vector2i>> ResizeCallbacks = new();
        internal List<Action<uint>> CharCallbacks = new();
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                GLFW.SetWindowTitle(_Window, value);
                _title = value;
            }
        }
        public Vector2i Size 
        {
            get 
            {
                GLFW.GetWindowSize(_Window,out int X,out int Y);
                return new Vector2i(X,Y);
            }
            set
            {
                GLFW.SetWindowSize(_Window, value.X, value.Y);
            }
        }
        public Vector2i FramebufferSize
        {
            get
            {
                GLFW.GetFramebufferSize(_Window, out int X, out int Y);
                return new Vector2i(X, Y);
            }
        }
        bool _VSync= true;
        public bool VSync
        {
            get {  return _VSync; }
            set
            {
                _VSync = value;
                GLFW.SwapInterval( value ? 1 :0 );
            }
        }
        bool _Fullscreen = true;
        private Vector2i OldPos;
        private Vector2i OldSize;
        public unsafe bool Fullscreen
        {
            get { return _Fullscreen; }
            set
            {
                _Fullscreen = value;
                if(value)
                {
                    GLFW.GetWindowPos(_Window, out OldPos.X, out OldPos.Y);
                    GLFW.GetWindowSize(_Window, out OldSize.X, out OldSize.Y);
                    VideoMode mode = *GLFW.GetVideoMode(GLFW.GetPrimaryMonitor());
                    GLFW.SetWindowMonitor(_Window, GLFW.GetPrimaryMonitor(), 0, 0, mode.Width, mode.Height, mode.RefreshRate);

                }else
                {
                    GLFW.SetWindowMonitor(_Window, (Monitor*)IntPtr.Zero, OldPos.X, OldPos.Y, OldSize.X, OldSize.Y,0);
                }
            }
        }
        bool _Grab;
        public bool Grab
        {
            get { return _Grab; }
            set
            {
                _Grab = value;
                if (value)
                {
                    GLFW.SetInputMode(_Window, CursorStateAttribute.Cursor,CursorModeValue.CursorDisabled);
                }
                else
                {
                    GLFW.SetInputMode(_Window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
                }
            }

        }
        bool _RawMouseInput;
        public bool RawMouseInput
        {
            get
            {
                return _RawMouseInput;
            }
            set
            {
                if (GLFW.RawMouseMotionSupported())
                {
                    GLFW.SetInputMode(_Window, RawMouseMotionAttribute.RawMouseMotion,value);
                    _RawMouseInput = value;
                }
                else
                {
                    _RawMouseInput = false;
                }
            }
        }
        private Window* _Window;
        public ClientWindow(int X, int Y,string Title,API api = API.OpenGL,bool Fullscreen =false,int SamplesCount = 4)
        {
            GLFWProvider.EnsureInitialized();
            GLFW.WindowHint(WindowHintBool.Visible, true);
            if(api == API.OpenGL)
            {
                GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Compat);
                    GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat,true);
                }
                else
                {
                    GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
                }
                GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
                GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
                //GLFW.WindowHint(WindowHintInt.Samples, SamplesCount);
               
            }
            _title = Title;
            _Window =  GLFW.CreateWindow(X, Y,Title, Fullscreen ? GLFW.GetPrimaryMonitor() : null, null);
            _keyCallback = new GLFWCallbacks.KeyCallback(KeyCallback);
            _buttonCallback = new GLFWCallbacks.MouseButtonCallback(MouseButtonCallback);
            _mouseposCallback = new GLFWCallbacks.CursorPosCallback(MousePositionCallback);
            _scrollCallback = new GLFWCallbacks.ScrollCallback(MouseScrollCallback);
            _sizeCallback = new GLFWCallbacks.WindowSizeCallback(ResizeCallback);
            _charCallback = new GLFWCallbacks.CharCallback(CharCallback);
            GLFW.SetKeyCallback(_Window,_keyCallback);
            GLFW.SetMouseButtonCallback(_Window,_buttonCallback);
            GLFW.SetCursorPosCallback(_Window,_mouseposCallback);
            GLFW.SetScrollCallback(_Window,_scrollCallback);
            GLFW.SetWindowSizeCallback(_Window,_sizeCallback);
            GLFW.SetCharCallback(_Window,_charCallback);
            if(api == API.OpenGL)
            {
                GLFW.MakeContextCurrent(_Window);
                OpenTK.Graphics.OpenGL.GL.LoadBindings(new GLFWBindingsContext());
            }
            GLFW.SwapInterval(1);
        }
        public void Destroy()
        {
            GLFW.DestroyWindow(_Window);
            GLFW.Terminate();
        }
        public void SwapBuffer()
        {
            GLFW.SwapBuffers(_Window);
        }
        public void PollEvent()
        {
            MouseListener.EndFrame();
            KeyboardListener.EndFrame();
            GLFW.PollEvents();
        }
        public void KeyCallback(Window* _window,Keys key , int scanCode,InputAction act , KeyModifiers mod)
        {
            KeyboardListener.KeyCallback(key,scanCode, act, mod);
        }
        public void MouseButtonCallback(Window* _window, MouseButton button, InputAction act, KeyModifiers mod)
        {
            MouseListener.MouseButtonCallback(button, act, mod);
        }
        public void MousePositionCallback(Window* _window, double x,double y)
        {
            MouseListener.MousePosition(x, y);
        }
        public void MouseScrollCallback(Window* _window, double x, double y)
        {
            MouseListener.MouseScrollCallback(x, y);
        }
        public void RegisterResizeCallback(Action<Vector2i,Vector2i> callback)
        {
            ResizeCallbacks.Add(callback);
        }
        public void ResizeCallback(Window* _window,int width ,int height)
        {
            foreach(var callback in ResizeCallbacks)
            {
                callback.Invoke(Size, FramebufferSize);
            }
        }
        public void RegisterCharCallback(Action<uint> callback)
        {
            CharCallbacks.Add(callback);
        }
        public void CharCallback(Window* _window, uint code)
        {
            foreach (var callback in CharCallbacks)
            {
                callback.Invoke(code);
            }
        }
        public void SetIcon(Image[] Icons)
        {
            GLFW.SetWindowIcon(_Window, Icons);
        }
        public bool ShouldClose()
        {
            return GLFW.WindowShouldClose(_Window);
        }
    }
}
