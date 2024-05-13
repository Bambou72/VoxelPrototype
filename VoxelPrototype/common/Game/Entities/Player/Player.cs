using LiteNetLib;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.API;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.Blocks.State;
using VoxelPrototype.client;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.common.Game.InventorySystem;
using VoxelPrototype.common.Network.client;
using VoxelPrototype.common.Network.packets;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.Physics;
using VoxelPrototype.server;
namespace VoxelPrototype.common.Game.Entities.Player
{
    public class Player : PhysicEntity
    {
        //
        internal float NormalSpeed = 4.317f;
        internal float SprintSpeed = 5.612f;
        internal float EntityHeight = 1.80f;
        internal float EntityWidth = 0.60f;
        internal float EntityDepth = 0.60f;
        internal float EntityEYEHeight = 1.70f;
        internal float Reach = 4.5f;
        internal string Name = "Test";
        internal ushort ClientID;
        public bool Ghost = false;

        //Client
        internal Vector2 _lastPos;
        internal Camera? _Camera;
        internal bool Local = false;
        internal bool ServerSide = false;
        internal Model _Model;
        internal Vector3i ViewedBlockPos;
        internal Ray? ViewRay;
        internal bool ViewBlock = false;
        //BlockBreaking
        internal Vector3i BlockCurrentBreaking;
        internal float sensitivity = 0.2f;
        internal bool _firstMove = true;
        internal ulong LastServerTick = 0;

        public Player(Vector3d _Position, ushort _ClientID, bool Local, bool Client)
        {
            Position = _Position;
            if (Client)
            {
                if (Local)
                {
                    ViewRay = new Ray(Vector3d.Zero, Vector3.Zero, Reach);

                    _Camera = new Camera((Vector3)_Position, (float)ClientAPI.WindowWidth()/ClientAPI.WindowHeight());
                }
                _Model = client.Client.TheClient.ResourcePackManager.GetEntityMesh("Voxel@entity/player");
            }
            else
            {
                ServerSide = true;
            }
            ClientID = _ClientID;

        }
        internal void HitCallBack(Vector3i CurrentBlock, Vector3i Normal, BlockState State)
        {
            ViewBlock = true;
            ViewedBlockPos = CurrentBlock;
            Vector3i BlockBefore = new Vector3i(CurrentBlock.X + Normal.X, CurrentBlock.Y + Normal.Y, CurrentBlock.Z + Normal.Z);
            if (InputSystem.MouseDown(MouseButton.Left) && InputSystem.Grab)
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(CurrentBlock, BlockRegister.Air);
            }
            else if (InputSystem.MousePressed(MouseButton.Right) && InputSystem.Grab)
            {
                State.Block.OnInteract(CurrentBlock, State,ServerSide);
            }
            /*
            else if (InputSystem.MousePressed(MouseButton.Middle))
            {
                if (ClientWorldManager.world.GetBlock(CurrentBlock.X, CurrentBlock.Y, CurrentBlock.Z, out VoxelInstance id))
                {
                    SelectedBlock = id;
                }
            }*/
        }
        internal void SendControl(InputState st)
        {
            PlayerControl message = new PlayerControl
            {
                Forward = st.controls.forward,
                Backward = st.controls.backward,
                Right = st.controls.right,
                Left = st.controls.left,
                Space = st.controls.space,
                Shift = st.controls.shift,
                Control = st.controls.control,
                Rotation = st.controls.Rotation,
                Front = st.controls.Front,
                CRight = st.controls.Right,
                dt = st.dt,
                ClientTick = st.currentTick,
            };
            ClientNetwork.SendPacket(message, DeliveryMethod.ReliableOrdered);
        }
        internal InputState GetInput(double DT, MouseState Mouse, KeyboardState Keyboard)
        {
            PlayerControls Controls = new PlayerControls();
            if (InputSystem.GetNoInput() == false)
            {
                if (Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    Controls.control = true;
                }
                else
                {
                    Controls.control = false;
                }
                if (Keyboard.IsKeyDown(Keys.W))
                {
                    Controls.forward = true;
                }
                else
                {
                    Controls.forward = false;
                }
                if (Keyboard.IsKeyDown(Keys.S))
                {
                    Controls.backward = true;
                }
                else
                {
                    Controls.backward = false;
                }
                if (Keyboard.IsKeyDown(Keys.A))
                {
                    Controls.left = true;
                }
                else
                {
                    Controls.left = false;
                }
                if (Keyboard.IsKeyDown(Keys.D))
                {
                    Controls.right = true;
                }
                else
                {
                    Controls.right = false;
                }
                if (Keyboard.IsKeyDown(Keys.Space))
                {
                    Controls.space = true;
                }
                else
                {
                    Controls.space = false;
                }
                if (Keyboard.IsKeyDown(Keys.LeftShift))
                {
                    Controls.shift = true;
                }
                else
                {
                    Controls.shift = false;
                }
                if (InputSystem.GetGrab())
                {
                    if (_firstMove) // This bool variable is initially set to true.
                    {
                        _lastPos = new Vector2(Mouse.X, Mouse.Y);
                        _firstMove = false;
                    }
                    else
                    {
                        var change = Mouse.Delta;
                        Rotation.X -= change.Y * sensitivity;
                        Rotation.Y += change.X * sensitivity;
                        if (Rotation.X > 89)
                            Rotation.X = 89;
                        else if (Rotation.X < -89)
                            Rotation.X = -89;
                        if (Rotation.Y > 360)
                            Rotation.Y = 0;
                        else if (Rotation.Y < 0)
                            Rotation.Y = 360;
                        _Camera.Yaw = Rotation.Y;
                        _Camera.Pitch = Rotation.X;
                        _lastPos = new Vector2(Mouse.X, Mouse.Y);
                    }
                    Controls.Front = new Vector3(_Camera.Front.X, 0, _Camera.Front.Z).Normalized();
                    Controls.Right = new Vector3(_Camera.Right.X, 0, _Camera.Right.Z);
                    Controls.Rotation = Rotation;
                }
            }
            return new InputState { controls = Controls, dt = DT, currentTick = Client.TheClient.World.PlayerFactory.CurrentTick };
        }
        internal void UpdateClient(InputState st)
        {
            base.UpdateClient(st.dt);
            ViewBlock = false;
            ViewRay.Update(new Vector3d(Position.X, Position.Y + EntityEYEHeight, Position.Z), _Camera.Front, Reach);
            ViewRay.TestWithTerrain(HitCallBack);
            float Speed = NormalSpeed;
            if (st.controls.control)
            {
                Speed = SprintSpeed;
            }
            if (st.controls.forward)
            {
                Acceleration += st.controls.Front * Speed;
            }
            if (st.controls.backward)
            {
                Acceleration -= st.controls.Front * Speed;
            }
            if (st.controls.left)
            {
                Acceleration -= st.controls.Right * Speed;
            }
            if (st.controls.right)
            {
                Acceleration += st.controls.Right * Speed;
            }
            if (st.controls.space)
            {
                if (!Fly && !Ghost)
                {
                    Jump();
                }
                else
                {
                    Acceleration.Y = Speed; // Up
                }
            }
            if (st.controls.shift)
            {
                if (Fly || Ghost)
                {
                    Acceleration.Y = -Speed; // Down
                }
            }
            if (!Ghost)
            {
                CollisionTerrain(st.dt);
            }
            else
            {
                Position += Acceleration * st.dt;
            }
            _Camera.Position = new Vector3((float)Position.X, (float)Position.Y + EntityEYEHeight, (float)Position.Z);
        }
        //Server
        internal List<Vector2i> inChunk = new();
        internal Queue<PlayerControlsServer> Controls = new();
        internal ulong LastClientTick;
        public Player(Vector3d _Position, ushort _ClientID)
        {
            ViewRay = new Ray(Vector3d.Zero, Vector3.Zero, Reach);

            Position = _Position;
            ClientID = _ClientID;
        }
        internal void Update()
        {
            while (Controls.Count > 0)
            {
                PlayerControlsServer current = Controls.Dequeue();
                UpdateServer(current);
            }
        }
        internal void UpdateServer(PlayerControlsServer Control)
        {
            //ViewRay.Update(new Vector3d(Position.X, Position.Y + EntityEYEHeight, Position.Z), _Camera.Front, Reach);
           // ViewRay.TestWithTerrain(HitCallBack);
            LastClientTick = Control.ClientTick;
            base.UpdateServer(Control.Dt);
            float Speed = NormalSpeed;
            if (Control.control)
            {
                Speed = SprintSpeed;
            }
            if (Control.forward)
            {
                Acceleration += Control.Front * Speed;
                Control.forward = false;
            }
            if (Control.backward)
            {
                Acceleration -= Control.Front * Speed;
                Control.backward = false;
            }
            if (Control.left)
            {
                Acceleration -= Control.Right * Speed;
                Control.left = false;
            }
            if (Control.right)
            {
                Acceleration += Control.Right * Speed;
                Control.right = false;
            }
            if (Control.space)
            {
                if (!Fly && !Ghost)
                {
                    Jump();
                }
                else
                {
                    Acceleration.Y = Speed; // Up
                }
                Control.space = false;
            }
            if (Control.shift)
            {
                if (!Fly && !Ghost)
                {
                }
                else
                {
                    Acceleration.Y = -Speed; // Down
                }
                Control.shift = false;
            }
            if (!Ghost)
            {
                CollisionTerrainServer(Control.Dt);
            }
            else
            {
                Position += Acceleration * Control.Dt;
            }
        }
        internal void SendData()
        {
            PlayerData packet = new PlayerData { ClientID = ClientID, Position = Position, Rotation = Rotation, Name = Name, ClientTick = LastClientTick, ServerTick = Server.TheServer.World.CurrentTick, Fly = Fly, Ghost = Ghost };
            ServerNetwork.SendPacketToAll(packet, DeliveryMethod.Unreliable);
        }
    }
}
