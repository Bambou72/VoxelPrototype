﻿using LiteNetLib;
using LiteNetLib.Utils;
using OpenTK.Mathematics;
using VoxelPrototype.utils;
namespace VoxelPrototype.network.packets
{
    public class ChunkData : Packet
    {
        public int importance;
        public Vector2i Pos;
        public byte[] Data;
        public NetPeer peer;
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Pos);
            writer.Put(importance);
            writer.Put(Data);
        }
        public override void Deserialize(NetDataReader reader)
        {
            Pos = reader.GetVector2i();
            importance = reader.GetInt();
            Data = reader.GetRemainingBytes();
        }
        public override Identifier GetID()
        {
            return new Identifier("ChunkData");
        }
    }
}
