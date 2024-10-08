﻿using OpenTK.Mathematics;
namespace VoxelPrototype.utils
{
    public static class CubeNeighbours
    {
        public static Vector3i[] Neighbours ={
            // y = -1
            new Vector3i(-1, -1, -1),
            new Vector3i( 0, -1, -1),
            new Vector3i( 1, -1, -1),
            new Vector3i(-1, -1,  0),
            new Vector3i( 0, -1,  0),
            new Vector3i( 1, -1,  0),
            new Vector3i(-1, -1,  1),
            new Vector3i( 0, -1,  1),
            new Vector3i( 1, -1,  1),
            // y = 0
            new Vector3i(-1,  0, -1),
            new Vector3i( 0,  0, -1),
            new Vector3i( 1,  0, -1),
            new Vector3i(-1,  0,  0),
            new Vector3i( 1,  0,  0),
            new Vector3i(-1,  0,  1),
            new Vector3i( 0,  0,  1),
            new Vector3i( 1,  0,  1),
            // y = 1
            new Vector3i(-1,  1, -1),
            new Vector3i( 0,  1, -1),
            new Vector3i( 1,  1, -1),
            new Vector3i(-1,  1,  0),
            new Vector3i( 0,  1,  0),
            new Vector3i( 1,  1,  0),
            new Vector3i(-1,  1,  1),
            new Vector3i( 0,  1,  1),
            new Vector3i( 1,  1,  1),
        };
        public static Vector2i[] XZNeighbours ={

            new Vector2i(-1, -1),
            new Vector2i( 0, -1),
            new Vector2i(-1,  0),
            new Vector2i( 1, -1),
            new Vector2i(-1,  1),
            new Vector2i( 1,  0),
            new Vector2i( 0,  1),
            new Vector2i( 1,  1),
        };
    }
}
