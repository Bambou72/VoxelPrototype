using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Concurrent;
using VoxelPrototype.common.Game.World;
using VoxelPrototype.common.Physics;
namespace VoxelPrototype.client.Render.World
{
    internal class WorldRenderer
    {
        internal Dictionary<Vector2i, ChunkMesh> ChunksMesh = new();
        internal static ConcurrentQueue<ChunkToBeMesh> ChunkToBeMesh = new();
        internal static ConcurrentQueue<ChunkMeshGenerator> ChunkToBeOG = new();
        internal Collider ChunkCollider = new(new Vector3d(0,0,0),new Vector3d(32,32,32));
        internal int RenderableChunkCount { get { return ChunksMesh.Count; } }
        internal int RenderedChunksCount = 0;
        internal void Dispose()
        {
            ChunkToBeMesh.Clear();
            ChunkToBeOG.Clear();
        }
        internal void AddChunkToBeMesh(Chunk ch, int prior)
        {
            if (!ChunkToBeMesh.Contains(new() { ch = ch, prior = prior }) && Vector2.Distance((Vector2)Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Xz,ch.Position) <= Client.TheClient.World.RenderDistance *Chunk.Size)
            {
                ChunkToBeMesh.Enqueue(new() { ch = ch, prior = prior });

            }
        }
        internal void Render()
        {
            RenderedChunksCount = 0;
            var Shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/chunk"));
            Shader.SetMatrix4("view", Client.TheClient.World.GetLocalPlayerCamera().GetViewMatrix());
            Shader.SetMatrix4("projection", Client.TheClient.World.GetLocalPlayerCamera().GetProjectionMatrix());
            Client.TheClient.TextureManager.GetTexture(new Resources.ResourceID("textures/block/atlas")).Use(TextureUnit.Texture0);
            int minx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Chunk.Size) - Client.TheClient.World.RenderDistance;
            int minz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Chunk.Size) - Client.TheClient.World.RenderDistance;
            int maxx = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.X / Chunk.Size) + Client.TheClient.World.RenderDistance;
            int maxz = (int)(Client.TheClient.World.PlayerFactory.LocalPlayer.Position.Z / Chunk.Size) + Client.TheClient.World.RenderDistance;
            /*Frustum Frustum = FrustumCulling.CreateFrustumFromCamera(
                Client.TheClient.World.GetLocalPlayerCamera(),
                ClientAPI.AspectRatio(),
                MathHelper.DegreesToRadians(OptionMenu.FrustumFov),
                Client.TheClient.World.GetLocalPlayerCamera().Near,
                Client.TheClient.World.GetLocalPlayerCamera().Far);*/
            foreach (var pos in ChunksMesh.Keys)
            {
                if (pos.X >= minx && pos.Y >= minz && pos.X <= maxx && pos.Y <= maxz)
                {
                    var mesh = ChunksMesh[pos];
                    if(mesh.VertC != 0)
                    {
                        //if(FrustumCulling.AABBIntersect(Frustum,ChunkCollider.Move(new Vector3(pos.X * Chunk.Size, pos.Y * Chunk.Size, pos.Z * Chunk.Size))))
                        //{
                            RenderedChunksCount++;
                            Matrix4 model = Matrix4.CreateTranslation(new Vector3(pos.X * Chunk.Size,0,pos.Y * Chunk.Size));
                            Shader.SetMatrix4("model", model);
                            Shader.Use();
                            GL.BindVertexArray(mesh.VAO);
                            GL.DrawElements(PrimitiveType.Triangles, mesh.IndC, DrawElementsType.UnsignedInt, 0);
                            GL.BindVertexArray(0);
                        //}
                    }
                }
            }
        }
        internal void RemoveChunkMesh(Vector2i pos)
        {
            if (ChunksMesh.ContainsKey(pos))
            {
                var mesh = ChunksMesh[pos];
                GL.DeleteVertexArray(mesh.VAO);
                GL.DeleteBuffer(mesh.VBO);
                GL.DeleteBuffer(mesh.EBO);
                ChunksMesh.Remove(pos);
            }
        }
        internal void Update()
        {
            GenerateMesh();
            for (int i = 0; i < 10; i++)
            {
                if (ChunkToBeOG.Count > 0)
                {
                    if (ChunkToBeOG.TryDequeue(out ChunkMeshGenerator chunkMeshGenerator))
                    {
                        if (ChunksMesh.ContainsKey(chunkMeshGenerator.pos))
                        {
                            RemoveChunkMesh(chunkMeshGenerator.pos);
                            ChunksMesh.Add(chunkMeshGenerator.pos, chunkMeshGenerator.GenerateOG());
                        }
                        else
                        {
                            ChunksMesh.Add(chunkMeshGenerator.pos, chunkMeshGenerator.GenerateOG());
                        }
                    }
                    else
                    {
                        ChunkToBeOG.Enqueue(chunkMeshGenerator);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        private static bool isGeneratingMesh = false;
        internal static void GenerateMesh()
        {
            ChunkToBeMesh = new ConcurrentQueue<ChunkToBeMesh>(ChunkToBeMesh.OrderBy(x => x.prior));
            if (ChunkToBeMesh.Count > 0 && !isGeneratingMesh)
            {
                if (!isGeneratingMesh)
                {
                    isGeneratingMesh = true;
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        while (ChunkToBeMesh.Count > 0)
                        {
                            if (ChunkToBeMesh.TryDequeue(out var chunk))
                            {
                                var MeshCreator = new ChunkMeshGenerator();
                                if(  chunk.ch.State.HasFlag(ChunkSate.Changed) /*&& !chunk.ch.Empty*/)
                                {
                                    MeshCreator.GenerateChunkMesh(chunk.ch);
                                    if(MeshCreator.Vertices.Count != 0)
                                    {
                                        ChunkToBeOG.Enqueue(MeshCreator);
                                    }
                                }
                            }
                        }
                        isGeneratingMesh = false;
                    });
                }
            }
        }
    }
    internal struct ChunkToBeMesh
    {
        internal Chunk ch;
        internal int prior;
    }
}
