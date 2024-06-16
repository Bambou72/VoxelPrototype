using K4os.Compression.LZ4;
using OpenTK.Mathematics;
using VoxelPrototype.VBF;
namespace VoxelPrototype.server.World.Level
{
    // TODO : Add a defragmentation algorithm
    public class RegionFile
    {
        public VBFCompound root;
        string path;
        public RegionFile(string Path)
        {
            path = Path;
            if (File.Exists(Path))
            {
                root = (VBFCompound)VBFSerializer.Deserialize(LZ4Pickler.Unpickle(File.ReadAllBytes(path)));
            }
            else
            {
                root = new VBFCompound();
                File.WriteAllBytes(Path,VBFSerializer.Serialize(root));
            }
        }
        internal VBFCompound? GetChunk(Vector2i pos)
        {
            return root.GetCompound(pos.X + ":" + pos.Y);
        }
        internal void SetChunk(Vector2i pos,VBFCompound data)
        {
            root[pos.X + ":" + pos.Y] = data;
            Save();
        }
        internal void Save()
        {
            File.WriteAllBytes(path,LZ4Pickler.Pickle(VBFSerializer.Serialize(root),LZ4Level.L06_HC));
        }
    }
}
