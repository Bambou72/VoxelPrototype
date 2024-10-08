﻿using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class PlayerData : Packet
    {
        public ushort ClientID;
        public string Name;
        public Vector3d Position;
        public Vector3 Rotation;
        public ulong ServerTick;
        public ulong ClientTick;
        public bool Fly;
        public bool Ghost;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientID);
            writer.Put(Name);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(ServerTick);
            writer.Put(ClientTick);
            writer.Put(Fly);
            writer.Put(Ghost);
        }
        public override void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetUShort();
            Name = reader.GetString();
            Position = reader.GetVector3d();
            Rotation = reader.GetVector3();
            ServerTick = reader.GetULong();
            ClientTick = reader.GetULong();
            Fly = reader.GetBool();
            Ghost = reader.GetBool();
        }
        public override Identifier GetID()
        {
            return new Identifier("PlayerData");
        }
    }
}
