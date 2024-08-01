namespace VoxelPrototype.client.rendering.model
{
    public class BlockMesh
    {
        private float[][] Mesh;
        private float[][] Uv;
        internal BlockMesh(float[][] Model, float[][] uv)
        {
            Mesh = Model;
            Uv = uv;
        }
        public float[][] GetMesh()
        {
            return Mesh;
        }
        public float[][] GetUV()
        {
            return Uv;
        }
    }
}
