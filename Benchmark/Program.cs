using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
public class VoxelSection1D
{
    private short[] voxels;
    private int width;
    private int height;
    private int depth;

    public VoxelSection1D(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        voxels = new short[width * height * depth];
    }

    private int GetIndex(int x, int y, int z)
    {
        return x + y * width + z * width * height;
    }

    public short GetVoxel(int x, int y, int z)
    {
        return voxels[GetIndex(x, y, z)];
    }

    public void SetVoxel(int x, int y, int z, short value)
    {
        voxels[GetIndex(x, y, z)] = value;
    }
}

public class OctreeNode
{
    public short? Value;
    public OctreeNode[] Children;

    public OctreeNode(short? value)
    {
        Value = value;
        Children = null;
    }

    public void Subdivide()
    {
        if (Children == null)
        {
            Children = new OctreeNode[8];
            for (int i = 0; i < 8; i++)
            {
                Children[i] = new OctreeNode(Value);
            }
            Value = null;
        }
    }

    public bool IsLeaf()
    {
        return Children == null;
    }
}

public class Octree
{
    private OctreeNode root;
    private int size;

    public Octree(short initialValue, int size = 16)
    {
        root = new OctreeNode(initialValue);
        this.size = size;
    }

    private int GetChildIndex(int x, int y, int z, int level)
    {
        int halfSize = 1 << (level - 1);
        return (x >= halfSize ? 1 : 0) | (y >= halfSize ? 2 : 0) | (z >= halfSize ? 4 : 0);
    }

    public void SetVoxel(int x, int y, int z, short value)
    {
        SetVoxel(root, x, y, z, value, size, 0);
    }

    private void SetVoxel(OctreeNode node, int x, int y, int z, short value, int currentSize, int level)
    {
        if (node.IsLeaf())
        {
            if (node.Value == value)
                return;

            if (currentSize == 1)
            {
                node.Value = value;
                return;
            }

            node.Subdivide();
        }

        int halfSize = currentSize / 2;
        int childIndex = GetChildIndex(x, y, z, (int)Math.Log2(currentSize));
        int offsetX = (childIndex & 1) * halfSize;
        int offsetY = ((childIndex & 2) >> 1) * halfSize;
        int offsetZ = ((childIndex & 4) >> 2) * halfSize;

        SetVoxel(node.Children[childIndex], x - offsetX, y - offsetY, z - offsetZ, value, halfSize, level + 1);
    }

    public short GetVoxel(int x, int y, int z)
    {
        return GetVoxel(root, x, y, z, size);
    }

    private short GetVoxel(OctreeNode node, int x, int y, int z, int currentSize)
    {
        if (node.IsLeaf())
        {
            return node.Value ?? 0;
        }

        int halfSize = currentSize / 2;
        int childIndex = GetChildIndex(x, y, z, (int)Math.Log2(currentSize));
        int offsetX = (childIndex & 1) * halfSize;
        int offsetY = ((childIndex & 2) >> 1) * halfSize;
        int offsetZ = ((childIndex & 4) >> 2) * halfSize;

        return GetVoxel(node.Children[childIndex], x - offsetX, y - offsetY, z - offsetZ, halfSize);
    }
}


public class VoxelBenchmark
{
    private VoxelSection1D voxelSection1D;
    private Octree octree;

    [GlobalSetup]
    public void Setup()
    {
        voxelSection1D = new VoxelSection1D(16, 16, 16);
        octree = new Octree(0, 16);

        // Initialiser les voxels
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    voxelSection1D.SetVoxel(x, y, z, (short)(x + y + z));
                    octree.SetVoxel(x, y, z, (short)(x + y + z));
                }
            }
        }
    }

    [Benchmark]
    public void AccessVoxel1D()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    short value = voxelSection1D.GetVoxel(x, y, z);
                }
            }
        }
    }

    [Benchmark]
    public void AccessVoxelOctree()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    short value = octree.GetVoxel(x, y, z);
                }
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<VoxelBenchmark>();
    }
}