/**
 * Client main logic
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelPrototype.api;
using VoxelPrototype.client.game.world;
using VoxelPrototype.client.Render;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.client.Render.GUI;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.network;
namespace VoxelPrototype.client
{
    public class Client : GameWindow
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public game.world.World World;
        public ClientNetworkManager NetworkManager;
        public static Client TheClient;
        internal EmbeddedServer EmbedderServer;
        internal Config ClientConfig;
        // internal UIManager UIManager;
        //internal UIMaster UIMaster;
        internal Resources.ResourcesManager ResourceManager;
        internal Renderer Renderer;
        internal InputEventManager InputEventManager;
        internal TextureManager TextureManager;
        internal FontManager FontManager;
        internal ModelManager ModelManager;
        internal BlockDataManager BlockDataManager;
        internal ShaderManager ShaderManager;
        //temp
        string[] ResourcesPacksPaths;
        public Client( string[]? ResourcesPacksPaths, GameWindowSettings GS , NativeWindowSettings NS) : base(GS, NS) 
        {
            if (TheClient == null)
            {
                TheClient = this;
            }
            this.ResourcesPacksPaths = ResourcesPacksPaths;
            ClientConfig = new();
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //Load
            //Renderer
            Renderer = new Renderer();
            Renderer.Init();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            //Mod
            new ModManager();
            ModManager.GetInstance().LoadMods();
            ModManager.GetInstance().PreInit();
            //Resources
            ResourceManager = new(ResourcesPacksPaths);
            ShaderManager = new ShaderManager();
            ResourceManager.RegisterManager(ShaderManager);
            FontManager = new FontManager();
            ResourceManager.RegisterManager(FontManager);
            TextureManager = new TextureManager();
            ResourceManager.RegisterManager(TextureManager);
            ModelManager = new ModelManager();
            ResourceManager.RegisterManager(ModelManager);
            BlockDataManager = new BlockDataManager();
            ResourceManager.RegisterManager(BlockDataManager);
            ResourceManager.Init();
            NetworkManager = new();
            //Mod
            ModManager.GetInstance().Init();
            //GUi
            GUIVar.Init(FramebufferSize);
            GUIVar.Load();
            //Input
            InputEventManager = new InputEventManager();
            //World
            World = new World();
            //OnResize(new());
            //UIManager = new();
            //InputEventManager.RegisterOnKeyDownCallback(UIManager.OnKeyDown);
            //InputEventManager.RegisterOnMouseUpCallback(UIManager.OnMouseUp);
            // InputEventManager.RegisterOnKeyPressedCallback(UIManager.OnKeyPressed);
            //InputEventManager.RegisterOnMouseMoveCallback(UIManager.OnMouseMove);
            //InputEventManager.RegisterOnMouseWheelCallback(UIManager.OnMouseWheel);
            //InputEventManager.RegisterOnMouseDownCallback(UIManager.OnMouseDown);
            //InputEventManager.RegisterOnMouseClickedCallback(UIManager.OnMouseClicked);
            //InputEventManager.RegisterOnMouseWheelCallback(UIManager.OnMouseWheel);
            //UIMaster = new();

        }
        protected override void OnUnload()
        {
            base.OnUnload();
            //Unload
            GUIVar.DeLoad();
            ClientConfig.SaveProperties();
            if(EmbedderServer != null)
            {
                EmbedderServer.Stop();
            }
#if PROFILE
            Profiler.Dispose();
#endif
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Update
            NetworkManager.Update();

#if PROFILE
            using (Profiler.BeginEvent("GUI Update"))
#endif
            {
                GUIVar.Update((float)e.Time);
                GUIVar.Update();
            }

            //UIManager.Update();
            if (World.Initialized)
            {
#if PROFILE
                using(Profiler.BeginEvent("World Tick"))
#endif
                {
                    World.Tick((float)e.Time);
                }
            }
            //InputEventManager.Update();
        }
        protected override void  OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //Render
            //
            //World
            if (World.Initialized)
            {
                GL.Disable(EnableCap.Multisample);
#if PROFILE
                using (Profiler.BeginEvent("World Render"))
#endif
                {
                    World.Render();
                }
                GL.Enable(EnableCap.Multisample);
                if (World.IsLocalPlayerExist())
                {
                    //Player.Draw(Subsystems.RessourceManager.RessourceManager.GetShader("Nentity"), PlayerAnimator);
                }/*
                if (World.IsLocalPlayerExist())
                {
                    DebugShapeRenderer.Render(World.GetLocalPlayerCamera().GetViewMatrix(), World.GetLocalPlayerCamera().GetProjectionMatrix());
                }*/
            }
            //
            //Debug
            //
            //
            //Debug UI
            //
            //ImGui.ShowDemoWindow();
            //UIManager.Render();
#if PROFILE
            using (Profiler.BeginEvent("GUI Render"))
#endif
            {
                GUIVar.RenderI();
                GUIVar.Render();
            }
            SwapBuffers();
#if PROFILE
            Profiler.ProfileFrame("Main");
#endif
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
            GUIVar.Resize(FramebufferSize);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            GUIVar.Char((char)(e.Unicode));

        }
    }
}