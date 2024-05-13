using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.API.Blocks;
using VoxelPrototype.API.Blocks.State;
using VoxelPrototype.common.Game.World;
namespace VoxelPrototype.client.Render.World
{
    public class ChunkMeshGenerator
    {
        internal List<float> Vertices;
        List<uint> Indices;
        uint IndexCounter = 0;
        internal Vector2i pos;
        internal Chunk Ch;
        //[Time]
        internal ChunkMesh GenerateOG()
        {
            if (Vertices.Count == 0)
            {
                return new ChunkMesh();
            }
            int VerticeCount = Vertices.Count;
            int IndexCount = Indices.Count;
            int VAO, VBO, EBO;
            (VAO, VBO, EBO) = GenerateVAO(Vertices.ToArray());
            Vertices.Clear();
            Indices.Clear();
            Ch.State &= ~ChunkSate.Changed;
            return new ChunkMesh() { VAO = VAO, VBO = VBO, EBO = EBO, IndC = IndexCount, VertC = VerticeCount };
        }
        private (int, int, int) GenerateVAO(float[] Vertices)
        {
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            var vertexLocation = Client.TheClient.ResourcePackManager.GetShader("Voxel@chunk").GetAttribLocation("Vertex");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var texCoordLocation = Client.TheClient.ResourcePackManager.GetShader("Voxel@chunk").GetAttribLocation("Texture");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            var AOLocation = Client.TheClient.ResourcePackManager.GetShader("Voxel@chunk").GetAttribLocation("AO");
            GL.EnableVertexAttribArray(AOLocation);
            GL.VertexAttribPointer(AOLocation, 1, VertexAttribPointerType.Float, false, 6 * sizeof(float), 5 * sizeof(float));
            //EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
            return (VAO, VBO, EBO);
        }
        //[Time]
        public void GenerateChunkMesh(Chunk Chunk)
        {
            pos = Chunk.Position;
            Vertices = new();
            Indices = new();
            IndexCounter = 0;
            Ch = Chunk;
            Parallel.For(0, Chunk.Size, x =>
            {
                for (int y = 0; y < Chunk.Height *Section.Size; y++)
                {
                    for (int z = 0; z < Chunk.Size; z++)
                    {
                        var block = Chunk.GetBlock(new Vector3i(x, y, z));
                        if (block !=  BlockRegister.Air)
                        {
                            if (BlockRegister.Blocks[block.Block.ID].RenderType == BlockRenderType.Cube)
                            {
                                GenerateDirection(new Vector3i(x, y, z));
                            }
                            else
                            {
                                var blockMesh = Client.TheClient.ResourcePackManager.GetBlockMesh(BlockRegister.Blocks[block.Block.ID].Model);
                                var meshLength = blockMesh.GetMesh().Length;
                                for (int i = 0; i < meshLength; i++)
                                {
                                    AddMeshFace(block, new Vector3i((int)x, y, z), i, false);
                                }
                            }
                        }
                    }
                }
            });
        }
        private bool IsTransparent(int dx, int dy, int dz, Vector3i blockPos)
        {
            BlockState ID = Ch.GetBlock(blockPos + new Vector3i(dx, dy, dz));
            if (ID == BlockRegister.Air)
            {
                return true;
            }
            return BlockRegister.Blocks[ID.Block.ID].Transparency;
        }

        private bool IsMeshBlockTransparent(Vector2i ChunkPos, Vector3i BlockPos)
        {
            Chunk Ch = Client.TheClient.World.GetChunk(ChunkPos.X,ChunkPos.Y);
            if(Ch == null)
            {
                return true;
            }else
            {
                return Ch.GetBlock(BlockPos).Block.Transparency;
            }
        }
        private void GenerateDirection(Vector3i BlockPos)
        {
            var block = Ch.GetBlock(BlockPos);
            //
            //X
            //
            if (BlockPos.X == Chunk.Size - 1)
            {
                if (IsMeshBlockTransparent(new Vector2i(Ch.Position.X + 1, Ch.Position.Y), new Vector3i(0, BlockPos.Y, BlockPos.Z)))
                {
                    AddMeshFace( block, BlockPos, 4, true);
                }
            }
            else if (IsTransparent(1, 0, 0, BlockPos))
            {
                AddMeshFace( block, BlockPos, 4, true);
            }
            if (BlockPos.X == 0)
            {
                if (IsMeshBlockTransparent(new Vector2i(Ch.Position.X - 1, Ch.Position.Y), new Vector3i(Chunk.Size - 1, BlockPos.Y, BlockPos.Z)))
                {
                    AddMeshFace( block, BlockPos, 5, true);
                }
            }
            else if (IsTransparent(-1, 0, 0, BlockPos))
            {
                AddMeshFace( block, BlockPos, 5, true);
            }
            //
            //Y
            //
            if (BlockPos.Y == (Chunk.Height * Section.Size) - 1 )
            {
                AddMeshFace( block, BlockPos, 0, true);
            }
            else if (IsTransparent(0, 1, 0, BlockPos))
            {
                AddMeshFace( block, BlockPos, 0, true);
            }
            if (BlockPos.Y == 0)
            {
                AddMeshFace( block, BlockPos, 1, true);
            }
            else if(IsTransparent(0, -1, 0, BlockPos))
            {
                AddMeshFace( block,BlockPos, 1, true);
            }
            //
            //Z
            //
            if (BlockPos.Z == Chunk.Size - 1)
            {
                if (IsMeshBlockTransparent(new Vector2i(Ch.Position.X, Ch.Position.Y + 1),new Vector3i(BlockPos.X, BlockPos.Y, 0)))
                {
                    AddMeshFace( block, BlockPos, 2, true);
                }
            }
            else if (IsTransparent(0, 0, 1, BlockPos))
            {
                AddMeshFace( block, BlockPos, 2, true);
            }
            if (BlockPos.Z == 0)
            {
                if (IsMeshBlockTransparent(new Vector2i(Ch.Position.X, Ch.Position.Y - 1),new Vector3i(BlockPos.X, BlockPos.Y, Chunk.Size - 1)))
                {
                    AddMeshFace( block, BlockPos, 3, true);
                }
            }
            else if (IsTransparent(0, 0, -1, BlockPos))
            {
                AddMeshFace(block, BlockPos, 3, true);
            }
        }
        private void AddMeshFace(BlockState Block,Vector3i BlockPos, int BF, bool ao)
        {
            float[] Vert = Block.Block.GetMesh(BF);
            float[] Uv = Block.Block.GetMeshTextureCoordinates(BF);
            float[] Tex = Block.Block.GetTextureCoordinates(BF,Block);
            int[] AO = BF switch
            {
                0 => CalculateAO(Ch, BlockPos + new Vector3i(0,1,0),1),
                1 => CalculateAO(Ch, BlockPos + new Vector3i(0,-1,0),1),
                2 => CalculateAO(Ch, BlockPos + new Vector3i(0,0,1),2),
                3 => CalculateAO(Ch, BlockPos + new Vector3i(0,0,-1),2),
                4 => CalculateAO(Ch, BlockPos + new Vector3i(1,0,0),0),
                _ => CalculateAO(Ch, BlockPos + new Vector3i(-1,0,0),0),
            };
            bool flip_id = AO[1] + AO[3] > AO[0] + AO[2];
            bool addedv = false;
            while (!addedv)
            {
                lock (Vertices)
                {
                    for (int i = flip_id ? 1 : 0; i < 4; i++)
                    {
                        Vertices.Add(Vert[i * 3] + BlockPos.X);
                        Vertices.Add(Vert[i * 3 + 1] + BlockPos.Y);
                        Vertices.Add(Vert[i * 3 + 2] + BlockPos.Z);
                        Vertices.Add((Tex[4] - Tex[0]) * Uv[i * 2] + Tex[0]);
                        Vertices.Add((Tex[3] - Tex[1]) * Uv[i * 2 + 1] + Tex[1]);
                        Vertices.Add(ao ? AO[i] : 3);
                    }
                    if (flip_id)
                    {
                        Vertices.Add(Vert[0 * 3] + BlockPos.X);
                        Vertices.Add(Vert[0 * 3 + 1] + BlockPos.Y);
                        Vertices.Add(Vert[0 * 3 + 2] + BlockPos.Z);
                        Vertices.Add((Tex[4] - Tex[0]) * Uv[0 * 2] + Tex[0]);
                        Vertices.Add((Tex[3] - Tex[1]) * Uv[0 * 2 + 1] + Tex[1]);
                        Vertices.Add(ao ? AO[0] : 3);
                    }
                    addedv = true;
                }
            }
            bool added = false;
            while (!added)
            {
                lock (Indices)
                {
                    uint[] indice = new uint[] { 0, 1, 2, 2, 3, 0 };
                    for (uint i = 0; i < 6; i++)
                    {
                        indice[i] += IndexCounter;
                    }
                    Indices.AddRange(indice);
                    IndexCounter += 4;
                    added = true;
                }
            }
        }
        private static int[] CalculateAO(Chunk Ch, Vector3i bpos, int plane)
        {
            int a, b, c, d, e, f, g, h;
            if (plane == 1)
            {
                a = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Ch.Position));
                b = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z - 1), Ch.Position));
                c = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Ch.Position));
                d = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z + 1), Ch.Position));
                e = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Ch.Position));
                f = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z + 1), Ch.Position));
                g = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Ch.Position));
                h = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z - 1), Ch.Position));
            }
            else if (plane == 0)
            {
                a = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Ch.Position));
                b = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z - 1), Ch.Position));
                c = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Ch.Position));
                d = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z + 1), Ch.Position));
                e = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Ch.Position));
                f = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z + 1), Ch.Position));
                g = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Ch.Position));
                h = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z - 1), Ch.Position));
            }
            else
            {
                a = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Ch.Position));
                b = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y - 1, bpos.Z), Ch.Position));
                c = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Ch.Position));
                d = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y - 1, bpos.Z), Ch.Position));
                e = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Ch.Position));
                f = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y + 1, bpos.Z), Ch.Position));
                g = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Ch.Position));
                h = BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y + 1, bpos.Z), Ch.Position));
            }
            int[] AO = new int[4]
            { 
                //BL
                a == 0 && c == 0 ? 0 : a+b+c,
                //BR
                a == 0 && g == 0 ? 0 : g+h+a,
                //BR
                e == 0 && g == 0 ? 0 :e+f+g,
                //BL
                c == 0 && e == 0 ? 0 :c+d+e
            };
            return AO;
        }
    }
}
