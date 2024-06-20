using System.Collections.Concurrent;
namespace VoxelPrototype.client.World.Level
{
    internal class RenderThread
    {
        volatile bool Running = false;
        Thread Thread;
        ConcurrentQueue<SectionToMeshing> SectionToBeMesh = new();
        internal int SectionToBeMeshCount 
        { 
            get
            {
                return SectionToBeMesh.Count;
            } 
        }
        public RenderThread()
        {
            Thread = new Thread(RenderThreadMethod);
            Thread.Name = "Render Thread";
        }
        public void Start()
        {
            Running = true;
            Thread.Start();
        }
        internal  void AddSectionToBeMesh(SectionToMeshing sectionToMeshing)
        {
            SectionToBeMesh.Enqueue(sectionToMeshing);
        }
        volatile int Counter = 0;
        public void RenderThreadMethod()
        {
            while (Running)
            {
                if(Counter > 500)
                {
                    SectionToBeMesh = new(SectionToBeMesh.OrderBy(x => x.Importance));
                    Counter = 0;
                }
                for (int i = 0; i<20;i++)
                {
                    if (SectionToBeMesh.TryDequeue(out SectionToMeshing section))
                    {
                        if (Client.TheClient.World.IsChunkSurrended(section.Pos.Xz))
                        {
                            section.Generator.Generate();
                            Client.TheClient.World.ChunkManager.AddSectionToOG(new()
                            {
                                Section = section.Pos,
                                OpaqueVertices = section.Generator.OpaqueVertices.ToArray(),
                                OpaqueIndices = section.Generator.OpaqueIndices.ToArray()
                            });
                        }
                        else
                        {
                            //if(section.NumberOfIt <100)
                           // {
                                section.Importance  = 10000;
                               // section.NumberOfIt++;
                                SectionToBeMesh.Enqueue(section);
                            //}
                        }
                    }
                    Counter++;

                }
            }
        }
        public void Stop()
        {
            Running = false;
        }
    }
}
