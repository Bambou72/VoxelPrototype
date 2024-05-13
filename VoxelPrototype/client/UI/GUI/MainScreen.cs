using Crecerelle;
using Crecerelle.Constraints;
using Crecerelle.Elements;
using Crecerelle.Utils;
using VoxelPrototype.client.UI.Elements;
using static System.Net.Mime.MediaTypeNames;
namespace VoxelPrototype.client.UI.GUI
{
    public class MainScreen : Crecerelle.GUI
    {
        StackPanel ButtonPanel;
        Label Version;
        public MainScreen()
        {
            Fullscreen = true;
            Show = true;
            CaptInput = true;
            AddChild(new Title("Voxel@ui/title"),
            new ConstraintHolder()
            {
                XConstraint = new CenterConstraint(),
                YConstraint = new RelativeConstraint(0.05f),
                WidthConstraint = new RelativeConstraint(0.4f),
                HeightConstraint = new AspectConstraint(3.49f),
            }
            );
            Version = new Label("V" + EngineVersion.Version.ToString(), 25, "Voxel@opensans", Color.Black);
            AddChild(Version,
            new ConstraintHolder()
            {
                XConstraint = new MarginConstraint(MarginDirection.Left, 10, false),
                YConstraint = new MarginConstraint(MarginDirection.Bottom, 10, false),
            }
            );
            AddChild(new Label("©Florian Pfeiffer", 25, "Voxel@opensans", Color.Black),
            new ConstraintHolder()
            {
                XConstraint = new MarginConstraint(MarginDirection.Right, 10, false),
                YConstraint = new MarginConstraint(MarginDirection.Bottom, 10, false),
            }
            );
            ButtonPanel = new(20);

            ButtonPanel.AddChild(new Button(null,"Singleplayer"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            }
            );
            ButtonPanel.AddChild(new Button(null, "Multiplayer"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            ButtonPanel.AddChild(new Button(UIMaster.OptionCallback,"Options"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            ButtonPanel.AddChild(new Button(Client.TheClient.Close, "Quit"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            AddChild(ButtonPanel, new ConstraintHolder()
            {
                XConstraint = new CenterConstraint(),
                YConstraint = new RelativeConstraint(0.3f),
                WidthConstraint = new RelativeConstraint(0.3f),
                HeightConstraint = new ParentConstraint(),
            });
        }
        public override void Update(UIManager Manager)
        {
            base.Update(Manager);
            Version.Text = "V" + EngineVersion.Version.ToString();
        }
    }
}
