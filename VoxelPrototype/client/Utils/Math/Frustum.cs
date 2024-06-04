using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.World.Level.Chunk.Render;
using VoxelPrototype.common.World;

namespace VoxelPrototype.client.Utils.Math
{
    internal class Frustum
    {
        internal FrustumCamera Camera;
        internal float FactorY;
        internal float TanY;
        internal float FactorX;
        internal float TanX;
        public Frustum(FrustumCamera camera)
        {
            Camera = camera;
             Recalculate();
            
        }
        public void Recalculate()
        {
            FactorY = 1.0f / MathF.Cos(Camera._fov * 0.5f);
            TanY = MathF.Tan(Camera._fov * 0.5f);
            float HFOV = 2 * MathF.Atan(MathF.Tan(Camera._fov * 0.5f) * Camera.AspectRatio);
            FactorX = 1.0f / MathF.Cos(HFOV * 0.5f);
            TanX = MathF.Tan(HFOV * 0.5f);


        }
        public bool IsSectionInFrustum(SectionMesh Section)
        {
            //Vector from cam to sphere
            Vector3 sphere_vec = Section.Center - Camera.Position;
            //Near and Far
            float SZ = Vector3.Dot(sphere_vec, Camera.Front);
            if(!(Camera.Near - World.Level.Chunk.Section.SphereRadius <= SZ && SZ <= Camera.Far + World.Level.Chunk.Section.SphereRadius))
            {
                return false;
            }
            //Top and Bottom
            float SY  = Vector3.Dot(sphere_vec, Camera.Up);
            float Dist = FactorY * World.Level.Chunk.Section.SphereRadius +SZ * TanY;
            if (!(-Dist<= SY && SY <= Dist))
            {
                return false;
            }
            //Right and Left
            float SX = Vector3.Dot(sphere_vec, Camera.Right);
            Dist = FactorX * World.Level.Chunk.Section.SphereRadius + SZ * TanX;
            if (!(-Dist <= SX && SX <= Dist))
            {
                return false;
            }
            return true;
        }
    }
}
