using Crecerelle;
using Crecerelle.Constraints;
using Crecerelle.Elements;
using K4os.Compression.LZ4.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPrototype.client.UI.Elements;

namespace VoxelPrototype.client.UI.GUI
{
    internal class OptionMenu : Crecerelle.GUI
    {
        StackPanel ButtonPanel1;
        StackPanel ButtonPanel2;
        public OptionMenu()
        {
            Active = false;
            Fullscreen = true;
            ButtonPanel1 = new(20);
            ButtonPanel2 = new(20);

            ButtonPanel1.AddChild(new Button(null, "Singleplayer"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            }
            );
            ButtonPanel1.AddChild(new Button(null, "Multiplayer"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            ButtonPanel2.AddChild(new Button(null, "Options"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            ButtonPanel2.AddChild(new Button(Client.TheClient.Close, "Quit"),
            new ConstraintHolder()
            {
                XConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new PixelConstraint(75),
            });
            AddChild(ButtonPanel1, new ConstraintHolder()
            {
                XConstraint = new RelativeConstraint(0.3f),
                YConstraint = new RelativeConstraint(0.3f),
                WidthConstraint = new RelativeConstraint(0.2f),
                HeightConstraint = new ParentConstraint(),
            });
            AddChild(ButtonPanel2, new ConstraintHolder()
            {
                XConstraint = new RelativeConstraint(0.55f),
                YConstraint = new RelativeConstraint(0.3f),
                WidthConstraint = new RelativeConstraint(0.2f),
                HeightConstraint = new ParentConstraint(),
            });
        }
    }
}
