using VoxelPrototype.client.Resources;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.game.Generators;
using VoxelPrototype.common;
using VoxelPrototype.game.Commands;

namespace VoxelPrototype.game
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
            string StoneID = Manager.BlockRegister.GetBlockID(Name, "stone");
            var Stone = new Block()
            {
                Model = new ResourceID("models/block/cube"),
                Data = new ResourceID("data/block/stone")
            };
            Manager.BlockRegister.RegisterBlock(StoneID, Stone);

            string CobblestoneID = Manager.BlockRegister.GetBlockID(Name, "cobblestone");
            var Cobblestone = new Block()
            {
                Model = new ResourceID("models/block/cube"),
                Data = new ResourceID("data/block/cobblestone")
            };
            Manager.BlockRegister.RegisterBlock(CobblestoneID, Cobblestone);
            string LampID = Manager.BlockRegister.GetBlockID(Name, "lamp");
            var Lamp = new Blocks.Lamp()
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
