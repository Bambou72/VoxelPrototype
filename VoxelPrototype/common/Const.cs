using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPrototype.common
{
    public static class Const
    {
        public const int ChunkHeight = 32;
        public const int ChunkSize = 16;
        public const int SectionSize = 16;
        public const int SectionVolume = SectionSize*SectionSize*SectionSize;
        public const int SectionSurface = SectionSize*SectionSize;

    }
}
