using LiteNetLib.Utils;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class PlayerDisconnection : Packet
    {
        public ushort ClientID;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientID);
        }
        public override void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetUShort();
        }
        public override Identifier GetID()
        {
            return new Identifier("PlayerDisconnect");
        }
    }
}
