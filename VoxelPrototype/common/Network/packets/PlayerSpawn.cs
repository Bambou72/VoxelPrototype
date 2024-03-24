using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.common.Network.packets
{
    internal struct PlayerSpawn : INetSerializable
    {
        public ushort ClientID;
        public Vector3d Position;
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientID);
            writer.Put(Position);
        }
        public void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetUShort();
             Position = reader.GetVector3d();
        }
    }
}
