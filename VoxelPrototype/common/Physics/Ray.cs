using OpenTK.Mathematics;
using VoxelPrototype.client;
using VoxelPrototype.common.Blocks.State;
namespace VoxelPrototype.common.Physics
{
    public class Ray
    {
        Vector3d StartPosition;
        Vector3 Direction;
        float Length;
        public Ray(Vector3d startPosition, Vector3 direction, float length)
        {
            StartPosition = startPosition;
            Direction = direction;
            Length = length;
        }
        public void Update(Vector3d startPosition, Vector3 direction, float length)
        {
            StartPosition = startPosition;
            Direction = direction;
            Length = length;
        }
        public void TestWithTerrain(Action<Vector3i, Vector3i, BlockState> Callback)
        {
            int x = (int)StartPosition.X;
            int y = (int)StartPosition.Y;
            int z = (int)StartPosition.Z;
            double vx = Direction.X;
            double vy = Direction.Y;
            double vz = Direction.Z;
            int step_x = vx > 0 ? 1 : -1;
            int step_y = vy > 0 ? 1 : -1;
            int step_z = vz > 0 ? 1 : -1;
            int cx = (int)Math.Floor(StartPosition.X + Direction.X * Length);
            int cy = (int)Math.Floor(StartPosition.Y + Direction.Y * Length);
            int cz = (int)Math.Floor(StartPosition.Z + Direction.Z * Length);
            List<Tuple<double?, Vector3i, Vector3i, BlockState>> PossibleCollision = new List<Tuple<double?, Vector3i, Vector3i, BlockState>>();
            for (int i = x - step_x; vx > 0 ? i < cx + step_x : i > cx + step_x; i += step_x)
            {
                for (int j = y - step_y; vy > 0 ? j < cy + step_y : j > cy + step_y; j += step_y)
                {
                    for (int k = z - step_z; vz > 0 ? k < cz + step_z : k > cz + step_z; k += step_z)
                    {
                        var State = Client.TheClient.World.ChunkManager.GetBlock(new Vector3i(i, j, k));
                        if (State != Client.TheClient.ModManager.BlockRegister.Air)
                        {
                            foreach (Collider collider in State.Block.GetColliders())
                            {
                                (double? entry_time, Vector3i normal) = RayVSAABB(collider.Move(new Vector3i(i, j, k)));
                                if (entry_time == null)
                                {
                                    continue;
                                }
                                PossibleCollision.Add(new Tuple<double?, Vector3i, Vector3i, BlockState>(entry_time, normal, new Vector3i(i, j, k), State));
                            }
                        }                    
                    }
                }
            }
            if (PossibleCollision.Count != 0)
            {
                (double? entry_time, Vector3i normal, Vector3i position, BlockState id) = PossibleCollision.OrderBy(x => x.Item1).First();
                entry_time -= 0.001f;
                Callback(position, normal, id);
            }
        }
        private (double?, Vector3i) RayVSAABB(Collider col)
        {
            double tmin = double.NegativeInfinity;
            double tmax = double.PositiveInfinity;
            double tminx = 0;
            double tminy = 0;
            double tminz = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(Direction.X) < 1e-8)
                {
                    if (StartPosition.X < col.x1 || StartPosition.X > col.x2)
                    {
                        return (null, Vector3i.Zero);
                    }
                }
                else
                {
                    double t1 = (col.x1 - StartPosition.X) / Direction.X;
                    double t2 = (col.x2 - StartPosition.X) / Direction.X;
                    tmin = Math.Max(tmin, Math.Min(t1, t2));
                    tminx = Math.Min(t1, t2);
                    tmax = Math.Min(tmax, Math.Max(t1, t2));
                }
                // Répéter pour les deux autres axes (Y et Z)
                if (Math.Abs(Direction.Y) < 1e-8)
                {
                    if (StartPosition.Y < col.y1 || StartPosition.Y > col.y2)
                    {
                        return (null, Vector3i.Zero);
                    }
                }
                else
                {
                    double t1 = (col.y1 - StartPosition.Y) / Direction.Y;
                    double t2 = (col.y2 - StartPosition.Y) / Direction.Y;
                    tmin = Math.Max(tmin, Math.Min(t1, t2));
                    tminy = Math.Min(t1, t2);
                    tmax = Math.Min(tmax, Math.Max(t1, t2));
                }
                if (Math.Abs(Direction.Z) < 1e-8)
                {
                    if (StartPosition.Z < col.z1 || StartPosition.Z > col.z2)
                    {
                        return (null, Vector3i.Zero);
                    }
                }
                else
                {
                    double t1 = (col.z1 - StartPosition.Z) / Direction.Z;
                    double t2 = (col.z2 - StartPosition.Z) / Direction.Z;
                    tmin = Math.Max(tmin, Math.Min(t1, t2));
                    tminz = Math.Min(t1, t2);
                    tmax = Math.Min(tmax, Math.Max(t1, t2));
                }
                if (tmax < Math.Max(tmin, 0.0))
                {
                    return (null, Vector3i.Zero);
                }
            }
            Vector3i normal = Vector3i.Zero;
            if (tmin == tminx)
            {
                normal = new Vector3i(Direction.X > 0 ? -1 : 1, 0, 0);
            }
            else if (tmin == tminy)
            {
                normal = new Vector3i(0, Direction.Y > 0 ? -1 : 1, 0);
            }
            else if (tmin == tminz)
            {
                normal = new Vector3i(0, 0, Direction.Z > 0 ? -1 : 1);
            }
            return (tmin, normal);
        }
    }
}
