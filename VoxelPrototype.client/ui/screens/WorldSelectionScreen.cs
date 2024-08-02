using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.elements;
using VoxelPrototype.client.ui.elements.container;
using VoxelPrototype.client.ui.renderer;
using VoxelPrototype.client.ui.utils;
using VoxelPrototype.game;
namespace VoxelPrototype.client.ui.screens
{
    internal class WorldSelectionScreen : UIScreen
    {
        Panel WorldScrollPanel;
        WorldSelectable SelectedWorld;
        public WorldSelectionScreen(UIScreen Parent)
        {
            this.Parent = Parent;
            WorldScrollPanel = new(true);
            var Worlds = LoadWorld();
            for(int i = 0; i< Worlds.Count;i++ ) 
            {
                var WorldSel = new WorldSelectable(Worlds[i].Name);
                WorldScrollPanel.Children.Add(WorldSel);

                if (i == 0)
                {
                    SelectedWorld =WorldSel;
                }
            }
            Children.Add(WorldScrollPanel);

        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.PartFrame);
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }

        public override void ComputeLayout()
        {
            WorldScrollPanel.Position = Position + new Vector2i(0,50);
            WorldScrollPanel.Size = Size - new Vector2i(0,200);

            base.ComputeLayout();
        }
        public static List<WorldInfo> LoadWorld()
        {
            List<WorldInfo> Worlds = new();
            string[] WorldFolders = Directory.GetDirectories("worlds");
            foreach (string world in WorldFolders)
            {
                if (File.Exists(world + "/world.vpw"))
                {
                    try
                    {
                        WorldInfo worldInfo = new WorldInfo().Deserialize(File.ReadAllBytes(world + "/world.vpw"));
                        worldInfo.Path = world;
                        worldInfo.Name = world.Split("\\").Last();
                        Worlds.Add(worldInfo);
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error(ex, "Worldinfo can't be load , possibly due to corrupted data.");
                    }
                }
                else
                {
                }
            }
            return Worlds;
        }
    }
    internal class WorldSelectable : Element
    {
        public string Name;

        public WorldSelectable(string name)
        {
            Name = name;
            Size = new Vector2i(0,150);
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, IsHovered ? 0x1f2121FF  : 0x333535FFu); 
            Renderer.RenderText("Name: "+Name, new Vector2i(Position.X+10, Position.Y + (Name.CalculateVerticalSize() +10)));
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            if(IsHovered && MState.IsButtonPressed(MouseButton.Left))
            {

            }
        }
    }
}
