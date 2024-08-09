using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmark;
public class FastNoiseVsIcaria
{
     FastNoiseLite Ft =new FastNoiseLite(1234);

    [GlobalSetup]
    public void Setup()
    {
        Ft.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        Ft.SetFractalOctaves(4);
        Ft.SetFrequency(0.005f);
    }

    [Benchmark]
    public float FastNoiseLib()
    {
        
        return Ft.GetNoise(0,0);
    }

    [Benchmark]
    public float Icaria()
    {
        return IcariaNoise.GradientNoise(0,0);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<FastNoiseVsIcaria>();
    }
}