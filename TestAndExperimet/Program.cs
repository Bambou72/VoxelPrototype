using VoxelPrototype.common.Game.Entities;
using VoxelPrototype.common.Game.World.Terrain;

class Program
{
    static unsafe void Main(string[] args)
    {
        
        RegionFile Test = new("test.vpr");
        Test.SectorsFree.ForEach(number => Console.WriteLine(number));
        Test.WriteOffset(0, 0, 251006, 25);
        (int offset,byte size) = Test.GetOffset(0, 0);
        Console.WriteLine("Localization:" + offset);
        Console.WriteLine("Size :" + size);
        Test.Close();
    }
}