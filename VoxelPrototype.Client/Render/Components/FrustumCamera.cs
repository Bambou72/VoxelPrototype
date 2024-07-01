/**
 * Camera class
 * Authors Opentk , Florian Pfeiffer
 **/
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Debug;
namespace VoxelPrototype.client.Render.Components
{
    public class FrustumCamera : Camera
    {
        internal Utils.Math.Frustum  Frustum ;
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