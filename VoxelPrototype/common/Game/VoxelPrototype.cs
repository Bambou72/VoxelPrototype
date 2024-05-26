
using VoxelPrototype.api;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.client.Resources;
using VoxelPrototype.api.Commands;
using VoxelPrototype.common.Game.Generators;
using VoxelPrototype.common.Game.Blocks;
using VoxelPrototype.common.Game.Commands;
namespace VoxelPrototype.common.Game
{
    public class VoxelPrototype : IMod
    {
        public string Name => "voxelprototype";
        public string Description => "voxelprototype base game";
        public string Version => "0.1";
        public void PreInit(ModManager Manager)
        {
        }
        public void Init(ModManager Manager)
        {
            string DirtID = Manager.BlockRegister.GetBlockID(Name, "dirt");
            var Dirt = new Block()
            {
                Model = new ResourceID("models/block/cube"),
                Data = new ResourceID("data/block/dirt")
            };
            Manager.BlockRegister.RegisterBlock(DirtID, Dirt);
            string GrassID = Manager.BlockRegister.GetBlockID(Name, "grass");
            var Grass = new Block()
            {
                Model = new ResourceID("models/block/cube"),
                Data = new ResourceID("data/block/grass")
            };
            Manager.BlockRegister.RegisterBlock(GrassID, Grass);
            string LampID = Manager.BlockRegister.GetBlockID(Name, "lamp");
            var Lamp = new Lamp()
            {
                Model = new ResourceID("models/block/cube"),
                Data = new ResourceID("data/block/lamp")
            };
            Manager.BlockRegister.RegisterBlock(LampID, Lamp);

            //
            //World Generators
            //
            Manager.WorldGeneratorRegistry.RegisterWorldGenerator("Complex", typeof(ComplexGenerator));
            Manager.WorldGeneratorRegistry.RegisterWorldGenerator("Flat", typeof(FlatGenerator));
            //
            //Commands
            //
            Manager.CommandRegister.RegisterCommand(new Tp());
            Manager.CommandRegister.RegisterCommand(new Fly());
            Manager.CommandRegister.RegisterCommand(new Ghost());
            Manager.CommandRegister.RegisterCommand(new Set());
            Manager.CommandRegister.RegisterCommand(new TPS());
        }
        public void DeInit(ModManager Manager)
        {
        }
    }
}
