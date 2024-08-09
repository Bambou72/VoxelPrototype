using OpenTK.Mathematics;
namespace VoxelPrototype.physics
{
    public struct Collider
    {
        public Vector3d Min;
        public Vector3d Max;
        public Collider(Vector3d pos1 = default, Vector3d pos2 = default)
        {
            Min = pos1;
            Max = pos2;
        }
        internal bool Intersect(Collider other)
        {
            return Math.Min(Max.X, other.Max.X) - Math.Max(Min.X, other.Min.X) > 0
            && Math.Min(Max.Y, other.Max.Y) - Math.Max(Min.Y, other.Min.Y) > 0
            && Math.Min(Max.Z, other.Max.Z) - Math.Max(Min.Z, other.Min.Z) > 0;
        }
        /*
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
        }*/
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
            double x_entry = Time(vx > 0 ? collider.Min.X - Max.X : collider.Max.X - Min.X, vx);
            double x_exit = Time(vx > 0 ? collider.Max.X - Min.X : collider.Min.X - Max.X, vx);
            double y_entry = Time(vy > 0 ? collider.Min.Y - Max.Y : collider.Max.Y - Min.Y, vy);
            double y_exit = Time(vy > 0 ? collider.Max.Y - Min.Y : collider.Min.Y - Max.Y, vy);
            double z_entry = Time(vz > 0 ? collider.Min.Z - Max.Z : collider.Max.Z - Min.Z, vz);
            double z_exit = Time(vz > 0 ? collider.Max.Z - Min.Z : collider.Min.Z - Max.Z, vz);
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
            return new Collider(Min+pos, Max+pos);
        }
    }
}
