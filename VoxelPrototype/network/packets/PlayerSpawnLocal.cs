using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class PlayerSpawnLocal :Packet
    {
        public ushort ClientID;
        public Vector3d Position;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientID);
            writer.Put(Position);
        }
        public override void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetUShort();
            Position = reader.GetVector3d();
        }
        public override Identifier GetID()
        {
            return new Identifier("LocalPlayerSpawn");
        }
    }
}
