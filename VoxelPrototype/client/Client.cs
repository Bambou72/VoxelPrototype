/**
 * Client main logic
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using Crecerelle;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelPrototype.API;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.client.Debug;
using VoxelPrototype.client.GUI;
using VoxelPrototype.client.Render;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.client.Resources;
using VoxelPrototype.client.UI;
using VoxelPrototype.client.Utils;
using VoxelPrototype.common.Game.World;
using VoxelPrototype.common.Network.client;
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
        internal ResourcePackManager ResourcePackManager;
        internal UIManager UIManager;
        internal UIMaster UIMaster;
        internal Renderer Renderer;
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
            ResourcePackManager = new ResourcePackManager();
            ResourcePackManager.Initialize();
            GUIVar.Init(ClientSize);
            Renderer= new Renderer();
            Renderer.Init();
            BlockRegister.Initialize();
            ModManager.LoadMods();
            
            ModManager.Init();
            GUIVar.Load();
            Load();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            World = new ClientWorld();
            OnResize(new());
            UIManager = new(new UIRenderer());
            UIMaster = new();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            InputSystem.Update(KeyboardState, MouseState, args.Time);
            GUIVar.Update(this, args.Time);
            
            ClientNetwork.Update();
            UIManager.Update(ClientSize,(Vector2i)MouseState.Position);
            if (World.Initialized)
            {
                World.Tick();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            //Render
            //
            //World
            if (World.Initialized)
            {
                World.UpdateRender();
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
            UIManager.Render();

            GUIVar.RenderI();
            GUIVar.Render();
            ImGuiController.CheckGLError("End of frame");
            //Window
            //button.Draw();
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
        public void Load()
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