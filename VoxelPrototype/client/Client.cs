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
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Items;
using VoxelPrototype.client.Debug;
using VoxelPrototype.client.GUI;
using VoxelPrototype.client.GUI.Prototype;
using VoxelPrototype.client.Render;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.World;
using VoxelPrototype.server;
namespace VoxelPrototype.client
{
    public class Client : GameWindow
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public ClientWorld World;
        public static Client TheClient;
        internal EmbeddedServer EmbedderServer;
        internal Config ClientConfig;
        // internal UIManager UIManager;
        //internal UIMaster UIMaster;
        internal ModManager ModManager;
        internal Resources.ResourceManager ResourceManager;
        internal Renderer Renderer;
        internal InputEventManager InputEventManager;
        internal TextureManager TextureManager;
        internal FontManager FontManager;
        internal ModelManager ModelManager;
        internal BlockDataManager BlockDataManager;
        internal ShaderManager ShaderManager;
        public Client(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            if (TheClient == null)
            {
                TheClient = this;
            }
            ClientConfig = new();
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //Renderer
            Renderer = new Renderer();
            Renderer.Init();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            //Mod
            ModManager = new();
            ModManager.LoadMods();
            ModManager.PreInit();
            //Resources
            ResourceManager = new();
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
            //Mod
            ModManager.Init();
            //GUi
            GUIVar.Init(ClientSize);
            GUIVar.Load();
            //Input
            InputEventManager = new InputEventManager();
            //World
            World = new ClientWorld();
            LoadWorld();
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
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            
            ClientNetwork.Update();
            GUIVar.Update(this, args.Time);
            GUIVar.Update();
            //UIManager.Update();
            if (World.Initialized)
            {
                World.Tick((float)args.Time);
            }
            InputEventManager.Update();
            World.UpdateRender();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            //Render
            //
            //World
            if (World.Initialized)
            {
                World.Render();
                if (World.IsLocalPlayerExist())
                {
                    //Player.Draw(Subsystems.RessourceManager.RessourceManager.GetShader("Nentity"), PlayerAnimator);
                }
                if (World.IsLocalPlayerExist())
                {
                    DebugShapeRenderer.Render(World.GetLocalPlayerCamera().GetViewMatrix(), World.GetLocalPlayerCamera().GetProjectionMatrix());
                }
            }
            //
            //Debug
            //
            //
            //Debug UI
            //
            //ImGui.ShowDemoWindow();
            //UIManager.Render();
            
            GUIVar.RenderI();
            GUIVar.Render();
            ImGuiController.CheckGLError("End of frame");
            SwapBuffers();
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            GUIVar.DeLoad();
            ClientConfig.SaveProperties();
        }
        internal List<string> Worlds = new();
        internal List<WorldInfo> WorldsInfo = new();
        public void LoadWorld()
        {
            string[] WorldFolders = Directory.GetDirectories("worlds");
            foreach (string world in WorldFolders)
            {
                if (File.Exists(world + "/world.vpw"))
                {
                    try
                    {
                        WorldInfo worldInfo = new WorldInfo().Deserialize(File.ReadAllBytes(world + "/world.vpw"));
                        worldInfo.Path = world;
                        worldInfo.Name = world.Split("\\").Last();
                        WorldsInfo.Add(worldInfo);
                        Worlds.Add(worldInfo.Name);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "A world can't be load , possibly due to corrupted data.");
                    }
                }
                else
                {
                }
            }
        }
        
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            InputEventManager.OnMouseDown(e.Button, (Vector2i)MousePosition);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            
           
            base.OnMouseUp(e);
            InputEventManager.OnMouseUp(e.Button, (Vector2i)MousePosition);


        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            InputEventManager.OnMouseMove((Vector2i)e.Position, (Vector2i)e.Delta);
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {

            InputEventManager.OnKeyDown(e.Key, e.Modifiers);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            InputEventManager.OnKeyUp(e.Key, e.Modifiers);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            InputEventManager.OnMouseWheel((Vector2i)e.Offset);
        }
        public void DeInitWorld()
        {
            World.Dispose();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            if (World.IsLocalPlayerExist())
            {
                World.GetLocalPlayerCamera().AspectRatio = ClientSize.X / (float)ClientSize.Y;
            }
            GUIVar.Resize(ClientSize);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            GUIVar.Char((char)(e.Unicode));

        }
    }
}