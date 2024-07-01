using MethodTimer;
using OpenTK.Mathematics;
using VoxelPrototype.common;
using VoxelPrototype.common.Blocks;
using VoxelPrototype.common.Blocks.State;
namespace VoxelPrototype.client.World.Level.Chunk.Render
{
    internal class SectionMeshGenerator
    {
        internal List<SectionVertex> OpaqueVertices;
        internal List<uint> OpaqueIndices;
        SectionMeshDataCache Data;
        uint IndexCounter;
        // Constants representing the bit positions of adjacent blocks
        public const uint ADJACENT_BITMASK_POS_Y = 1 << 0;      // 00000000000000000000000001
        public const uint ADJACENT_BITMASK_NEG_Y = 1 << 1;      // 00000000000000000000000010
        public const uint ADJACENT_BITMASK_NEG_X = 1 << 2;      // 00000000000000000000000100
        public const uint ADJACENT_BITMASK_POS_X = 1 << 3;      // 00000000000000000000001000
        public const uint ADJACENT_BITMASK_POS_Z = 1 << 4;      // 00000000000000000000010000
        public const uint ADJACENT_BITMASK_NEG_Z = 1 << 5;      // 00000000000000000000100000

        // Constants representing edge adjacent blocks
        public const uint ADJACENT_BITMASK_POS_Y_NEG_X = 1 << 6;   // 00000000000000000001000000
        public const uint ADJACENT_BITMASK_POS_Y_POS_X = 1 << 7;   // 00000000000000000010000000
        public const uint ADJACENT_BITMASK_POS_Y_NEG_Z = 1 << 8;   // 00000000000000000100000000
        public const uint ADJACENT_BITMASK_POS_Y_POS_Z = 1 << 9;   // 00000000000000001000000000
        public const uint ADJACENT_BITMASK_NEG_Y_NEG_X = 1 << 10;  // 00000000000000010000000000
        public const uint ADJACENT_BITMASK_NEG_Y_POS_X = 1 << 11;  // 00000000000000100000000000
        public const uint ADJACENT_BITMASK_NEG_Y_NEG_Z = 1 << 12;  // 00000000000001000000000000
        public const uint ADJACENT_BITMASK_NEG_Y_POS_Z = 1 << 13;  // 00000000000010000000000000
        public const uint ADJACENT_BITMASK_POS_X_NEG_Z = 1 << 14;  // 00000000000100000000000000
        public const uint ADJACENT_BITMASK_POS_X_POS_Z = 1 << 15;  // 00000000001000000000000000
        public const uint ADJACENT_BITMASK_NEG_X_NEG_Z = 1 << 16;  // 00000000010000000000000000
        public const uint ADJACENT_BITMASK_NEG_X_POS_Z = 1 << 17;  // 00000000100000000000000000

        // Constants representing corner adjacent blocks
        public const uint ADJACENT_BITMASK_POS_Y_NEG_X_NEG_Z = 1 << 18; // 00000001000000000000000000
        public const uint ADJACENT_BITMASK_POS_Y_NEG_X_POS_Z = 1 << 19; // 00000010000000000000000000
        public const uint ADJACENT_BITMASK_POS_Y_POS_X_NEG_Z = 1 << 20; // 00000100000000000000000000
        public const uint ADJACENT_BITMASK_POS_Y_POS_X_POS_Z = 1 << 21; // 00001000000000000000000000
        public const uint ADJACENT_BITMASK_NEG_Y_NEG_X_NEG_Z = 1 << 22; // 00010000000000000000000000
        public const uint ADJACENT_BITMASK_NEG_Y_NEG_X_POS_Z = 1 << 23; // 00100000000000000000000000
        public const uint ADJACENT_BITMASK_NEG_Y_POS_X_NEG_Z = 1 << 24; // 01000000000000000000000000
        public const uint ADJACENT_BITMASK_NEG_Y_POS_X_POS_Z = 1 << 25; // 10000000000000000000000000

        public const uint ADJACENT_BITMASK_FULL = ADJACENT_BITMASK_POS_Y | ADJACENT_BITMASK_NEG_Y | ADJACENT_BITMASK_NEG_X | ADJACENT_BITMASK_POS_X | ADJACENT_BITMASK_POS_Z | ADJACENT_BITMASK_NEG_Z; // 111111

        



        public uint GetNeighbours(Vector3i position,Block CurrentBlock,bool AO)
        {
            uint BitMask = 0;
            //UP
            var BlockUp = Data.GetBlock(position+new Vector3i(0,1,0)).Block;
            if( !BlockUp.Transparent ||(CurrentBlock.Cullself&& BlockUp.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_Y;
            }
            //DOWN
            var BlockDown = Data.GetBlock(position + new Vector3i(0, -1, 0)).Block;
            if (!BlockDown.Transparent || (CurrentBlock.Cullself && BlockDown.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_Y;
            }
            //RIGHT
            var BlockRight = Data.GetBlock(position + new Vector3i(1, 0, 0)).Block;
            if (!BlockRight.Transparent || (CurrentBlock.Cullself && BlockRight.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_X;
            }
            //LEFT
            var BlockLeft = Data.GetBlock(position + new Vector3i(-1, 0, 0)).Block;
            if (!BlockLeft.Transparent || (CurrentBlock.Cullself && BlockLeft.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_X;
            }
            //FRONT
            var BlockFront = Data.GetBlock(position + new Vector3i(0, 0, -1)).Block;
            if (!BlockFront.Transparent || (CurrentBlock.Cullself && BlockFront.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_NEG_Z;
            }
            //BACK
            var BlockBack = Data.GetBlock(position + new Vector3i(0, 0,1)).Block;
            if (!BlockBack.Transparent || (CurrentBlock.Cullself && BlockBack.ID == CurrentBlock.ID))
            {
                BitMask |= ADJACENT_BITMASK_POS_Z;
            }
            // If AO is true, check edge and corner adjacent blocks
            if (!(BitMask == ADJACENT_BITMASK_FULL) && AO)
            {
                // Edge adjacent blocks
                if (!Data.GetBlock(position + new Vector3i(1, 1, 0)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_POS_X;
                if (!Data.GetBlock(position + new Vector3i(-1, 1, 0)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_NEG_X;
                if (!Data.GetBlock(position + new Vector3i(0, 1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(0, 1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(1, -1, 0)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_POS_X;
                if (!Data.GetBlock(position + new Vector3i(-1, -1, 0)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_NEG_X;
                if (!Data.GetBlock(position + new Vector3i(0, -1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(0, -1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(1, 0, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_X_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(1, 0, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_X_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, 0, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_X_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, 0, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_X_POS_Z;
                // Corner adjacent blocks
                if (!Data.GetBlock(position + new Vector3i(1, 1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_POS_X_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(1, 1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_POS_X_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, 1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_NEG_X_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, 1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_POS_Y_NEG_X_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(1, -1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_POS_X_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(1, -1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_POS_X_NEG_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, -1, 1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_NEG_X_POS_Z;
                if (!Data.GetBlock(position + new Vector3i(-1, -1, -1)).Block.Transparent)
                    BitMask |= ADJACENT_BITMASK_NEG_Y_NEG_X_NEG_Z;
            }
            return BitMask;
        }
        [Time]
        internal void Generate(SectionMeshDataCache Data)
        {
            this.Data = Data;
            //Data.LockChunk();
            OpaqueVertices = new();
            OpaqueIndices = new();
            IndexCounter = 0;
            for (int x = 0; x < Const.SectionSize; x++)
            {
                for (int y = 0; y < Const.SectionSize; y++)
                {
                    for (int z = 0; z < Const.SectionSize; z++)
                    {

                        Vector3i BPosition = new Vector3i(x, y, z);
                        var block = Data.GetBlock(BPosition);
                        if (block != Client.TheClient.ModManager.BlockRegister.Air)
                        {
                            uint BitMask = GetNeighbours(BPosition,block.Block,true);
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
                                    AddMeshFace(BitMask,block, BPosition, i, false);
                                }
                            }
                        }
                        
                    }
                }
            }
            //Data.UnLockChunk();
        }
        private void GenerateDirection(uint BitMask, Vector3i BlockPos, BlockState Current)
        {
            //Up
            if ((BitMask & ADJACENT_BITMASK_POS_Y) == 0)
            {
                AddMeshFace(BitMask,Current, BlockPos, 0, true);
            }
            //Bottom
            if ((BitMask & ADJACENT_BITMASK_NEG_Y) == 0)
            {
                AddMeshFace(BitMask, Current, BlockPos, 1, true);
            }
            //Right
            if ((BitMask & ADJACENT_BITMASK_POS_X) == 0)
            {
                AddMeshFace(BitMask, Current, BlockPos, 4, true);
            }
            //Left
            if ((BitMask & ADJACENT_BITMASK_NEG_X) == 0)
            {
                AddMeshFace(BitMask, Current, BlockPos, 5, true);
            }
            //Front
            if ((BitMask & ADJACENT_BITMASK_NEG_Z) == 0)
            {
                AddMeshFace(BitMask, Current, BlockPos, 3, true);
            }
            //Back
            if ((BitMask & ADJACENT_BITMASK_POS_Z) == 0)
            {
                AddMeshFace(BitMask, Current, BlockPos, 2, true);
            }
        }
        private void AddMeshFace(uint BitMask ,BlockState Block, Vector3i BlockPos, int BF, bool ao)
        {
            float[] Vert = Block.Block.GetMesh(BF);
            float[] Uv = Block.Block.GetMeshTextureCoordinates(BF);
            float[] Tex = Block.Block.GetTextureCoordinates(BF, Block);
            Vector3i AOBlockPos = BlockPos + new Vector3i(0,Data.CurrentSection.Y * Const.SectionSize, 0);
            byte[] AO = { 0, 0, 0, 0 };
            bool flip_id = false;
            if (ao)
            {
                AO = BF switch
                {
                    0 => AOBlockPos.Y < Const.SectionSize * Const.ChunkHeight ? CalculateAO(1, 1,BitMask) : new byte[] { 0, 0, 0, 0 },
                    1 => AOBlockPos.Y > 0 ?  CalculateAO(  -1, 1,BitMask) :new byte[]{ 0, 0, 0, 0 } ,
                    2 => CalculateAO(1, 2,BitMask),
                    3 => CalculateAO( -1, 2,BitMask),
                    4 => CalculateAO(1, 0,BitMask),
                    _ => CalculateAO(-1, 0,BitMask),
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
        private byte[] CalculateAO(int offset, int plane,uint BitMask)
        {
            int a, b, c, d, e, f, g, h;
            if (plane == 1)
            {
                if(offset ==1)
                {
                    a =( BitMask & ADJACENT_BITMASK_POS_Y_NEG_Z )!= 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_POS_Y_POS_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_NEG_Z) != 0 ? 0 : 1;

                }
                else
                {
                    a = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_Z) != 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_NEG_Z) != 0 ? 0 : 1;

                }
            }
            else if (plane == 0)
            {
                if (offset == 1)
                {
                    a = (BitMask & ADJACENT_BITMASK_POS_X_NEG_Z) != 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_NEG_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_POS_X_POS_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_NEG_Z) != 0 ? 0 : 1;
                }
                else
                {
                    a = (BitMask & ADJACENT_BITMASK_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_NEG_X_POS_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;

                }
            }
            else
            {
                if (offset == 1)
                {
                    a = (BitMask & ADJACENT_BITMASK_NEG_X_POS_Z) != 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_Z) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_POS_X_POS_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_POS_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_POS_Y_POS_Z) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_POS_Z) != 0 ? 0 : 1;
                }
                else
                {
                    a = (BitMask & ADJACENT_BITMASK_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    b = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;
                    c = (BitMask & ADJACENT_BITMASK_NEG_Y_NEG_Z) != 0 ? 0 : 1;
                    d = (BitMask & ADJACENT_BITMASK_NEG_Y_POS_X_NEG_Z) != 0 ? 0 : 1;
                    e = (BitMask & ADJACENT_BITMASK_POS_X_NEG_Z) != 0 ? 0 : 1;
                    f = (BitMask & ADJACENT_BITMASK_POS_Y_POS_X_NEG_Z) != 0 ? 0 : 1;
                    g = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_Z) != 0 ? 0 : 1;
                    h = (BitMask & ADJACENT_BITMASK_POS_Y_NEG_X_NEG_Z) != 0 ? 0 : 1;

                }
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
        public void Clean()
        {
            OpaqueVertices.Clear();
            OpaqueIndices.Clear();
        }
    }
}
