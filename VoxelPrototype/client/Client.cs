/**
 * Client main logic
 * Copyrights Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using VoxelPrototype.client.Render;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.client.Render.GUI;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.client.Resources.Managers;
using VoxelPrototype.client.World;
using VoxelPrototype.common;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.World;
using VoxelPrototype.server;
using static OpenTK.Graphics.OpenGL.GL;
namespace VoxelPrototype.client
{
    public class Client 
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public World.World World;
        public static Client TheClient;
        internal EmbeddedServer EmbedderServer;
        internal Config ClientConfig;
        // internal UIManager UIManager;
        //internal UIMaster UIMaster;
        internal IClientInterface ClientInterface;
        internal ModManager ModManager;
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
        bool Running = true;
        private const double MaxFrequency = 500.0;

        private readonly Stopwatch _watchUpdate = new Stopwatch();
        private int _slowUpdates = 0;
        protected bool IsRunningSlowly { get; private set; }
        private double _updateFrequency;
        public double UpdateFrequency
        {
            get => _updateFrequency;

            set
            {
                if (value < 1.0)
                {
                    _updateFrequency = 0.0;
                }
                else if (value <= MaxFrequency)
                {
                    _updateFrequency = value;
                }
                else
                {
                    Debug.Print("Target render frequency clamped to {0}Hz.", MaxFrequency);
                    _updateFrequency = MaxFrequency;
                }
            }
        }
        public double UpdateTime { get; protected set; }
        public Client(IClientInterface clientInterface, string[]? ResourcesPacksPaths) 
        {
            ClientInterface = clientInterface;
            clientInterface.RegisterOnResize(OnResize);
            clientInterface.RegisterOnChar(OnTextInput);
            if (TheClient == null)
            {
                TheClient = this;
            }
            this.ResourcesPacksPaths = ResourcesPacksPaths;
            ClientConfig = new();
        }

        public void Run()
        {
            //Load
            //Renderer
            Renderer = new Renderer();
            Renderer.Init();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            //Mod
            ModManager = new();
            ModManager.LoadMods();
            ModManager.PreInit();
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
            //Mod
            ModManager.Init();
            //GUi
            GUIVar.Init(ClientInterface.GetFramebufferSize());
            GUIVar.Load();
            //Input
            InputEventManager = new InputEventManager();
            //World
            World = new World.World();
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
            _watchUpdate.Start();
            while (Running && !ClientInterface.ShouldEnd())
            {

                double updatePeriod = UpdateFrequency == 0 ? 0 : 1 / UpdateFrequency;
                double elapsed = _watchUpdate.Elapsed.TotalSeconds;
                if (elapsed > updatePeriod)
                {
                    _watchUpdate.Restart();
                    ClientInterface.PollInputs();
                    UpdateTime = elapsed;
                    Update(elapsed);
                    Render();
                    const int MaxSlowUpdates = 80;
                    const int SlowUpdatesThreshold = 45;

                    double time = _watchUpdate.Elapsed.TotalSeconds;
                    if (updatePeriod < time)
                    {
                        _slowUpdates++;
                        if (_slowUpdates > MaxSlowUpdates)
                        {
                            _slowUpdates = MaxSlowUpdates;
                        }
                    }
                    else
                    {
                        _slowUpdates--;
                        if (_slowUpdates < 0)
                        {
                            _slowUpdates = 0;
                        }
                    }
                    IsRunningSlowly = _slowUpdates > SlowUpdatesThreshold;
                }
                // The time we have left to the next update.
                double timeToNextUpdate = updatePeriod - _watchUpdate.Elapsed.TotalSeconds;

                if (timeToNextUpdate > 0)
                {
                    AccurateSleep(timeToNextUpdate, 8);
                }
            }
            //Unload
            GUIVar.DeLoad();
            ClientConfig.SaveProperties();

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
        public void Update(double DT)
        {
            //Update
            ClientNetwork.Update();
            GUIVar.Update(ClientInterface, DT);
            GUIVar.Update();
            //UIManager.Update();
            if (World.Initialized)
            {
                World.Tick((float)DT);
            }
            //InputEventManager.Update();
            World.UpdateRender();


        }
        public void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //Render
            //
            //World
            if (World.Initialized)
            {
                GL.Disable(EnableCap.Multisample);
                World.Render();
                GL.Enable(EnableCap.Multisample);
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
            ClientInterface.SwapBuffers();
        }
        public void Close()
        {
            Running = false;
        }
        public void DeInitWorld()
        {
            World.Dispose();
        }
        protected void OnResize(Vector2i Size, Vector2i FramebufferSize)
        {
            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            if (World.IsLocalPlayerExist())
            {
                World.GetLocalPlayerCamera().AspectRatio = FramebufferSize.X / (float)FramebufferSize.Y;
            }
            GUIVar.Resize(FramebufferSize);
        }
        protected void OnTextInput(uint code)
        {
            GUIVar.Char((char)(code));

        }
    }
}