﻿using LiteNetLib.Utils;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.game.world.storage;

namespace VoxelPrototype.server.game.world.Level.Chunk
{
    public class Section : IVBFSerializable<Section>
    {
        internal BlockPalette BlockPalette;
        internal int Y;
        internal Chunk Chunk;
        public Section()
        {
            BlockPalette = new(1);
        }

        public bool Empty { get { return BlockPalette.Palette[0].RefCount == Const.SectionVolume; } }

        public Section Deserialize(VBFCompound compound)
        {
            Y = compound.GetInt("Y").Value;
            BlockPalette = BlockPalette.Deserialize(compound.Get<VBFCompound>("BP"));
            return this;
        }
        public VBFCompound Serialize()
        {
            VBFCompound Section = new();
            Section.AddInt("Y", Y);
            Section.Add("BP", BlockPalette.Serialize());
            return Section;
        }
        public void SetBlock(Vector3i pos, BlockState id)
        {
            if (pos.Y >= Const.SectionSize || pos.Y < 0)
            {
                throw new Exception("Error");
            }
            BlockPalette.Set(pos, id);
        }
    }

}