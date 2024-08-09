using LiteNetLib;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.api;
using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;
using VoxelPrototype.client.rendering.camera;
using VoxelPrototype.client.rendering.model;
using VoxelPrototype.game;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.network.packets;
using VoxelPrototype.physics;
namespace VoxelPrototype.client.game.entity
{
    internal class ClientPlayer : Player
    {
        static string PlayerModelResourceID = "voxelprototype:models/entity/player";
        internal FrustumCamera? _Camera;
        internal bool Local = false;
        internal bool ServerSide = false;
        internal Model _Model;
        internal Vector3i ViewedBlockPos;
        internal Ray? ViewRay;
        internal bool ViewBlock = false;
        internal Block SelectedBlock = null;
        //BlockBreaking
        internal Vector3i BlockCurrentBreaking;
        internal float sensitivity = 0.5f;
        internal ulong LastServerTick = 0;
        public ClientPlayer(Vector3d _Position, ushort _ClientID, bool Local, bool Client)
        {
            Position = _Position;
            if (Client)
            {
                if (Local)
                {
                    ViewRay = new Ray(Vector3d.Zero, Vector3.Zero, Reach);

                    _Camera = new FrustumCamera((Vector3)_Position, (float) client.Client.TheClient.ClientSize.X / client.Client.TheClient.ClientSize.Y);
                }
                _Model = client.Client.TheClient.ModelManager.GetEntityModel(PlayerModelResourceID);
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

            if (Client.TheClient.MouseState.IsButtonDown(MouseButton.Left) && Client.TheClient.Grab)
            {
                Client.TheClient.World.ChunkManager.ChangeChunk(CurrentBlock, BlockRegistry.GetInstance().Air);
            }
            else if (Client.TheClient.MouseState.IsButtonPressed(MouseButton.Right) && Client.TheClient.Grab)
            {
                /*if(SelectedBlock == null)
                {
                    SelectedBlock = BlockRegistry.GetInstance().GetBlock("voxelprototype:lamp");
                }
                Client.TheClient.World.ChunkManager.ChangeChunk(CurrentBlock+Normal, SelectedBlock.GetDefaultState());*/
                State.Block.OnInteract(Client.TheClient.World, CurrentBlock, State, false);

            }
            else if (Client.TheClient.MouseState.IsButtonPressed(MouseButton.Middle))
            {
                SelectedBlock = Client.TheClient.World.GetBlock(new Vector3i(CurrentBlock.X, CurrentBlock.Y, CurrentBlock.Z)).Block;
            }
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
            Client.TheClient.NetworkManager.SendPacketToServer(message, DeliveryMethod.ReliableOrdered);
        }
        internal InputState GetInput(double DT)
        {
            PlayerControls Controls = new PlayerControls();

            if (Client.TheClient.NoInput == false)
            {
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Controls.control = true;
                }
                else
                {
                    Controls.control = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.W))
                {
                    Controls.forward = true;
                }
                else
                {
                    Controls.forward = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.S))
                {
                    Controls.backward = true;
                }
                else
                {
                    Controls.backward = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.A))
                {
                    Controls.left = true;
                }
                else
                {
                    Controls.left = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.D))
                {
                    Controls.right = true;
                }
                else
                {
                    Controls.right = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.Space))
                {
                    Controls.space = true;
                }
                else
                {
                    Controls.space = false;
                }
                if (Client.TheClient.KeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    Controls.shift = true;
                }
                else
                {
                    Controls.shift = false;
                }
                if (Client.TheClient.Grab)
                {
                    Vector2 change = Client.TheClient.MouseState.Delta;
                    float s = MathF.Pow(0.6f * sensitivity + 0.2f, 3) * 8.0f;
                    Rotation.X -= change.Y * sensitivity * 0.15f;
                    Rotation.Y += change.X * sensitivity * 0.15f;
                    if (Rotation.X > 90)
                        Rotation.X = 90;
                    else if (Rotation.X < -90)
                        Rotation.X = -90;
                    if (Rotation.Y > 360)
                        Rotation.Y = 0;
                    else if (Rotation.Y < 0)
                        Rotation.Y = 360;
                    _Camera.Yaw = Rotation.Y;
                    _Camera.Pitch = Rotation.X;
                    Controls.Front = new Vector3(_Camera.Front.X, 0, _Camera.Front.Z).Normalized();
                    Controls.Right = new Vector3(_Camera.Right.X, 0, _Camera.Right.Z);
                    Controls.Rotation = Rotation;
                }
            }
            return new InputState { controls = Controls, dt = DT, currentTick = Client.TheClient.World.PlayerFactory.CurrentTick };
        }
        public  void Update(InputState st, IWorld World)
        {
            base.Update(World, st.dt);
            ViewBlock = false;
            ViewRay.Update(new Vector3d(Position.X, Position.Y + EntityEYEHeight, Position.Z), _Camera.Front, Reach);
            ViewRay.TestWithTerrain(Client.TheClient.World, HitCallBack);
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
                if (!Fly && !NoClip)
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
                if (Fly || NoClip)
                {
                    Acceleration.Y = -Speed; // Down
                }
            }
            _Camera.Position = new Vector3((float)Position.X, (float)Position.Y + EntityEYEHeight, (float)Position.Z);
        }
    }
}

