﻿using OpenTK.Mathematics;
namespace VoxelPrototype.common.Entities
{
    public class Entity
    {
        public Vector3d Position;
        public Vector3 Rotation = new Vector3(0, 0, 0);
        public string ID = Guid.NewGuid().ToString();
        public virtual void UpdateClient(double DT) { }
        public virtual void UpdateServer(double DT) { }
    }
}
