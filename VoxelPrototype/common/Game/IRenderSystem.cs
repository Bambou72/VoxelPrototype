using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPrototype.common.Game
{
    internal interface IRenderSystem
    {
        public void UpdateRender();
        public void Render();
    }
}
