using LiteNetLib.Utils;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    internal class ClientInitialPacket : Packet
    {
        public string Name;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Name);
        }
        public override void Deserialize(NetDataReader reader)
        {
            Name = reader.GetString();
        }
        public override Identifier GetID()
        {
            return new Identifier("InitialClient");
        }
    }
}
