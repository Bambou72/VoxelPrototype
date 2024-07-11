using LiteNetLib.Utils;
using VoxelPrototype.utils;

namespace VoxelPrototype.network
{
    public abstract class Packet
    {
        public abstract Identifier GetID();
        public abstract void Serialize(NetDataWriter writer);
        public abstract void Deserialize(NetDataReader reader);
    }
}
