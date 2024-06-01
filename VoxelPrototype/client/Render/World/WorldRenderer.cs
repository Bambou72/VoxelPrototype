using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using VoxelPrototype.common.Physics;
using VoxelPrototype.common.World;
namespace VoxelPrototype.client.Render.World
{
    internal class WorldRenderer
    {
        internal Dictionary<Vector3i, SectionMesh> SectionMesh = new();
        ConcurrentQueue<SectionMesh> SectionsToBeGenerate = new();
        ConcurrentQueue<SectionMesh> SectionsToBeFinalize = new();
        Queue<SectionMesh> SectionsToBeDestroy = new();

        internal Collider ChunkCollider = new(new Vector3d(0, 0, 0), new Vector3d(32, 32, 32));
        internal int RenderableChunkCount { get { return SectionMesh.Count; } }
        internal int RenderedChunksCount = 0;
        private static bool isGeneratingMesh = false;
        internal static bool DisposeVar = false;

        internal void Dispose()
        {
            DisposeVar = true;
            foreach (var sectionmesh in SectionMesh.Values) 
            {
                sectionmesh.Destroy();
            }
            SectionMesh.Clear();
        }
        internal void AddSection(Section section)
        {
            if(!section.Empty)
            {
                var Pos = new Vector3i(section.Chunk.X,section.Y,section.Chunk.Z);
                SectionMesh[Pos] = new SectionMesh(Pos, section);
            }
        }
        internal void GenerateSection(Section section)
        {
            var Pos = new Vector3i(section.Chunk.X, section.Y, section.Chunk.Z);
            if (!SectionMesh.ContainsKey(Pos))
            {
               AddSection(section);
            }
            if (!section.Empty)
            {
                var sec = SectionMesh[Pos];
                if (!SectionsToBeGenerate.Contains(sec))
                {
                    SectionsToBeGenerate.Enqueue(sec);
                }
            }
        }
        internal void DestroySection(Section section)
        {
            var Pos = new Vector3i(section.Chunk.X, section.Y, section.Chunk.Z);
            if (SectionMesh.ContainsKey(Pos))
            {
                SectionsToBeDestroy.Enqueue(SectionMesh[Pos]);
            }
        }
        internal void Render()
        {
            RenderedChunksCount = 0;
            var Shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/chunk"));
            var Camera = Client.TheClient.World.GetLocalPlayerCamera();
            Camera.Update();
            Shader.SetMatrix4("view", Camera.GetViewMatrix());
            Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());
            Client.TheClient.TextureManager.GetTexture(new Resources.ResourceID("textures/block/atlas")).Use(TextureUnit.Texture0);
            int minx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Chunk.Size) - Client.TheClient.World.RenderDistance;
            int minz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Chunk.Size) - Client.TheClient.World.RenderDistance;
            int maxx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Chunk.Size) + Client.TheClient.World.RenderDistance;
            int maxz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Chunk.Size) + Client.TheClient.World.RenderDistance;
            Shader.Use();
            /*Frustum Frustum = FrustumCulling.CreateFrustumFromCamera(
                Client.TheClient.World.GetLocalPlayerCamera(),
                ClientAPI.AspectRatio(),
                MathHelper.DegreesToRadians(OptionMenu.FrustumFov),
                Client.TheClient.World.GetLocalPlayerCamera().Near,
                Client.TheClient.World.GetLocalPlayerCamera().Far);*/
            foreach (var pos in SectionMesh.Keys)
            {
                if(pos.X >= minx && pos.Z >= minz && pos.X <= maxx && pos.Z <= maxz)
                {
                    var mesh = SectionMesh[pos];
                    if (mesh.GetOpaqueMesh().GetVerticesCount() != 0 && Camera.Frustum.IsSectionInFrustum(mesh))
                    {
                        //if(FrustumCulling.AABBIntersect(Frustum,ChunkCollider.Move(new Vector3(pos.X * Chunk.Size, pos.Y * Chunk.Size, pos.Z * Chunk.Size))))
                        //{
                        RenderedChunksCount++;
                        Matrix4 model = Matrix4.CreateTranslation(new Vector3(pos.X * Chunk.Size, pos.Y * Section.Size, pos.Z * Chunk.Size));
                        Shader.SetMatrix4("model", model);
 
                        GL.BindVertexArray(mesh.GetOpaqueMesh().GetVAO());
                        GL.DrawElements(PrimitiveType.Triangles, mesh.GetOpaqueMesh().GetIndicesCount(), DrawElementsType.UnsignedInt, 0);
                        GL.BindVertexArray(0);
                        //}
                    }
                }
            }
        }
        internal void Update()
        {
            //MeshGeneration
            if (SectionsToBeGenerate.Count > 0 && !isGeneratingMesh)
            {
                ThreadPool.QueueUserWorkItem(async state =>
                {
                    List<SectionMesh> ToReQueue = new List<SectionMesh>();
                    isGeneratingMesh = true;
                    List<Task> tasks = new List<Task>();
                    for(int i =  0;i<100;i++)
                    {
                        if (SectionsToBeGenerate.TryDequeue(out var section))
                        {
                            if ( section.Section.Chunk.IsSurrendedClient())
                            {
                                if(!section.Section.Empty)
                                {
                                    tasks.Add(Task.Run(() =>
                                    {
                                        section.Generate();
                                        SectionsToBeFinalize.Enqueue(section);
                                    }));
                                }
                            }else
                            {
                                ToReQueue.Add(section);
                            }
                        }
                    }
                    foreach(var section in ToReQueue) 
                    {
                        SectionsToBeGenerate.Enqueue(section);
                    }
                    await Task.WhenAll(tasks);
                    isGeneratingMesh = false;
                });
            }
            //Finalize
            while(SectionsToBeFinalize.Count > 0)
            {
                if (SectionsToBeFinalize.TryDequeue(out var Sec))
                {
                    if(Sec != null)
                    {
                        Sec.Upload();
                    }
                }
            }
            //Destroy
            while (SectionsToBeDestroy.Count > 0)
            {
                if (SectionsToBeDestroy.TryDequeue(out var Sec))
                {
                    if( Sec != null) 
                    {
                        Sec.Destroy();
                        SectionMesh.Remove(Sec.Position);
                        Sec = null;
                    }

                }
            }
        }
    }
}
