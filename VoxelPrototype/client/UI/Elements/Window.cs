using Crecerelle;
using Crecerelle.Constraints;
using Crecerelle.Elements;
using Crecerelle.Renderer;
using Crecerelle.Utils;
using OpenTK.Mathematics;
namespace VoxelPrototype.client.UI.Elements
{
    /*
    public class Window : UIElement
    {
        public bool Open = true;
        public string Title = "";
        bool IsDragging;
        Vector2i DragStartPosition;
        Quad TitleBarQuad;
        Quad Frame;
        Quad Minimize;
        Text TitleText;
        public Window()
        {

            TestInput = true;
            TitleBarQuad = new Quad()
            {
                Color = new Color(0.20f, 0.21f, 0.21f, 1),
                Parent = this,
                XConstraint = new ParentConstraint(),
                YConstraint = new ParentConstraint(),
                WidthConstraint = new ParentConstraint(),
                Size = new(0, 25)

            };
            Children.Add(TitleBarQuad);
            TitleText = new Text()
            {
                Value = Title,
                FontSize = 22,
                Parent = this,
                XConstraint = new PixelConstraint(3),
                YConstraint = new PixelConstraint(22),
            };
            TitleBarQuad.Children.Add(TitleText);
            Minimize = new Quad()
            {
                Color = new Color(0.30f, 0.31f, 0.31f, 1),
                Parent = this,
                XConstraint = new MarginConstraint(MarginDirection.Right, 3, false),
                YConstraint = new PixelConstraint(3),
                Size = new(19, 19)
            };
            TitleBarQuad.Children.Add(Minimize);
            Frame = new Quad()
            {
                Color = new Color(0.25f, 0.26f, 0.26f, 1),
                Parent = this,
                XConstraint = new ParentConstraint(),
                YConstraint = new PixelConstraint(25),
                WidthConstraint = new ParentConstraint(),
                HeightConstraint = new ParentConstraint(),
            };
            Children.Add(Frame);
        }
        public override void Update(UIManager Manager)
        {
            TitleText.Value = Title;
            base.Update(Manager);
        }
        public override bool LogicUpdate(UIManager Manager)
        {
            bool Captured = false;
            if (Open && MouseCheck.IsHovering(Position, Position + Size) && InputSystem.MousePressed(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
            {
                Captured = true;

            }

            if (MouseCheck.IsHovering(new Vector2i((int)(Position.X + Size.X -22), (int)(Position.Y + 2.5f)), new Vector2i((int)(Position.X + Size.X - 22), (int)(Position.Y + 2.5f)) + new Vector2i(19, 19)))
            {
                if (InputSystem.MousePressed(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
                {
                    Open = !Open;
                    Captured = true;
                }
            }
            else
            {
                if (InputSystem.MouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
                {
                    if (!Manager.IsDraging && !IsDragging && MouseCheck.IsHovering(new Vector2i(Position.X, Position.Y), new Vector2i(Position.X + Size.X, Position.Y + 25)))
                    {
                        Client.TheClient.UIManager.IsDraging = true;
                        IsDragging = true;
                        DragStartPosition = (Vector2i)(InputSystem.Mouse.Position) - Position;
                        Captured = true;

                    }
                }
                else
                {
                    Manager.IsDraging = false;
                    IsDragging = false;
                }
            }
            if (IsDragging)
            {
                Captured = true;
                Position = (Vector2i)InputSystem.Mouse.Position - DragStartPosition;
            }
            Frame.Active = Open;
            if (Captured)
            {
                //  Manager.SetFrontUI(this);
            }
            return Captured;
        }
    }*/
}
