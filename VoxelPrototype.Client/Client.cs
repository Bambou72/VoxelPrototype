/**
 * Client main logic
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.api;
using VoxelPrototype.client.game.world;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.client.server;
using VoxelPrototype.client.ui;
using VoxelPrototype.client.ui.screens;
using VoxelPrototype.client.ui.utils;
using VoxelPrototype.network;
namespace VoxelPrototype.client
{
    public class Client : GameWindow
    {
        public static Client TheClient; 
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public World World;
        public ClientNetworkManager NetworkManager;
        internal EmbeddedServer EmbedderServer;
        //internal Config ClientConfig;
        internal UIManager UIManager;
        internal Resources.ResourcesManager ResourceManager;
        internal TextureManager TextureManager;
        internal FontManager FontManager;
        internal ModelManager ModelManager;
        internal BlockDataManager BlockDataManager;
        internal ShaderManager ShaderManager;
        //temp
        string[] ResourcesPacksPaths;
        //OpenGL
        private static int MaxTextureSize;
        private static int MaxTextureLayers;
        //
        public bool Grab = false;
        public bool NoInput = false;
        public bool DebugFPS = false;

        public Client( string[]? ResourcesPacksPaths, GameWindowSettings GS , NativeWindowSettings NS) : base(GS, NS) 
        {
            if (TheClient == null)
            {
                TheClient = this;
            }
            this.ResourcesPacksPaths = ResourcesPacksPaths;
            //ClientConfig = new();
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //Load
            //Renderer
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.DepthTest);
            MaxTextureSize = GL.GetInteger(GetPName.MaxTextureSize);
            MaxTextureLayers = GL.GetInteger(GetPName.MaxArrayTextureLayers);
            Logger.Info($"Max texture size is {MaxTextureSize}");
            Logger.Info($"Max texture array layer size is {MaxTextureLayers}");
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            //Mod
            new ModManager();
            ModManager.GetInstance().LoadMods();
            ModManager.GetInstance().PreInit();
            //Resources
            InitResources();
            NetworkManager = new();
            //Mods
            ModManager.GetInstance().Init();
            //World
            World = new World();
            UIManager = new();
            UIManager.SetCurrentScreen(new MainScreen());
        }
        public void InitResources()
        {
            ResourceManager = new(ResourcesPacksPaths);
            TextureManager = new TextureManager();
            ResourceManager.RegisterManager(TextureManager);
            ShaderManager = new ShaderManager();
            ResourceManager.RegisterManager(ShaderManager);
            FontManager = new FontManager();
            ResourceManager.RegisterManager(FontManager);
            ModelManager = new ModelManager();
            ResourceManager.RegisterManager(ModelManager);
            BlockDataManager = new BlockDataManager();
            ResourceManager.RegisterManager(BlockDataManager);
            ResourceManager.Init();
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            //Unload
            //ClientConfig.SaveProperties();
            if(EmbedderServer != null)
            {
                EmbedderServer.Stop();
            }
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Update
            //Debug FPS
            if (KeyboardState.IsKeyPressed(Keys.F1))
            {
                DebugFPS = !DebugFPS;
            }
            //Debug Grab
            if (KeyboardState.IsKeyPressed(Keys.F3))
            {
                if (Grab == false)
                {
                    SetCursorState(CursorState.Grabbed);
                    Grab = true;
                }
                else
                {
                    SetCursorState(CursorState.Normal);
                    Grab = false;
                }
            }
            //
            NetworkManager.Update();
            UIManager.Update();
            if (World.Initialized)
            {
                {
                    World.Tick((float)e.Time);
                }
            }
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );

            //Render
            //
            //World
            if (World.Initialized)
            {
                GL.Disable(EnableCap.Multisample);
                {
                    World.Render();
                }
                GL.Enable(EnableCap.Multisample);

            }
            //
            //Debug
            //
            //
            //Debug UI
            //
            //ImGui.ShowDemoWindow();
            UIManager.Render();
            if(DebugFPS)
            {
                UIManager.Renderer.RenderText("FPS:"+ (1 / e.Time).ToString("0") +" ms:"+(e.Time*1000).ToString("0.00"),new Vector2i(0,TextSizeCalculator.CalculateVerticalSize((1 / e.Time).ToString("0"))),Shadow:false);
            }
            SwapBuffers();
        }

        public void DeInitWorld()
        {
            World.Dispose();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            if (World.IsLocalPlayerExist())
            {
                World.GetLocalPlayerCamera().AspectRatio = FramebufferSize.X / (float)FramebufferSize.Y;
            }
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            UIManager.OnResize();
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
        }
        Vector2i SavedSize;
        Vector2i SavedLocation;
        public  void SetFullscreen(bool fullscreen)
        {
            if(fullscreen)
            {
                SavedSize = Size;
                SavedLocation = Location;
                WindowState = WindowState.Fullscreen;
            }else
            { 
                WindowState = WindowState.Normal;
                Size =  SavedSize;
                Location = SavedLocation;
            }
        }
        public  bool GetFullscreen()
        {
            return WindowState == WindowState.Fullscreen;
        }
        public  void SetCursorState(CursorState state)
        {
           CursorState = state;
        }
    }
}