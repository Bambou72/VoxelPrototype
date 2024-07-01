using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.Components;
namespace VoxelPrototype.client.Render.World
{
    internal static class SkyboxRender
    {
        static SkyBoxModel SkyboxModel = new SkyBoxModel(SkyboxMesh.SkyboxVertices);
        internal static void RenderSkyBox(Matrix4 View, Matrix4 Projection)
        {
            GL.Disable(EnableCap.DepthTest);
            var Shader = Client.TheClient.ShaderManager.GetShader(new Resources.ResourceID("shaders/cubemap"));
            Shader.Use();
            Shader.SetMatrix4("projection", Projection);
            Shader.SetMatrix4("view", View);
            GL.BindVertexArray(SkyboxModel.Vao);
            //Client.TheClient.ShaderManager.GetCubeMap("Voxel@base").Use(TextureUnit.Texture0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
        }
    }
    internal static class SkyboxMesh
    {
        internal static float[] SkyboxVertices = {
            // positions          
			-1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };
    }
}
