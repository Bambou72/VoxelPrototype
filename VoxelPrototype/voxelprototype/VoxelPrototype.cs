using VoxelPrototype.api;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Commands;
using VoxelPrototype.api.WorldGenerator;
using VoxelPrototype.utils;
using VoxelPrototype.voxelprototype.command;
using VoxelPrototype.voxelprototype.generator;

namespace VoxelPrototype.voxelprototype
{
    public class VoxelPrototype : IModInitializer
    {
        public const string Name = "voxelprototype";
        public const string Description = "voxelprototype base game";
        public const string Version = "0.1";
        public void PreInit(ModManager Manager)
        {
        }
        public void Init(ModManager Manager)
        {
            BlockRegistry BRegistry = BlockRegistry.GetInstance();
            string DirtID = BRegistry.GetBlockID(Name, "dirt");
            var Dirt = new Block()
            {
                Data = "data/block/dirt"
            };
            BRegistry.RegisterBlock(DirtID, Dirt);
            string GrassID = BRegistry.GetBlockID(Name, "grass");
            var Grass = new Block()
            {
                Data = "data/block/grass"
            };
            BRegistry.RegisterBlock(GrassID, Grass);
            string StoneID = BRegistry.GetBlockID(Name, "stone");
            var Stone = new Block()
            {
                Data = "data/block/stone"
            };
            BRegistry.RegisterBlock(StoneID, Stone);
            string CobblestoneID = BRegistry.GetBlockID(Name, "cobblestone");
            var Cobblestone = new Block()
            {
                Data = "data/block/cobblestone"
            };
            BRegistry.RegisterBlock(CobblestoneID, Cobblestone);
            string LampID = BRegistry.GetBlockID(Name, "lamp");
            var Lamp = new block.Lamp()
            {
                Data ="data/block/lamp"
            };
            BRegistry.RegisterBlock(LampID, Lamp);
            //
            //World Generators
            //
            WorldGeneratorRegistry WRegistry = WorldGeneratorRegistry.GetInstance();
            WRegistry.RegisterWorldGenerator("Complex", typeof(ComplexGenerator));
            WRegistry.RegisterWorldGenerator("Flat", typeof(FlatGenerator));
            //
            //Commands
            //
            CommandRegistry CRegistry = CommandRegistry.GetInstance();
            CRegistry.RegisterCommand(new Tp());
            CRegistry.RegisterCommand(new Fly());
            CRegistry.RegisterCommand(new Ghost());
            CRegistry.RegisterCommand(new Set());
            CRegistry.RegisterCommand(new TPS());
        }
        public void DeInit(ModManager Manager)
        {
        }

        public string GetModName()
        {
            return Name;
        }

        public string GetModVersion()
        {
            return Version;
        }

        public string GetModDescription()
        {
            return Description;
        }
    }
}
