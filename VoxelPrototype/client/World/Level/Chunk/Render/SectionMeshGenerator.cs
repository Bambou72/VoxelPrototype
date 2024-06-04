using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.World;
using VoxelPrototype.client.World.Level.Chunk;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.common.Blocks.State;
using VoxelPrototype.common.World;

namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal class SectionMeshGenerator
    {
        Section Section;
        Vector3i Position;
        internal List<SectionVertex> OpaqueVertices;
        internal List<uint> OpaqueIndices;
        uint IndexCounter;

        public SectionMeshGenerator(Section section, Vector3i position)
        {
            Section = section;
            Position = position;
        }

        public Block[] GetNeigbours(Vector3i position)
        {
            Block[] Neighbours = new Block[6];
            int chunkSizeMinusOne = Section.Size - 1;

            // Up
            if (position.Y < Section.Size - 1)
            {
                Neighbours[0] = Section.BlockPalette.Get(new Vector3i(position.X, position.Y + 1, position.Z)).Block;
            }
            else if (Position.Y < Const.ChunkHeight - 1)
            {
                Neighbours[0] = Section.Chunk.Sections[Position.Y + 1].BlockPalette.Get(new Vector3i(position.X, 0, position.Z)).Block;
            }

            // Down
            if (position.Y > 0)
            {
                Neighbours[1] = Section.BlockPalette.Get(new Vector3i(position.X, position.Y - 1, position.Z)).Block;
            }
            else if (Position.Y > 0)
            {
                Neighbours[1] = Section.Chunk.Sections[Position.Y - 1].BlockPalette.Get(new Vector3i(position.X, Section.Size - 1, position.Z)).Block;
            }

            // Right
            if (position.X < chunkSizeMinusOne)
            {
                Neighbours[2] = Section.BlockPalette.Get(new Vector3i(position.X + 1, position.Y, position.Z)).Block;
            }
            else
            {
                Chunk rightChunk = Client.TheClient.World.GetChunk(Position.X + 1, Position.Z);
                Neighbours[2] = rightChunk.GetBlockFast(new Vector3i(0, position.Y + Section.Size * Position.Y, position.Z)).Block;
            }

            // Left
            if (position.X > 0)
            {
                Neighbours[3] = Section.BlockPalette.Get(new Vector3i(position.X - 1, position.Y, position.Z)).Block;
            }
            else
            {
                Chunk leftChunk = Client.TheClient.World.GetChunk(Position.X - 1, Position.Z);
                Neighbours[3] = leftChunk.GetBlockFast(new Vector3i(chunkSizeMinusOne, position.Y + Section.Size * Position.Y, position.Z)).Block;
            }

            // Front
            if (position.Z > 0)
            {
                Neighbours[4] = Section.BlockPalette.Get(new Vector3i(position.X, position.Y, position.Z - 1)).Block;
            }
            else
            {
                Chunk frontChunk = Client.TheClient.World.GetChunk(Position.X, Position.Z - 1);
                Neighbours[4] = frontChunk.GetBlockFast(new Vector3i(position.X, position.Y + Section.Size * Position.Y, chunkSizeMinusOne)).Block;
            }

            // Back
            if (position.Z < chunkSizeMinusOne)
            {
                Neighbours[5] = Section.BlockPalette.Get(new Vector3i(position.X, position.Y, position.Z + 1)).Block;
            }
            else
            {
                Chunk backChunk = Client.TheClient.World.GetChunk(Position.X, Position.Z + 1);
                Neighbours[5] = backChunk.GetBlockFast(new Vector3i(position.X, position.Y + Section.Size * Position.Y, 0)).Block;
            }

            return Neighbours;
        }
        internal void Generate()
        {
            OpaqueVertices = new();
            OpaqueIndices = new();
            IndexCounter = 0;
            for (int x = 0; x < Section.Size; x++)
            {
                for (int y = 0; y < Section.Size; y++)
                {
                    for (int z = 0; z < Section.Size; z++)
                    {

                        Vector3i BPosition = new Vector3i(x, y, z);
                        var block = Section.BlockPalette.Get(BPosition);
                        if (block != Client.TheClient.ModManager.BlockRegister.Air)
                        {
                            Block[] Neighbours = GetNeigbours(BPosition);
                            if (block.Block.RenderType == BlockRenderType.Cube)
                            {
                                GenerateDirection(Neighbours, BPosition, block);
                            }
                            else
                            {
                                var blockMesh = Client.TheClient.ModelManager.GetBlockMesh(block.Block.Model);
                                for (int i = 0; i < blockMesh.GetMesh().Length; i++)
                                {
                                    AddMeshFace(block, BPosition, i, false);
                                }
                            }
                        }
                        
                    }
                }
            }
        }
        private void GenerateDirection(Block[] Neighboor, Vector3i BlockPos, BlockState Current)
        {
            //Up
            if (Neighboor[0] != null)
            {
                if (Neighboor[0].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 0, true);

                }
            }
            else
            {
                AddMeshFace(Current, BlockPos, 0, true);

            }
            //Bottom
            if (Neighboor[1] != null)
            {
                if (Neighboor[1].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 1, true);
                }
            }
            else
            {
                if (Position.Y > 0)
                {
                    AddMeshFace(Current, BlockPos, 1, true);
                }
            }
            //Right
            if (Neighboor[2] != null)
            {
                if (Neighboor[2].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 4, true);
                }
            }
            else
            {
                AddMeshFace(Current, BlockPos, 4, true);

            }
            //Left
            if (Neighboor[3] != null)
            {
                if (Neighboor[3].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 5, true);
                }
            }
            else
            {
                AddMeshFace(Current, BlockPos, 5, true);

            }
            //Front
            if (Neighboor[4] != null)
            {
                if (Neighboor[4].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 3, true);
                }
            }
            else
            {
                AddMeshFace(Current, BlockPos, 3, true);

            }
            //Back
            if (Neighboor[5] != null)
            {
                if (Neighboor[5].Transparent)
                {
                    AddMeshFace(Current, BlockPos, 2, true);
                }
            }
            else
            {
                AddMeshFace(Current, BlockPos, 2, true);

            }
        }
        private void AddMeshFace(BlockState Block, Vector3i BlockPos, int BF, bool ao)
        {
            float[] Vert = Block.Block.GetMesh(BF);
            float[] Uv = Block.Block.GetMeshTextureCoordinates(BF);
            float[] Tex = Block.Block.GetTextureCoordinates(BF, Block);
            Vector3i AOBlockPos = BlockPos + new Vector3i(0, Section.Y * Section.Size, 0);
            int[] AO = BF switch
            {
                0 => CalculateAO(AOBlockPos + new Vector3i(0, 1, 0), 1),
                1 => CalculateAO(AOBlockPos + new Vector3i(0, -1, 0), 1),
                2 => CalculateAO(AOBlockPos + new Vector3i(0, 0, 1), 2),
                3 => CalculateAO(AOBlockPos + new Vector3i(0, 0, -1), 2),
                4 => CalculateAO(AOBlockPos + new Vector3i(1, 0, 0), 0),
                _ => CalculateAO(AOBlockPos + new Vector3i(-1, 0, 0), 0),
            };
            bool flip_id = AO[1] + AO[3] > AO[0] + AO[2];
            SectionVertex SVert;
            for (int i = flip_id ? 1 : 0; i < 4; i++)
            {
                SVert = new()
                {
                    Position = new Vector3(Vert[i * 3] + BlockPos.X, Vert[i * 3 + 1] + BlockPos.Y, Vert[i * 3 + 2] + BlockPos.Z),
                    Uv = new Vector2((Tex[4] - Tex[0]) * Uv[i * 2] + Tex[0], (Tex[3] - Tex[1]) * Uv[i * 2 + 1] + Tex[1]),
                    AO = ao ? AO[i] : 3
                };
                OpaqueVertices.Add(SVert);
            }
            if (flip_id)
            {
                SVert = new()
                {
                    Position = new Vector3(Vert[0] + BlockPos.X, Vert[1] + BlockPos.Y, Vert[2] + BlockPos.Z),
                    Uv = new Vector2((Tex[4] - Tex[0]) * Uv[0 * 2] + Tex[0], (Tex[3] - Tex[1]) * Uv[0 * 2 + 1] + Tex[1]),
                    AO = ao ? AO[0] : 3
                };
                OpaqueVertices.Add(SVert);
            }
            uint[] indice = { 0, 1, 2, 2, 3, 0 };
            for (uint i = 0; i < 6; i++)
            {
                indice[i] += IndexCounter;
            }
            OpaqueIndices.AddRange(indice);
            IndexCounter += 4;

        }
        private int[] CalculateAO(Vector3i bpos, int plane)
        {
            int a, b, c, d, e, f, g, h;
            if (plane == 1)
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Position.Xz));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z - 1), Position.Xz));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Position.Xz));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z + 1), Position.Xz));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Position.Xz));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z + 1), Position.Xz));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Position.Xz));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z - 1), Position.Xz));
            }
            else if (plane == 0)
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z - 1), Position.Xz));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z - 1), Position.Xz));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Position.Xz));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z + 1), Position.Xz));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y, bpos.Z + 1), Position.Xz));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z + 1), Position.Xz));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Position.Xz));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z - 1), Position.Xz));
            }
            else
            {
                a = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y, bpos.Z), Position.Xz));
                b = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y - 1, bpos.Z), Position.Xz));
                c = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y - 1, bpos.Z), Position.Xz));
                d = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y - 1, bpos.Z), Position.Xz));
                e = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y, bpos.Z), Position.Xz));
                f = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X + 1, bpos.Y + 1, bpos.Z), Position.Xz));
                g = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X, bpos.Y + 1, bpos.Z), Position.Xz));
                h = Client.TheClient.ModManager.BlockRegister.GetTransForAO(Client.TheClient.World.ChunkManager.GetBlockForMesh(new Vector3i(bpos.X - 1, bpos.Y + 1, bpos.Z), Position.Xz));
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
        internal void Clear()
        {
            OpaqueIndices.Clear();
            OpaqueIndices = null;
            OpaqueVertices.Clear();
            OpaqueVertices = null;
        }
    }
}
