using MethodTimer;
using NLog.LayoutRenderers;
using OpenTK.Mathematics;
using System.Net.Http.Json;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.common.Blocks.State;
namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal class SectionMeshGenerator
    {

        internal Section Section;
        Vector3i Position;
        internal List<SectionVertex> OpaqueVertices;
        internal List<uint> OpaqueIndices;
        uint IndexCounter;
        public const byte ADJACENT_BITMASK_POS_Y = 1 << 0;      // 000001
        public const byte ADJACENT_BITMASK_NEG_Y = 1 << 1;   // 000010
        public const byte ADJACENT_BITMASK_NEG_X = 1 << 2;     // 000100
        public const byte ADJACENT_BITMASK_POS_X = 1 << 3;    // 001000
        public const byte ADJACENT_BITMASK_POS_Z = 1 << 4;    // 010000
        public const byte ADJACENT_BITMASK_NEG_Z = 1 << 5;     // 100000

        public const byte ADJACENT_BITMASK_FULL = ADJACENT_BITMASK_POS_Y | ADJACENT_BITMASK_NEG_Y | ADJACENT_BITMASK_NEG_X | ADJACENT_BITMASK_POS_X | ADJACENT_BITMASK_POS_Z | ADJACENT_BITMASK_NEG_Z; // 111111

        

        public SectionMeshGenerator(Section section)
        {
            Section = section;
            Position = new Vector3i(section.Chunk.X,section.Y,section.Chunk.Z);
        }

        public byte GetNeigbours(Vector3i position,Block CurrentBlock)
        {
            Vector3i GlobalPos = new Vector3i(Position.X * Const.ChunkSize,Position.Y *Section.Size,Position.Z * Const.ChunkSize) + position;
            byte BitMask= 0;
            //UP
            var BlockUp = Client.TheClient.World.GetBlock(GlobalPos+new Vector3i(0,1,0)).Block;
            if( !BlockUp.Transparent ||(CurrentBlock.Cullself&& BlockUp.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_Y;
            }
            //DOWN
            var BlockDown = Client.TheClient.World.GetBlock(GlobalPos + new Vector3i(0, -1, 0)).Block;
            if (!BlockDown.Transparent || (CurrentBlock.Cullself && BlockDown.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_Y;
            }
            //RIGHT
            var BlockRight = Client.TheClient.World.GetBlock(GlobalPos + new Vector3i(1, 0, 0)).Block;
            if (!BlockRight.Transparent || (CurrentBlock.Cullself && BlockRight.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_X;
            }
            //LEFT
            var BlockLeft = Client.TheClient.World.GetBlock(GlobalPos + new Vector3i(-1, 0, 0)).Block;
            if (!BlockLeft.Transparent || (CurrentBlock.Cullself && BlockLeft.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_X;
            }
            //FRONT
            var BlockFront = Client.TheClient.World.GetBlock(GlobalPos + new Vector3i(0, 0, -1)).Block;
            if (!BlockFront.Transparent || (CurrentBlock.Cullself && BlockFront.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_Z;
            }
            //BACK
            var BlockBack = Client.TheClient.World.GetBlock(GlobalPos + new Vector3i(0, 0,1)).Block;
            if (!BlockBack.Transparent || (CurrentBlock.Cullself && BlockBack.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_Z;
            }
            return BitMask;
        }
        //[Time]
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
                            byte BitMask = GetNeigbours(BPosition,block.Block);
                            if(BitMask == ADJACENT_BITMASK_FULL)
                            {
                                continue;
                            }


                            if (block.Block.RenderType == BlockRenderType.Cube)
                            {
                                GenerateDirection(BitMask, BPosition, block);
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
        private void GenerateDirection(byte BitMask, Vector3i BlockPos, BlockState Current)
        {
            //Up
            if ((BitMask & ADJACENT_BITMASK_POS_Y) == 0)
            {
                AddMeshFace(Current, BlockPos, 0, true);
            }
            //Bottom
            if ((BitMask & ADJACENT_BITMASK_NEG_Y) == 0)
            {
                AddMeshFace(Current, BlockPos, 1, true);
            }
            //Right
            if ((BitMask & ADJACENT_BITMASK_POS_X) == 0)
            {
                AddMeshFace(Current, BlockPos, 4, true);
            }
            //Left
            if ((BitMask & ADJACENT_BITMASK_NEG_X) == 0)
            {
                AddMeshFace(Current, BlockPos, 5, true);
            }
            //Front
            if ((BitMask & ADJACENT_BITMASK_POS_Z) == 0)
            {
                AddMeshFace(Current, BlockPos, 3, true);
            }
            //Back
            if ((BitMask & ADJACENT_BITMASK_NEG_Z) == 0)
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
            byte[] AO = { 0, 0, 0, 0 };
            bool flip_id = false;
            if (ao)
            {
                AO = BF switch
                {
                    0 => AOBlockPos.Y < Section.Size * Const.ChunkHeight ? CalculateAO(AOBlockPos + new Vector3i(0, 1, 0), 1) : new byte[] { 0, 0, 0, 0 },
                    1 => AOBlockPos.Y > 0 ?  CalculateAO(AOBlockPos + new Vector3i(0, -1, 0), 1) :new byte[]{ 0, 0, 0, 0 } ,
                    2 => CalculateAO(AOBlockPos + new Vector3i(0, 0, 1), 2),
                    3 => CalculateAO(AOBlockPos + new Vector3i(0, 0, -1), 2),
                    4 => CalculateAO(AOBlockPos + new Vector3i(1, 0, 0), 0),
                    _ => CalculateAO(AOBlockPos + new Vector3i(-1, 0, 0), 0),
                };
                flip_id = AO[1] + AO[3] > AO[0] + AO[2];

            }
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
        private byte[] CalculateAO(Vector3i bpos, int plane)
        {
            byte a, b, c, d, e, f, g, h;
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
            byte[] AO =
            { 
                //BL
                (byte)(a == 0 && c == 0 ? 0 : a+b+c),
                //BR
                (byte)( a == 0 && g == 0 ? 0 : g+h+a),
                //BR
                 (byte)(e == 0 && g == 0 ? 0 :e+f+g),
                //BL
                 (byte)(c == 0 && e == 0 ? 0 :c+d+e)
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
