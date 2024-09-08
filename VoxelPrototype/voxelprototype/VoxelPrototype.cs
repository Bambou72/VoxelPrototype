using VoxelPrototype.api;
using VoxelPrototype.api.block;
using VoxelPrototype.api.command;
using VoxelPrototype.api.worldgeneration;
using VoxelPrototype.utils;
using VoxelPrototype.voxelprototype.command;
using VoxelPrototype.voxelprototype.generator;

namespace VoxelPrototype.voxelprototype
{
    public class VoxelPrototype : IModInitializer
    {
        public const string Name = "VoxelPrototype";
        public const string Description = "voxelprototype base game";
        public const string Version = "0.1";
        public const string Namespace = "vp";
        public void PreInit(ModManager Manager)
        {
        }
        public void Init(ModManager Manager)
        {
            BlockRegistry BRegistry = BlockRegistry.GetInstance();
            var Dirt = new Block();
            BRegistry.RegisterBlock(Namespace+":dirt", Dirt);
            var Grass = new Block();
            BRegistry.RegisterBlock(Namespace + ":grass", Grass);
            var Stone = new Block();
            BRegistry.RegisterBlock(Namespace + ":stone", Stone);
            var Cobblestone = new Block();
            BRegistry.RegisterBlock(Namespace + ":cobblestone", Cobblestone);
            var Lamp = new block.Lamp();
            BRegistry.RegisterBlock(Namespace + ":lamp", Lamp);
            var mandelbrot = new block.Mandelbrot();
            BRegistry.RegisterBlock(Namespace + ":mandelbrot", mandelbrot);
            //
            //
            //World Generators
            //
            WorldGeneratorRegistry WRegistry = WorldGeneratorRegistry.GetInstance();
            WRegistry.RegisterWorldType(new ComplexWorldType());
            WRegistry.RegisterWorldType(new FlatWorldType());
            WRegistry.RegisterWorldType(new MandelbrotWorldType());
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
        public string GetModNamespace()
        {
            return Namespace;
        }
    }
}
