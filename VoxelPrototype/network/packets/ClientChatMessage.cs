using LiteNetLib.Utils;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class ClientChatMessage : Packet
    {
        public string Message;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Message);
        }
        public override void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
        }
        public override Identifier GetID()
        {
            return new Identifier("ClientChatMessage");
        }
    }
}
