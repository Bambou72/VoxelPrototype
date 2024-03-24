using Voxel.Commands;
using Voxel.Generators;
using VoxelPrototype.common.API;
using VoxelPrototype.common.API.Blocks;
using VoxelPrototype.common.API.Commands;
using VoxelPrototype.common.API.WorldGenerator;
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
                Collider = "Voxel@Colliders/Blocks/Cube",
                Model = "Voxel@Meshs/Blocks/Cube",
                Data = "Voxel@Data/Blocks/Dirt"
            };
            BlockRegister.RegisterBlock(DirtID, Dirt);
            string GrassID = BlockRegister.GetBlockID("Voxel", "Grass");
            var Grass = new Block()
            {
                Collider = "Voxel@Colliders/Blocks/Cube",
                Model = "Voxel@Meshs/Blocks/Cube",
                Data = "Voxel@Data/Blocks/Grass"
            };
            BlockRegister.RegisterBlock(GrassID, Grass);
            //
            //Commands
            //
            CommandRegister.RegisterCommand(new Tp());
            CommandRegister.RegisterCommand(new Fly());
            CommandRegister.RegisterCommand(new Ghost());
            //
            //World Generators
            //
            WorldGeneratorRegistry.RegisterWorldGenerator("Complex", typeof(ComplexGenerator));
            WorldGeneratorRegistry.RegisterWorldGenerator("Flat", typeof(FlatGenerator));
        }
    }
}
