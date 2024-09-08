using OpenTK.Mathematics;
using System.Drawing;
using VoxelPrototype.api.block;
using VoxelPrototype.api.worldgeneration;
using VoxelPrototype.server.game.world.Level.Chunk;
using VoxelPrototype.voxelprototype.block;

namespace VoxelPrototype.voxelprototype.generator
{
    
    internal class MandelbrotGenerator : IChunkGenerator
    {
        double xmin = -2.5;
        double xmax = 1.0;
        double ymin = -1.0;
        double ymax = 1.0;
        int maxIterations = 1000;
        int Width = 768;
        int Height = (int)(768 / 1.33f);
        public Chunk GenerateChunk(Vector2i Position)
        {
            Chunk chunk = new Chunk(Position);
            for (int cx = 0; cx < Const.ChunkSize; cx++)
            {
                for (int cz = 0; cz < Const.ChunkSize; cz++)
                {
                    int GlobalX = cx + Position.X * 16;
                    int GlobalZ = cz + Position.Y * 16;
                    double x0 = xmin + GlobalX * (xmax - xmin) / (Width - 1);
                    double z0 = ymin + GlobalZ * (ymax - ymin) / (Height - 1);
                    double x = 0.0;
                    double z = 0.0;
                    int iteration = 0;
                    // Mandelbrot iteration
                    while (x * x + z * z <= 4 && iteration < maxIterations)
                    {
                        double xtemp = x * x - z * z + x0;
                        z = 2 * x * z + z0;
                        x = xtemp;
                        iteration++;
                    }
                    int colorIndex = iteration == maxIterations ? 0 : iteration % 16; // Use only 16 colors
                    chunk.SetBlock(new Vector3i(cx, 1,cz), BlockRegistry.GetInstance().GetBlock("vp:mandelbrot").GetDefaultState().With(Mandelbrot.Color,colorIndex));              
                }
            }
            return chunk;
        }
    }
}
