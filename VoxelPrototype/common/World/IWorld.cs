using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.Utils;

namespace VoxelPrototype.common.World
{
    public interface IWorld : IBlockAcessor, ITickable
    {
    }
}
