using OpenTK.Mathematics;
namespace ModelEditor
{
    public class ArcBallCamera
    {
        private Quaternion rotation = Quaternion.Identity;
        private Vector3 target = new Vector3(0f, 0f, 0f); // Au centre de la scène
        private float distance = 5.0f;
        public Vector3 Position
        {
            get { return target + Vector3.Transform(Vector3.UnitZ * distance, rotation); }
        }
        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        public void Rotate(Vector2 delta)
        {
            float horizontalAngle = -delta.X;
            float verticalAngle = delta.Y;
            Quaternion horizontalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, horizontalAngle);
            Quaternion verticalRotation = Quaternion.FromAxisAngle(Vector3.UnitX, verticalAngle);
            // Appliquer les rotations dans l'ordre souhaité
            rotation = verticalRotation * rotation * horizontalRotation;
        }
        public void Translate(Vector2 delta)
        {
            float horizontalAngle = -delta.X;
            float verticalAngle = -delta.Y;
            target += new Vector3(horizontalAngle, 0, verticalAngle);
        }
        public void Zoom(float delta)
        {
            distance -= delta;
            if (distance < 1.0f)
                distance = 1.0f;
        }
        public Matrix4 GetViewMatrix()
        {
            Matrix4 lookAt = Matrix4.LookAt(Position, target, Rotation * Vector3.UnitY);
            return lookAt;
        }
    }
}
