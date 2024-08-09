using VoxelPrototype.api.block;
using VoxelPrototype.api.block.state;
using VoxelPrototype.api.block.state.properties;
namespace VoxelPrototype.voxelprototype.block
{
    internal class Mandelbrot : Block
    {
        internal static IntegerProperty Color = new("Color", 0, 15, 0);
        public override void RegisterProperties(BlockStateBuilder Builder)
        {
            base.RegisterProperties(Builder);
            Builder.Register(Color);
        }
        public override uint GetColor(BlockState Sate)
        {
            switch(Sate.Get(Color))
            {
                case 0:
                    return 0x000000FF;
                case 1:
                    return 0xFF0000FF;
                case 2:
                    return 0xFF7F00FF;
                case 3:
                    return 0xFFFF00FF;
                case 4:
                    return 0x7FFF00FF;
                case 5:
                    return 0x00FF00FF;
                case 6:
                    return 0x00FF7FFF;
                case 7:
                    return 0x00FFFFFF;
                case 8:
                    return 0x007FFFFF;
                case 9:
                    return 0x0000FFFF;
                case 10:
                    return 0x7F00FFFF;
                case 11:
                    return 0xFF00FFFF;
                case 12:
                    return 0xFF007FFF;
                case 13:
                    return 0xFFD700FF;
                case 14:
                    return 0x8B4513FF;
                case 15:
                    return 0x9400D3FF;
                default:
                    return 0x000000FF;
            }
        }
    }
}
