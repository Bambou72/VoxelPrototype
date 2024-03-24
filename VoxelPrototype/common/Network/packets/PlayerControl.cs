using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.common.Network.packets
{
    internal struct PlayerControl : INetSerializable
    {
        public bool Forward;
        public bool Backward;
        public bool Right;
        public bool Left;
        public bool Space;
        public bool Shift;
        public bool Control;
        public Vector3 Rotation;
        public Vector3 Front;
        public Vector3 CRight;
        public ulong ClientTick;
        public float dt;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Forward);
            writer.Put(Backward);
            writer.Put(Right);
            writer.Put(Left);
            writer.Put(Space);
            writer.Put(Shift);
            writer.Put(Control);
            writer.Put(Rotation);
            writer.Put(Front);
            writer.Put(CRight);
            writer.Put(dt);
            writer.Put(ClientTick);
        }
        public void Deserialize(NetDataReader reader)
        {
            Forward = reader.GetBool();
            Backward = reader.GetBool();
            Right = reader.GetBool();
            Left = reader.GetBool();
            Space = reader.GetBool();
            Shift = reader.GetBool();
            Control = reader.GetBool();
            Rotation = reader.GetVector3();
            Front = reader.GetVector3();
            CRight = reader.GetVector3();
            dt = reader.GetFloat();
            ClientTick = reader.GetULong();
        }
    }
}
