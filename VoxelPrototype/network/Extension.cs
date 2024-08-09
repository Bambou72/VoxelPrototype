using LiteNetLib.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.network
{
    public static class Extensions
    {
        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.X);
            writer.Put(vector.Y);
        }
        public static Vector2 GetVector2(this NetDataReader reader)
        {
            Vector2 v;
            v.X = reader.GetFloat();
            v.Y = reader.GetFloat();
            return v;
        }
        public static void Put(this NetDataWriter writer, Vector2i vector)
        {
            writer.Put(vector.X);
            writer.Put(vector.Y);
        }
        public static Vector2i GetVector2i(this NetDataReader reader)
        {
            Vector2i v;
            v.X = reader.GetInt();
            v.Y = reader.GetInt();
            return v;
        }
        public static void Put(this NetDataWriter writer, Vector3 vector)
        {
            writer.Put(vector.X);
            writer.Put(vector.Y);
            writer.Put(vector.Z);
        }
        public static Vector3 GetVector3(this NetDataReader reader)
        {
            Vector3 v;
            v.X = reader.GetFloat();
            v.Y = reader.GetFloat();
            v.Z = reader.GetFloat();
            return v;
        }
        public static void Put(this NetDataWriter writer, Quaternion quaternion)
        {
            writer.Put(quaternion.X);
            writer.Put(quaternion.Y);
            writer.Put(quaternion.Z);
            writer.Put(quaternion.W);
        }
        public static void Put(this NetDataWriter writer, Vector3d vector)
        {
            writer.Put(vector.X);
            writer.Put(vector.Y);
            writer.Put(vector.Z);
        }
        public static Vector3d GetVector3d(this NetDataReader reader)
        {
            Vector3d v;
            v.X = reader.GetDouble();
            v.Y = reader.GetDouble();
            v.Z = reader.GetDouble();
            return v;
        }
        public static void Put(this NetDataWriter writer, Vector3i vector)
        {
            writer.Put(vector.X);
            writer.Put(vector.Y);
            writer.Put(vector.Z);
        }

        public static Vector3i GetVector3i(this NetDataReader reader)
        {
            Vector3i v;
            v.X = reader.GetInt();
            v.Y = reader.GetInt();
            v.Z = reader.GetInt();
            return v;
        }

        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            Quaternion q = new();
            q.X = reader.GetFloat();
            q.Y = reader.GetFloat();
            q.Z = reader.GetFloat();
            q.W = reader.GetFloat();
            return q;
        }
        public static void Put(this NetDataWriter writer, Vector3i[] vectors)
        {
            writer.Put(vectors.Length);
            foreach (Vector3i v in vectors)
            {
                writer.Put(v.X);
                writer.Put(v.Y);
                writer.Put(v.Z);
            }
        }
        public static Vector3i[] GetVector3iArray(this NetDataReader reader)
        {
            Vector3i[] array = new Vector3i[reader.GetInt()];
            for (int i = 0; i < array.Length; i++)
            {
                Vector3i v;
                v.X = reader.GetInt();
                v.Y = reader.GetInt();
                v.Z = reader.GetInt();
                array[i] = v;
            }
            return array;
        }
        public static void Put(this NetDataWriter writer, Vector2i[] vectors)
        {
            writer.Put(vectors.Length);
            foreach (Vector2i v in vectors)
            {
                writer.Put(v.X);
                writer.Put(v.Y);
            }
        }
        public static Vector2i[] GetVector2iArray(this NetDataReader reader)
        {
            Vector2i[] array = new Vector2i[reader.GetInt()];
            for (int i = 0; i < array.Length; i++)
            {
                Vector2i v;
                v.X = reader.GetInt();
                v.Y = reader.GetInt();
                array[i] = v;
            }
            return array;
        }
    }
}
