﻿/**
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
using VoxelPrototype.client.debug;
using VoxelPrototype.client.game.world;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.client.server;
using VoxelPrototype.client.ui;
using VoxelPrototype.client.ui.prototype.renderer;
using VoxelPrototype.network;
namespace VoxelPrototype.client
{
    public class Client : GameWindow
    {
        public static Client TheClient; 
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Client");
        public World World;
        public ClientNetworkManager NetworkManager;
        internal EmbeddedServer EmbedderServer;
        //internal Config ClientConfig;
        //internal UIManager UIManager;
        UIRenderer UIRenderer;
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
        //
        public bool Grab = false;
        public bool NoInput = false;
        public bool DebugFPS = false;
        internal bool DebugChunk;
        internal bool ShowAABB;

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
            GL.LoadBindings(new GLFWBindingsContext());
            base.OnLoad();
            GUIManager.Init(ClientSize);
            //VoxelGui.Init();
            //Load
            //Renderer
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.DepthTest);
            UIRenderer = new();
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
//UIManager = new();
            //UIManager.SetCurrentScreen(new MainScreen());
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
                SetGrab(!Grab);
            }
            //
            NetworkManager.Update();
            GUIManager.Begin(this, e.Time);
            GUIManager.Update();
            //UIManager.Update();
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
                {
                    World.Render();
                }
                if (ShowAABB)
                {
                    //Show player aabb
                    foreach (var entity in Client.TheClient.World.PlayerFactory.PlayerList.Values)
                    {
                        DebugShapeRenderer.RenderDebugBox(new DebugBox()
                        {
                            Size = new Vector3d(entity.EntityWidth, entity.EntityHeight, entity.EntityWidth),
                            Color = new Vector4(1f, 0f, 0f, 1f),
                            Position = (Vector3)entity.Coll.Min,
                            Rotation = Quaternion.Identity,
                        });
                    }
                }
                if (DebugChunk)
                {
                    Vector3i playpos = new Vector3i((int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Const.ChunkSize), (int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Y / Const.ChunkSize), (int)Math.Floor(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Const.ChunkSize));
                    foreach (Vector2i pos in Client.TheClient.World.ChunkManager.ChunkByCoordinate.Keys)
                    {
                        DebugShapeRenderer.RenderDebugBox(new DebugBox()
                        {
                            Size = new Vector3d(Const.ChunkSize, Const.ChunkHeight * Const.SectionSize, Const.ChunkSize),
                            Color = new Vector4(1f, 0f, 0f, 1f),
                            Position = new Vector3(pos.X, 0, pos.Y) * Const.ChunkSize,
                            Rotation = Quaternion.Identity,
                        });
                    }
                }

            }            
            //
            //Debug
            //
            //
            //Debug UI
            //
            //ImGui.ShowDemoWindow();
            GUIManager.End();
            //UIManager.Render();
            //if(DebugFPS)
            //{
                //UIRenderer.RenderText("FPS:"+ (1 / e.Time).ToString("0") +" ms:"+(e.Time*1000).ToString("0.00"),new Vector2i(0,((1 / e.Time).ToString("0")).CalculateVerticalSize()),Shadow:false);
            //}
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
            GUIManager.ResizeCallback(ClientSize);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            GUIManager.CharCallback((char)e.Unicode);
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
        public void SetGrab(bool Grab)
        {
            this.Grab = Grab;
            CursorState = Grab ? CursorState.Grabbed : CursorState.Normal;
        }
    }
}