/**
 * Camera class
 * Authors Opentk , Florian Pfeiffer
 **/
using OpenTK.Mathematics;
using VoxelPrototype.client.utils.math;
namespace VoxelPrototype.client.rendering.camera
{
    public class FrustumCamera : Camera
    {
        internal Frustum Frustum;
        internal FrustumCamera(Vector3 position, float aspectRatio) : base(position, aspectRatio)
        {
            Frustum = new(this);
        }
        internal void Update()
        {
            Frustum.Recalculate();
        }
    }
}