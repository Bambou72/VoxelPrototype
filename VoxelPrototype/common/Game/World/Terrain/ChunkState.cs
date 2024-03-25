namespace VoxelPrototype.common.Game.World.Terrain
{
    [Flags]
    internal enum ChunkSate
    {
        None = 0,
        Ready = 1,
        Changed = 2,
        Unsaved = 4,
    }
    internal enum ServerChunkSate
    {
        None,
        Dirty,
    }
}
