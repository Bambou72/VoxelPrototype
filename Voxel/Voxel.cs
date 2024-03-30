using Voxel.Blocks;
using Voxel.Commands;
using Voxel.Generators;
using VoxelPrototype.API;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.Commands;
using VoxelPrototype.API.WorldGenerator;

namespace Voxel
{
    public class Voxel : IMod
    {
        public string Name => "Voxel";
        public string Description => "Voxel base game";
        public string Version => "0.1";
        public void DeInit()
        {
        }
        public void Init()
        {
            string DirtID = BlockRegister.GetBlockID("Voxel", "Dirt");
            var Dirt = new Block()
            {
                Collider = "Voxel@block/cube",
                Model = "Voxel@block/cube",
                Data = "Voxel@block/dirt"
            };
            BlockRegister.RegisterBlock(DirtID, Dirt);
            string GrassID = BlockRegister.GetBlockID("Voxel", "Grass");
            var Grass = new Block()
            {
                Collider = "Voxel@block/cube",
                Model = "Voxel@block/cube",
                Data = "Voxel@block/grass"
            };
            BlockRegister.RegisterBlock(GrassID, Grass);
            string LampID = BlockRegister.GetBlockID("Voxel", "Lamp");
            var Lamp = new Lamp()
            {
                Collider = "Voxel@block/cube",
                Model = "Voxel@block/cube",
                Data = "Voxel@block/lamp"
            };
            BlockRegister.RegisterBlock(LampID, Lamp);
            //
            //Commands
            //
            CommandRegister.RegisterCommand(new Tp());
            CommandRegister.RegisterCommand(new Fly());
            CommandRegister.RegisterCommand(new Ghost());
            CommandRegister.RegisterCommand(new Set());
            //
            //World Generators
            //
            WorldGeneratorRegistry.RegisterWorldGenerator("Complex", typeof(ComplexGenerator));
            WorldGeneratorRegistry.RegisterWorldGenerator("Flat", typeof(FlatGenerator));
        }
    }
}
