using VoxelPrototype.API;
using VoxelPrototype.API.Commands;
using VoxelPrototype.common.Base.Commands;

namespace VoxelPrototype.common.Base
{
    internal class Base : IMod
    {
        public string Name => "Base";

        public string Description => "";

        public string Version => "";

        public void DeInit()
        {
        }

        public void Init()
        {
            CommandRegister.RegisterCommand(new Tp());
            CommandRegister.RegisterCommand(new Fly());
            CommandRegister.RegisterCommand(new Ghost());
            CommandRegister.RegisterCommand(new Set());
            CommandRegister.RegisterCommand(new TPS());
        }
    }
}
