using System.Collections.Concurrent;
using System.Diagnostics;
using VoxelPrototype.client.game.world.Level.Chunk.Render;

namespace VoxelPrototype.client.game.world.Level
{
    internal class MeshingThread
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        volatile bool Running = false;
        internal volatile bool Stopped = false;
        Thread Thread;
        ConcurrentQueue<SectionToMeshing> SectionToBeMesh = new();
        SectionMeshGenerator SectionMeshGenerator = new();
        // Compteur pour le nombre d'événements
        int eventCount = 0;
        internal int SectionToBeMeshCount
        {
            get
            {
                return SectionToBeMesh.Count;
            }
        }
        public MeshingThread()
        {
            Thread = new Thread(RenderThreadMethod);
            Thread.Name = "MeshingThread";
            Thread.IsBackground = true;
            Thread.Priority = ThreadPriority.Normal;
        }
        public void Clear()
        {
            SectionToBeMesh.Clear();
        }
        public void Start()
        {
            Running = true;
            Stopped = false;
            Thread.Start();
        }
        internal void AddSectionToBeMesh(SectionToMeshing sectionToMeshing)
        {
            SectionToBeMesh.Enqueue(sectionToMeshing);
        }
        public void RenderThreadMethod()
        {

            try
            {
                while (Running)
                {
                    SectionToBeMesh = new(SectionToBeMesh.OrderBy(x => x.Importance));
                    for (int i = 0; i < 200; i++)
                    {
                        if (Running)
                        {
                            if (SectionToBeMesh.TryDequeue(out SectionToMeshing section))
                            {
                                Client.TheClient.World.ChunkManager.ChunkByCoordinateLock.EnterReadLock();
                                SectionMeshGenerator.Generate(section.Pos);
                                Client.TheClient.World.ChunkManager.ChunkByCoordinateLock.ExitReadLock();
                                Client.TheClient.World.ChunkManager.AddSectionToOG(new()
                                {
                                    Section = section.Pos,
                                    OpaqueVertices = SectionMeshGenerator.OpaqueVertices.ToArray(),
                                    OpaqueIndices = SectionMeshGenerator.OpaqueIndices.ToArray()
                                });
                                SectionMeshGenerator.Clean();
                            }

                        }
                        else
                        {
                            Stopped = true;
                            return;
                        }
                    }

                }
                Stopped = true;
                return;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }
        public void Stop()
        {
            Running = false;
        }
    }
}
