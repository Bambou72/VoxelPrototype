/**
 * Client main logic
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using VoxelPrototype.API;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.client.Debug;
using VoxelPrototype.client.GUI;
using VoxelPrototype.client.Render;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.common.Debug;
using VoxelPrototype.common.Game.World;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Utils;
using VoxelPrototype.server;
namespace VoxelPrototype.client
{
    public class Client: IRunnable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        const float UpdateTime = 1f / 60f;
        Stopwatch Garbage = new();
        private readonly Stopwatch _watchUpdate = new Stopwatch();
        private int _slowUpdates = 0;
        protected bool IsRunningSlowly { get; private set; }
        internal GLFWWindow Window;
        public ClientWorld World;
        public static Client TheClient;
        internal EmbeddedServer EmbedderServer;
        public Client()
        {
            if(TheClient == null)
            {
                TheClient = this;

            }
        }
        internal double deltaTime;
        internal double accumulator;
        public void Run()
        {
            Window = new("Voxel Prototype", 900, 800);
            Window.RegisterResizeCallback(OnResize);
            Window.RegisterTextInputCallback(OnTextInput);
            LoggingSystem.Init();
            Logger.Info("GLFW timer frequency: " + Window.TimerFrequency +"HZ");
            GUIVar.Init( Window.GetWindowSize());
            Renderer.Init();
            ClientRessourcePackManager.RessourcePackManager.Initialize();
            ModManager.LoadMods();
            BlockRegister.Initialize();
            ModManager.Init();
            GUIVar.Load();
            Load();
            Garbage.Start();
            GL.ClearColor(0.39f, 0.58f, 0.92f, 1.0f);
            World = new ClientWorld();
            _watchUpdate.Start();
            double currentFrame = GLFW.GetTime();
            double lastFrame = currentFrame;

            while (!Window.ShouldClose())
            {
                Window.NewInputFrame();
                Window.ProcessWindowEvents();
                currentFrame = GLFW.GetTime();
                deltaTime = currentFrame - lastFrame;
                lastFrame = currentFrame;
                accumulator += deltaTime;
                //
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                //Update
                //
                InputSystem.Update(Window.GetKeyboardState(), Window.GetMouseState());
                //InputSystem.Update(new(), new());
                ClientNetwork.Update();
                if (World.Initialized)
                {
                    World.Tick();
                }

                if (Garbage.ElapsedMilliseconds >= 4000)
                {
                    GC.Collect();
                    Garbage.Restart();
                }


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
                GUIVar.Update(Window, (float)deltaTime);
                //ImGui.ShowDemoWindow();
                GUIVar.Render();
                GUIVar.FinalRender();
                ImGuiController.CheckGLError("End of frame");
                //Window
                Window.SwapBuffer();
            }
            
            GUIVar.DeLoad();

            Window.Destroy();
        }
        public static void AccurateSleep(double seconds, int expectedSchedulerPeriod)
        {
            // FIXME: Make this a parameter?
            const double TOLERANCE = 0.02;

            long t0 = Stopwatch.GetTimestamp();
            long target = t0 + (long)(seconds * Stopwatch.Frequency);

            double ms = (seconds * 1000) - (expectedSchedulerPeriod * TOLERANCE);
            int ticks = (int)(ms / expectedSchedulerPeriod);
            if (ticks > 0)
            {
                Thread.Sleep(ticks * expectedSchedulerPeriod);
            }

            while (Stopwatch.GetTimestamp() < target)
            {
                Thread.Yield();
            }
        }
        internal List<string> Worlds = new();
        internal  List<WorldInfo> WorldsInfo = new();
        public  void Load()
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
        void OnResize(int Width , int Height)
        {
            GL.Viewport(0, 0, Width, Height);
            if (World.IsLocalPlayerExist())
            {
                World.GetLocalPlayerCamera().AspectRatio = Window.GetWindowSize().X / (float)Window.GetWindowSize().Y;
            }
            GUIVar.Resize(Window.GetWindowSize());
        }
        void OnTextInput(string text)
        {
            GUIVar.Char(char.Parse(text));
        }
    }
}