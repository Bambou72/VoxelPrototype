using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using System.Numerics;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.client.Render.Debug;
using VoxelPrototype.common.Physics;

namespace VoxelPrototype.common.Utils.Math
{
    /*
    public static class FrustumCulling
    {
        public static bool AABBIntersect(Frustum frustum, Collider collider)
        {
            float m, n; 
            int i;
            bool result = true;

            for (i = 0; i < 6; i++)
            {
                Plane p = frustum[i];

                m = (b->mx * p->a) + (b->my * p->b) + (b->mz * p->c) + p->d;
                n = (b->dx * fabs(p->a)) + (b->dy * fabs(p->b)) + (b->dz * fabs(p->c));

                if (m + n < 0) return false;
                if (m - n < 0) result = true;

            }
            return result;
        }
        public static Frustum CreateFrustumFromCamera(Camera Camera, float aspect, float fovY,float zNear, float zFar)
        {
            Frustum frustum = new();
            float halfVSide = zFar * MathF.Tan(fovY * 0.5f);
            float halfHSide = halfVSide * aspect;
            Vector3 frontMultFar = zFar * Camera.Front;
            frustum.NearFace = new( Camera.Position + zNear* Camera.Front, Camera.Front );
            frustum.FarFace  = new(Camera.Position + frontMultFar, -Camera.Front);
            frustum.RightFace = new(Camera.Position,Vector3.Cross(frontMultFar - Camera.Right * halfHSide, Camera.Up));
            frustum.LeftFace = new (Camera.Position,Vector3.Cross(Camera.Up, frontMultFar + Camera.Right * halfHSide));
            frustum.TopFace = new (Camera.Position,Vector3.Cross(Camera.Right, frontMultFar - Camera.Up * halfVSide));
            frustum.BottomFace = new(Camera.Position,Vector3.Cross(frontMultFar + Camera.Up * halfVSide, Camera.Right));
            return frustum;
        }
        // Fonction pour récupérer les plans de coupe du frustum
        Plane[] CalculateFrustumPlanes(Matrix4 projection, Matrix4 view)
        {
            Plane[] frustumPlanes = new Plane[6];
            Matrix4 viewProjection = view * projection;
            viewProjection.Invert();

            // Extraire les plans de coupe à partir de la matrice de vue-projection inversée
            frustumPlanes[0] = new Plane(viewProjection.Row3 + viewProjection.Row0);
            frustumPlanes[1] = new Plane(viewProjection.Row3 - viewProjection.Row0);
            frustumPlanes[2] = new Plane(viewProjection.Row3 + viewProjection.Row1);
            frustumPlanes[3] = new Plane(viewProjection.Row3 - viewProjection.Row1);
            frustumPlanes[4] = new Plane(viewProjection.Row3 + viewProjection.Row2);
            frustumPlanes[5] = new Plane(viewProjection.Row3 - viewProjection.Row2);

            return frustumPlanes;
        }
    }*/
}
