using OpenTK.Mathematics;

namespace VoxelPrototype.common.Utils.Math
{
    public struct Plane
    {
        public Vector3 Normal { get; private set; }
        public float D { get; private set; } // Distance du plan à l'origine

        public Plane(Vector3 normal, float d)
        {
            Normal = normal.Normalized(); // Normaliser le vecteur normal
            D = d;
        }

        // Fonction pour tester si un point est devant ou derrière le plan
        public float DistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }
    }
}
