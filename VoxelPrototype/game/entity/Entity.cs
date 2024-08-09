using OpenTK.Mathematics;
namespace VoxelPrototype.game.entity
{
    public class Entity
    {
        public Vector3d Position;
        public Vector3 Rotation = new Vector3(0, 0, 0);
        public static uint ENTITY_COUNTER;
        public static uint GET_ENTITY_ID() { Interlocked.Increment(ref ENTITY_COUNTER); return ENTITY_COUNTER; }
        //public string ID = Guid.NewGuid().ToString();
        public virtual void Update(IWorld World, double DT) { }
    }
}
