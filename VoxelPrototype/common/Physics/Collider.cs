using OpenTK.Mathematics;
namespace VoxelPrototype.common.Physics
{
    public struct Collider
    {
        public double x1, y1, z1;
        public double x2, y2, z2;
        public Vector3d min { get { return new Vector3d(x1, y1, z1); } }
        public Vector3d max { get { return new Vector3d(x2, y2, z2); } }
        public Collider(Vector3d pos1 = default, Vector3d pos2 = default)
        {
            x1 = pos1.X;
            y1 = pos1.Y;
            z1 = pos1.Z;
            x2 = pos2.X;
            y2 = pos2.Y;
            z2 = pos2.Z;
        }
        internal bool Intersect(Collider other)
        {
            return Math.Min(x2, other.x2) - Math.Max(x1, other.x1) > 0
            && Math.Min(y2, other.y2) - Math.Max(y1, other.y1) > 0
            && Math.Min(z2, other.z2) - Math.Max(z1, other.z1) > 0;
        }
        public Vector3d[] GetCorners()
        {
            Vector3d[] corners = new Vector3d[8];
            corners[0] = new Vector3d(x1, y1, z1);
            corners[1] = new Vector3d(x2, y1, z1);
            corners[2] = new Vector3d(x1, y2, z1);
            corners[3] = new Vector3d(x2, y2, z1);
            corners[4] = new Vector3d(x1, y1, z2);
            corners[5] = new Vector3d(x2, y1, z2);
            corners[6] = new Vector3d(x1, y2, z2);
            corners[7] = new Vector3d(x2, y2, z2);

            return corners;
        }
        internal static double Time(double x, double y)
        {
            if (y == 0f)
            {
                return x > 0 ? double.NegativeInfinity : double.PositiveInfinity;
            }
            else
            {
                return x / y;
            }
        }
        public (double?, Vector3d) Collide(Collider collider, Vector3d velocity)
        {
            double vx = velocity.X, vy = velocity.Y, vz = velocity.Z;
            double x_entry = Time(vx > 0 ? collider.x1 - x2 : collider.x2 - x1, vx);
            double x_exit = Time(vx > 0 ? collider.x2 - x1 : collider.x1 - x2, vx);
            double y_entry = Time(vy > 0 ? collider.y1 - y2 : collider.y2 - y1, vy);
            double y_exit = Time(vy > 0 ? collider.y2 - y1 : collider.y1 - y2, vy);
            double z_entry = Time(vz > 0 ? collider.z1 - z2 : collider.z2 - z1, vz);
            double z_exit = Time(vz > 0 ? collider.z2 - z1 : collider.z1 - z2, vz);
            if (x_entry < 0 && y_entry < 0 && z_entry < 0)
            {
                return (null, Vector3d.Zero);
            }
            if (x_entry > 1 || y_entry > 1 || z_entry > 1)
            {
                return (null, Vector3d.Zero);
            }
            double entry = Math.Max(Math.Max(x_entry, y_entry), z_entry);
            double exit_ = Math.Min(Math.Min(x_exit, y_exit), z_exit);
            if (entry > exit_)
            {
                return (null, Vector3d.Zero);
            }
            Vector3d normal = Vector3d.Zero;
            if (entry == x_entry)
            {
                normal = new Vector3d(vx > 0 ? -1 : 1, 0, 0);
            }
            else if (entry == y_entry)
            {
                normal = new Vector3d(0, vy > 0 ? -1 : 1, 0);
            }
            else if (entry == z_entry)
            {
                normal = new Vector3d(0, 0, vz > 0 ? -1 : 1);
            }
            return (entry, normal);
        }
        public Collider Move(Vector3d pos)
        {
            double x = pos.X;
            double y = pos.Y;
            double z = pos.Z;
            return new Collider(new Vector3d(x1 + x, y1 + y, z1 + z), new Vector3d(x2 + x, y2 + y, z2 + z));
        }
    }
}
