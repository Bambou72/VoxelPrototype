using OpenTK.Mathematics;
using VoxelPrototype.client.game.world.Level.Chunk;
using VoxelPrototype.client.game.world.Level.Chunk.Render;
using VoxelPrototype.client.rendering.camera;

namespace VoxelPrototype.client.utils.math
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
        public bool IsSectionInFrustum(Section Section)
        {
            //Vector from cam to sphere
            Vector3 sphere_vec = Section.Center - Camera.Position;
            //Near and Far
            float SZ = Vector3.Dot(sphere_vec, Camera.Front);
            if(!(Camera.Near - game.world.Level.Chunk.Section.SphereRadius <= SZ && SZ <= Camera.Far + game.world.Level.Chunk.Section.SphereRadius))
            {
                return false;
            }
            //Top and Bottom
            float SY  = Vector3.Dot(sphere_vec, Camera.Up);
            float Dist = FactorY * game.world.Level.Chunk.Section.SphereRadius +SZ * TanY;
            if (!(-Dist<= SY && SY <= Dist))
            {
                return false;
            }
            //Right and Left
            float SX = Vector3.Dot(sphere_vec, Camera.Right);
            Dist = FactorX * game.world.Level.Chunk.Section.SphereRadius + SZ * TanX;
            if (!(-Dist <= SX && SX <= Dist))
            {
                return false;
            }
            return true;
        }
        public bool IsSectionInFrustumSimple(Section Section)
        {
            //Vector from cam to sphere
            Vector3 sphere_vec = Section.Center - Camera.Position;
            //Near and Far
            float SZ = Vector3.Dot(sphere_vec, Camera.Front);
            //Top and Bottom
            float SY = Vector3.Dot(sphere_vec, Camera.Up);
            float Dist = FactorY * game.world.Level.Chunk.Section.SphereRadius + SZ * TanY;
            if (!(-Dist <= SY && SY <= Dist))
            {
                return false;
            }
            return true;
        }
        public bool IsChunkInFrustum(Chunk chunk)
        {
            //Vector from cam to sphere
            Vector2 sphere_vec = chunk.Center - Camera.Position.Xz;
            //Near and Far
            float SZ = Vector2.Dot(sphere_vec, Camera.Front.Xz.Normalized());
            if (!(Camera.Near - Chunk.CircleRadius <= SZ && SZ <= Camera.Far + Chunk.CircleRadius))
            {
                return false;
            }
            //Right and Left
            float SX = Vector2.Dot(sphere_vec, Camera.Right.Xz.Normalized());
            float Dist = FactorX * Chunk.CircleRadius + SZ * TanX;
            if (!(-Dist <= SX && SX <= Dist))
            {
                return false;
            }
            return true;
        }
    }
}
