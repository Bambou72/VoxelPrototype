using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal class SectionMeshDataCache
    {
        internal Section CurrentSection;
        internal Dictionary<Vector3i,Section> NeighboorsSection = new();

        public SectionMeshDataCache(Section currentSection)
        {
            CurrentSection = currentSection;
            foreach (Vector3i neighborpos in CubeNeighbours.Neighbours)
            {
                Vector3i NeighPos = CurrentSection.Position+ neighborpos;
                if(NeighPos.Y >=0 && NeighPos.Y < Const.ChunkHeight)
                {
                    NeighboorsSection[NeighPos] = currentSection.Chunk.Manager.GetChunk(NeighPos.Xz).Sections[NeighPos.Y];
                }
            }
        }
        public BlockState GetBlock(Vector3i Position)
        {
            if (Position.X >= 0 && Position.X < Const.ChunkSize && Position.Y >= 0 && Position.Y < Const.SectionSize && Position.Z >= 0 && Position.Z < Const.ChunkSize)
            {
                return CurrentSection.BlockPalette.Get(Position);
            }else
            {
                Vector3i Spos = CurrentSection.Position;
                if(Position.X <0)
                {
                    Position.X = Const.ChunkSizeM1;
                    Spos.X--;
                }else if(Position.X > Const.ChunkSizeM1)
                {
                    Position.X = 0;
                    Spos.X++;

                }
                if (Position.Y < 0)
                {
                    Position.Y = Const.ChunkSizeM1;
                    Spos.Y--;
                }
                else if (Position.Y > Const.ChunkSizeM1)
                {
                    Position.Y = 0;
                    Spos.Y++;

                }
                if (Position.Z < 0)
                {
                    Position.Z = Const.ChunkSizeM1;
                    Spos.Z--;
                }
                else if (Position.Z > Const.ChunkSizeM1)
                {
                    Position.Z = 0;
                    Spos.Z++;
                }
                if (Spos.Y >= 0 && Spos.Y < Const.ChunkHeight)
                {
                    return NeighboorsSection[Spos].BlockPalette.Get(Position);
                }
            }
            return Client.TheClient.ModManager.BlockRegister.Air;
        }
        public void LockChunk()
        {
            CurrentSection.Lock.EnterReadLock();
            foreach(Section se in  NeighboorsSection.Values)
            { 
                se.Lock.EnterReadLock();
            }
        }
        public void UnLockChunk()
        {
            CurrentSection.Lock.ExitReadLock();
            foreach (Section se in NeighboorsSection.Values)
            {
                se.Lock.ExitReadLock();
            }
        }
    }
}
