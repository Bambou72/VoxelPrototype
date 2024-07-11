using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.game.entity;
using VoxelPrototype.client.Render.Components;
using VoxelPrototype.game.entity.player;
using VoxelPrototype.utils;

namespace VoxelPrototype.client.Render.Entities
{
    internal class PlayersRenderer
    {
        internal static DebugBox BlockBox = new DebugBox(new Vector3d(1.03125, 1.03125, 1.03125), new Vector4(0.33f, 0.33f, 0.33f, 1));
        public void RenderPlayers()
        {
            if (Client.TheClient.World.IsLocalPlayerExist())
            {
                RenderSelectedBlock(Client.TheClient.World.PlayerFactory.LocalPlayer);
            }
            foreach (var player in Client.TheClient.World.PlayerFactory.PlayerList.Values)
            {
                if (player != Client.TheClient.World.PlayerFactory.LocalPlayer)
                {
                    RenderPlayer(player);
                }
            }
        }
        internal void RenderSelectedBlock(ClientPlayer play)
        {
            if (play.ViewBlock)
            {
                //RenderSystem.RenderDebugBox(BlockBox, play.ViewedBlockPos - new Vector3(0.015625f));
            }
        }
        internal void RenderPlayer(ClientPlayer play)
        {
            GL.BindVertexArray(play._Model.Vao);
            var Shader = Client.TheClient.ShaderManager.GetShader(new ResourceID("shaders/entity"));
            Client.TheClient.TextureManager.GetTexture(new ResourceID("textures/entity/player")).Use(TextureUnit.Texture0);
            Shader.Use();
            var model = Matrix4.Identity * Matrix4.CreateTranslation(new Vector3((float)play.Position.X, (float)(play.Position.Y + play.EntityEYEHeight), (float)play.Position.Z)) /** Matrix4.CreateRotationY(Rotation.Y)*/;
            Shader.SetMatrix4("model", model);
            Shader.SetMatrix4("view", Client.TheClient.World.GetLocalPlayerCamera().GetViewMatrix());
            Shader.SetMatrix4("projection", Client.TheClient.World.GetLocalPlayerCamera().GetProjectionMatrix());
            GL.DrawArrays(PrimitiveType.Triangles, 0, play._Model._Vertices.Count());
            GL.BindVertexArray(0);
        }
    }
}
