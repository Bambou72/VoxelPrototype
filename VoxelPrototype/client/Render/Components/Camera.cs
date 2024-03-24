/**
 * Camera class
 * Authors Opentk
 **/
using OpenTK.Mathematics;
namespace VoxelPrototype.client.Render.Components
{
    public class Camera
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;
        internal float _fov = MathHelper.DegreesToRadians(70);
        internal Vector3 Direction { get { return new Vector3(-GetViewMatrix().M13, -GetViewMatrix().M23, -GetViewMatrix().M33); } }
        internal Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }
        internal Vector3 Position { get; set; }
        internal float AspectRatio { get; set; }
        internal Vector3 Front => _front;
        internal Vector3 Up => _up;
        internal Vector3 Right => _right;
        internal float Near= 0.05f;
        internal float Far = 1024f;
        internal float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }
        internal float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        internal float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 110f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        internal Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }
        internal Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, Near, Far);
        }
        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
            _front = Vector3.Normalize(_front);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}