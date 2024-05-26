using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoxelPrototype.api.Blocks;
using VoxelPrototype.api.Blocks.State;
using VoxelPrototype.client.Render.UI;
using VoxelPrototype.common.World;
namespace VoxelPrototype.client.Render.World
{
    public class ChunkMeshGenerator
    {
        internal List<ChunkVertex> Vertices;
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
        private (int, int, int) GenerateVAO(ChunkVertex[] Vertices)
        {
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            var ChunkShader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/chunk"));
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Unsafe.SizeOf<ChunkVertex>(), Vertices, BufferUsageHint.StaticDraw);
            var vertexLocation = ChunkShader.GetAttribLocation("Vertex");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ChunkVertex>(), 0);
            var texCoordLocation = ChunkShader.GetAttribLocation("Texture");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<ChunkVertex>(), Marshal.OffsetOf<ChunkVertex>("Uv"));
            var AOLocation = ChunkShader.GetAttribLocation("AO");
            GL.EnableVertexAttribArray(AOLocation);
            GL.VertexAttribIPointer(AOLocation, 1, VertexAttribIntegerType.Int, Unsafe.SizeOf<ChunkVertex>(), Marshal.OffsetOf<ChunkVertex>("AO"));
            
            //EBO
            int EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);
            return (VAO, VBO, EBO);
        }
        //[Time]

        //0: Up,1:Bottom,2:right,3:left,4:Front,5:Back

        public Block[] GetNeigboors(Block[] Neighboors, Chunk chunk, Vector3i position)
        {
            int chunkSizeMinusOne = Chunk.Size - 1;
            int chunkHeightTimesSectionSize = Chunk.Height * Section.Size;

            // Up
            if (position.Y < chunkHeightTimesSectionSize)
            {
                Neighboors[0] = chunk.GetBlockFast(new Vector3i(position.X, position.Y + 1, position.Z)).Block;
            }

            // Down
            if (position.Y > 0)
            {
                Neighboors[1] = chunk.GetBlockFast(new Vector3i(position.X, position.Y - 1, position.Z)).Block;
            }

            // Right
            if (position.X < chunkSizeMinusOne)
            {
                Neighboors[2] = chunk.GetBlockFast(new Vector3i(position.X + 1, position.Y, position.Z)).Block;
            }
            else
            {
                Chunk rightChunk = Client.TheClient.World.GetChunk(chunk.X + 1, chunk.Z);
                Neighboors[2] = rightChunk.GetBlockFast(new Vector3i(0, position.Y, position.Z)).Block;
            }

            // Left
            if (position.X > 0)
            {
                Neighboors[3] = chunk.GetBlockFast(new Vector3i(position.X - 1, position.Y, position.Z)).Block;
            }
            else
            {
                Chunk leftChunk = Client.TheClient.World.GetChunk(chunk.X - 1, chunk.Z);
                Neighboors[3] = leftChunk.GetBlockFast(new Vector3i(chunkSizeMinusOne, position.Y, position.Z)).Block;
            }

            // Front
            if (position.Z > 0)
            {
                Neighboors[4] = chunk.GetBlockFast(new Vector3i(position.X, position.Y, position.Z - 1)).Block;
            }
            else
            {
                Chunk frontChunk = Client.TheClient.World.GetChunk(chunk.X, chunk.Z - 1);
                Neighboors[4] = frontChunk.GetBlockFast(new Vector3i(position.X, position.Y, chunkSizeMinusOne)).Block;
            }

            // Back
            if (position.Z < chunkSizeMinusOne)
            {
                Neighboors[5] = chunk.GetBlockFast(new Vector3i(position.X, position.Y, position.Z + 1)).Block;
            }
            else
            {
                Chunk backChunk = Client.TheClient.World.GetChunk(chunk.X, chunk.Z + 1);
                Neighboors[5] = backChunk.GetBlockFast(new Vector3i(position.X, position.Y, 0)).Block;
            }

            return Neighboors;
        }
        public void GenerateChunkMesh(Chunk Chunk)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            pos = Chunk.Position;
            Vertices = new();
            Indices = new();
            IndexCounter = 0;
            Ch = Chunk;
            Block[] Neighbors = new Block[6];
            for(int x = 0; x < Chunk.Size; x++)
            {
                for (int y = 0; y < Chunk.Height * Section.Size; y++)
                {
                    for (int z = 0; z < Chunk.Size; z++)
                    {
                        Vector3i BPosition = new Vector3i(x, y, z);
                        var block = Chunk.GetBlockFast(BPosition);
                        if (block != Client.TheClient.ModManager.BlockRegister.Air)
                        {
                            Block[] Neighboors = GetNeigboors(Neighbors,Chunk, BPosition);
                            if (block.Block.RenderType == BlockRenderType.Cube)
                            {
                                GenerateDirection(Neighboors, Chunk, BPosition, block);
                            }
                            else
                            {
                                var blockMesh = Client.TheClient.ModelManager.GetBlockMesh(block.Block.Model);
                                for (int i = 0; i < blockMesh.GetMesh().Length; i++)
                                {
                                    AddMeshFace(block, Chunk, BPosition, i, false);
                                }
                            }
                        }
                    }
                }
            }
            //watch.Stop();
            //Console.WriteLine("chunk:" + watch.ElapsedMilliseconds);

        }
        private void GenerateDirection(Block[] Neighboor, Chunk chunk,Vector3i BlockPos,BlockState Current)
        {
            //Up
            if (Neighboor[0] != null)
            {
                if (Neighboor[0].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 0, true);
                }
            }
            else
            {
                AddMeshFace(Current, chunk, BlockPos, 0, true);

            }
            //Bottom
            if (Neighboor[1] != null)
            {
                if (Neighboor[1].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 1, true);
                }
            }
            else
            {
                if(BlockPos.Y >0)
                {
                    AddMeshFace(Current, chunk, BlockPos, 1, true);
                }
            }
            //Right
            if (Neighboor[2] != null)
            {
                if (Neighboor[2].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 4, true);
                }
            }
            else
            {
                AddMeshFace(Current, chunk, BlockPos, 4, true);

            }
            //Left
            if (Neighboor[3] != null)
            {
                if (Neighboor[3].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 5, true);
                }
            }
            else
            {
                AddMeshFace(Current, chunk, BlockPos, 5, true);

            }
            //Front
            if (Neighboor[4] != null)
            {
                if (Neighboor[4].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 3, true);
                }
            }
            else
            {
                AddMeshFace(Current, chunk, BlockPos, 3, true);

            }
            //Back
            if (Neighboor[5] != null)
            {
                if (Neighboor[5].Transparency)
                {
                    AddMeshFace(Current, chunk, BlockPos, 2, true);
                }
            }
            else
            {
                AddMeshFace(Current, chunk, BlockPos, 2, true);

            }
        }
        private void AddMeshFace(BlockState Block,Chunk chunk, Vector3i BlockPos, int BF, bool ao)
        {

            float[] Vert = Block.Block.GetMesh(BF);
            float[] Uv = Block.Block.GetMeshTextureCoordinates(BF);
            float[] Tex = Block.Block.GetTextureCoordinates(BF, Block);
            int[] AO = BF switch
            {
                0 => CalculateAO(chunk, BlockPos + new Vector3i(0, 1, 0), 1),
                1 => CalculateAO(chunk, BlockPos + new Vector3i(0, -1, 0), 1),
                2 => CalculateAO(chunk, BlockPos + new Vector3i(0, 0, 1), 2),
                3 => CalculateAO(chunk, BlockPos + new Vector3i(0, 0, -1), 2),
                4 => CalculateAO(chunk, BlockPos + new Vector3i(1, 0, 0), 0),
                _ => CalculateAO(chunk, BlockPos + new Vector3i(-1, 0, 0), 0),
            };
            bool flip_id = AO[1] + AO[3] > AO[0] + AO[2];
            ChunkVertex CVert;
            for (int i = flip_id ? 1 : 0; i < 4; i++)
            {
                CVert= new()
                {
                    Position = new Vector3(Vert[i * 3] + BlockPos.X, Vert[i * 3 + 1] + BlockPos.Y, Vert[i * 3 + 2] + BlockPos.Z),
                    Uv = new Vector2((Tex[4] - Tex[0]) * Uv[i * 2] + Tex[0], (Tex[3] - Tex[1]) * Uv[i * 2 + 1] + Tex[1]),
                    AO = ao ? AO[i] : 3
                };
                Vertices.Add(CVert);
            }
            if (flip_id)
            {
                CVert = new()
                {
                    Position = new Vector3(Vert[0] + BlockPos.X, Vert[1] + BlockPos.Y, Vert[2] + BlockPos.Z),
                    Uv = new Vector2((Tex[4] - Tex[0]) * Uv[0 * 2] + Tex[0],(Tex[3] - Tex[1]) * Uv[0 * 2 + 1] + Tex[1]),
                    AO = ao ? AO[0] : 3
                };
                Vertices.Add(CVert);
            }
            uint[] indice = { 0, 1, 2, 2, 3, 0 };
            for (uint i = 0; i < 6; i++)
            {
                indice[i] += IndexCounter;
            }
            Indices.AddRange(indice);
            IndexCounter += 4;

        }
        private static int[] CalculateAO(Chunk Ch, Vector3i bpos, int plane)
        {
            int a, b, c, d, e, f, g, h;
            if (plane == 1)
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Ch.Position));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z - 1), Ch.Position));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Ch.Position));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z + 1), Ch.Position));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Ch.Position));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z + 1), Ch.Position));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Ch.Position));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z - 1), Ch.Position));
            }
            else if (plane == 0)
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Ch.Position));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z - 1), Ch.Position));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Ch.Position));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z + 1), Ch.Position));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Ch.Position));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z + 1), Ch.Position));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Ch.Position));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z - 1), Ch.Position));
            }
            else
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Ch.Position));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y - 1, bpos.Z), Ch.Position));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Ch.Position));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y - 1, bpos.Z), Ch.Position));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Ch.Position));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y + 1, bpos.Z), Ch.Position));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Ch.Position));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y + 1, bpos.Z), Ch.Position));
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
