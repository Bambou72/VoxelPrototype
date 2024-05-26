using K4os.Compression.LZ4;
using VoxelPrototype.common.Utils;
namespace VoxelPrototype.common.World
{
    public enum CompressionType : byte
    {
        Uncompressed = 0,
        Deflate = 1,
        LZ4 = 2,
    }
    // TODO : Add a defragmentation algorithm
    public class RegionFile
    {
        public const int Size = 32;
        private const int SectorSize = 4096;
        FileStream FileStream { get; set; }
        public List<bool> SectorsFree = new List<bool>();
        public RegionFile(string Path)
        {
            if (File.Exists(Path))
            {
                FileStream = new(Path, FileMode.Open, FileAccess.ReadWrite);
            }
            else
            {
                FileStream = new(Path, FileMode.Create, FileAccess.ReadWrite);
                CreateRegion();
            }
            TestAllSectors();

        }
        private void CreateRegion()
        {
            for (int i = 0; i < Size * Size; i++)
            {
                FileStream.Write(new byte[4], 0, 4);
            }
        }
        public void Close()
        {
            FileStream?.Close();
        }
        //
        //Read Chunk
        //
        public VBFCompound[] ReadAllChunkAsVBF()
        {
            var Offsets = GetAllUsedOffset();
            List<VBFCompound> Chunks = new();
            foreach (var offset in Offsets)
            {
                (byte[] data, CompressionType CompType) = ReadChunkData(offset.Item1, offset.Item2);
                byte[] FinalData = data;
                if (CompType == CompressionType.LZ4)
                {
                    FinalData = LZ4Pickler.Unpickle(data);
                }
                else if (CompType == CompressionType.Deflate)
                {
                    FinalData = Deflate.Decompress(data);
                }
                Chunks.Add((VBFCompound)VBFSerializer.Deserialize(FinalData));
            }
            return Chunks.ToArray();
        }
        public (byte[], CompressionType) ReadChunk(int X, int Z)
        {
            (int Localization, byte Size) = GetOffset(X, Z);
            if (Localization != 0 && Size != 0)
            {
                return ReadChunkData(Localization, Size);
            }
            return (new byte[0], CompressionType.Uncompressed);
        }
        private (byte[], CompressionType) ReadChunkData(int Localisation, int Size)
        {
            FileStream.Seek(Localisation * SectorSize, SeekOrigin.Begin);
            byte[] Data = new byte[Size * SectorSize];
            int Length = FileStream.Read(Data, 0, Size * SectorSize);
            int DataLength = Data[0] << 24 | Data[1] << 16 | Data[2] << 8 | Data[3];
            CompressionType CompressionType = (CompressionType)Data[4];
            byte[] ChunkData = new byte[DataLength];
            Array.Copy(Data, 5, ChunkData, 0, DataLength);
            return (ChunkData, CompressionType);
        }
        //
        //Write Chunk
        //
        public void WriteChunk(int X, int Z, byte[] ChunkData, CompressionType CompressionType)
        {
            int totalsize = ChunkData.Length + 5;
            int numSections = (int)Math.Ceiling((double)totalsize / SectorSize);
            (int Localization, byte Size) = GetOffset(X, Z);
            if (Localization != 0 && Size != 0 && numSections <= Size)
            {
                WriteChunkData(Localization, ChunkData, CompressionType);
            }
            else
            {
                int Index = FindFirstFreeSector(numSections);
                if (Index != -1)
                {
                    WriteOffset(X, Z, Index, (byte)numSections);
                    WriteChunkData(Index, ChunkData, CompressionType);
                }
                else
                {
                    int NewLoc = (int)AllocateSectors(numSections);
                    WriteOffset(X, Z, NewLoc, (byte)numSections);
                    WriteChunkData(NewLoc, ChunkData, CompressionType);
                    if (Localization != 0 && Size != 0)
                    {
                        ClearSections(Localization, Size);
                    }
                }
            }
            TestAllSectors();
        }
        private void WriteChunkData(int Localization, byte[] ChunkData, CompressionType compressionType)
        {
            FileStream.Seek(Localization * SectorSize, SeekOrigin.Begin);
            byte[] ChunkDataLength = BitConverter.GetBytes(ChunkData.Length);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(ChunkDataLength);
            }
            FileStream.Write(new byte[4] { ChunkDataLength[3], ChunkDataLength[2], ChunkDataLength[1], ChunkDataLength[0] }, 0, 4);
            FileStream.Write(new byte[1] { (byte)compressionType }, 0, 1);
            FileStream.Write(ChunkData, 0, ChunkData.Length);
        }
        //
        //Offset table
        //
        public (int, byte)[] GetAllUsedOffset()
        {
            List<(int, byte)> Offsets = new();
            for (int i = 0; i < 1024; i++)
            {
                byte[] Data = new byte[4];
                FileStream.Seek(i * 4, SeekOrigin.Begin);
                FileStream.Read(Data, 0, 4);
                int Localization = Data[0] << 16 | Data[1] << 8 | Data[2];
                byte NumberOfSectors = Data[3];
                if (Localization != 0 && NumberOfSectors != 0)
                {
                    Offsets.Add((Localization, NumberOfSectors));
                }
            }
            return Offsets.ToArray();
        }
        public (int, byte) GetOffset(int X, int Z)
        {
            int index = Z * Size + X;
            byte[] Data = new byte[4];
            FileStream.Seek(index * 4, SeekOrigin.Begin);
            FileStream.Read(Data, 0, 4);
            int Localization = Data[0] << 16 | Data[1] << 8 | Data[2];
            byte NumberOfSectors = Data[3];
            return (Localization, NumberOfSectors);
        }
        public void WriteOffset(int X, int Z, int offset, byte NumberOfSectors)
        {
            int index = Z * Size + X;
            byte[] offsetBytes = BitConverter.GetBytes(offset);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(offsetBytes);
            }
            FileStream.Seek(index * 4, SeekOrigin.Begin);
            FileStream.Write(new byte[4] { offsetBytes[2], offsetBytes[1], offsetBytes[0], NumberOfSectors }, 0, 4);
        }
        //
        //Utils
        //
        private void ClearSections(int Index, int Size)
        {
            for (int i = Index; i < Size; i++)
            {
                ClearSection(i);
            }
        }
        private void ClearSection(int Index)
        {
            FileStream.Seek(Index * SectorSize, SeekOrigin.Begin);
            FileStream.Write(new byte[SectorSize], 0, SectorSize);
        }
        private int FindFirstFreeSector(int numberOfConsecutiveSectors)
        {
            for (int i = 0; i <= SectorsFree.Count - numberOfConsecutiveSectors; i++)
            {
                bool found = true;
                for (int j = 0; j < numberOfConsecutiveSectors; j++)
                {
                    if (SectorsFree[i + j] == false)
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        private void TestAllSectors()
        {
            SectorsFree = new();
            int numSectors = (int)Math.Ceiling((double)FileStream.Length / SectorSize);
            for (int i = 0; i < numSectors; i++)
            {
                if (i == 0)
                {
                    SectorsFree.Add(false);
                }
                else
                {
                    byte[] sectorData = new byte[SectorSize];
                    FileStream.Seek(i * SectorSize, SeekOrigin.Begin);
                    int bytesRead = FileStream.Read(sectorData, 0, SectorSize);
                    if (bytesRead < SectorSize)
                    {
                        SectorsFree.Add(true);
                    }
                    else
                    {
                        SectorsFree.Add(IsEmptySector(sectorData));
                    }
                }
            }
        }
        public long AllocateSectors(int Count)
        {
            FileStream.Seek(0, SeekOrigin.End);
            long BaseAllocationLocation = FileStream.Position;
            for (int i = 0; i < Count; i++)
            {
                FileStream.Write(new byte[SectorSize], 0, SectorSize);
            }
            TestAllSectors();
            return (int)Math.Ceiling((double)BaseAllocationLocation / SectorSize);
        }
        private bool IsEmptySector(byte[] data)
        {
            return data.All(b => b == 0);
        }
    }
}
