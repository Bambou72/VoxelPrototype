using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.GUI.Prototype.Utils;
using VoxelPrototype.client.Render.UI;

namespace VoxelPrototype.client.Render.GUI.Prototype.Layouts
{
    internal class ScrollPanel : Layout
    {
        int Offset;
        float scrollOffset = 0;
        float contentHeight = 0;
        bool IsDragging;
        float DragStartPosition;
        float TabHeight
        {
            get
            {
                if (contentHeight > Size.Y)
                {
                    return Size.Y / contentHeight * Size.Y;
                }
                return Size.Y;
            }
        }
        float MaxScroll
        {
            get
            {
                return contentHeight - Size.Y;
            }
        }
        public ScrollPanel(int offset)
        {
            Offset = offset;
        }

        public override void Render(UIRenderer Renderer)
        {
            GL.Enable(EnableCap.ScissorTest);
            GL.Scissor((int)MathF.Ceiling(Position.X), (int)MathF.Ceiling(Position.Y), (int)(MathF.Ceiling(Size.X) - 40), (int)Size.Y);
            base.Render(Renderer);
            Renderer.Flush();
            GL.Disable(EnableCap.ScissorTest);
            Renderer.RenderTextureQuad(
                Position + new Vector2(Size.X - 36, scrollOffset / MaxScroll * (Size.Y - TabHeight)),
                new Vector2(32, TabHeight), UIStyle.ScrollGrabberBackColor);
            Renderer.RenderTextureQuad(Position + new Vector2(Size.X - 40, 0), new Vector2(40, Size.Y), UIStyle.ScrollBackColor);
            Renderer.RenderTextureQuad(Position, new Vector2(Size.X - 40, Size.Y), UIStyle.FrameColor);
        }
        public override void Update(Vector2 MousePos)
        {
            base.Update(MousePos);
            if (!MouseCheck.IsHovering(MousePos,

                Position + new Vector2(Size.X - 100, scrollOffset / MaxScroll * (Size.Y - TabHeight)),
                Position + new Vector2(Size.X, scrollOffset / MaxScroll * (Size.Y - TabHeight)) + new Vector2(50, TabHeight)))
            {
                IsDragging = false;
            }
            if (IsDragging)
            {

                scrollOffset = MousePos.Y - DragStartPosition;
                scrollOffset = Math.Max(0, Math.Min(scrollOffset, contentHeight - Size.Y));
            }
            contentHeight = 0;
            for (int i = 0; i < Elements.Count; i++)
            {
                UIElement Element = Elements[i];
                contentHeight += Element.Size.Y + (i < Elements.Count - 1 ? Offset : 0);
            }

        }
        public override void Compose(Vector2i ScreenSize)
        {
            float CurrentYPos = Position.Y - scrollOffset;
            foreach (UIElement Element in Elements)
            {
                Element.Position.X = Position.X;
                Element.Position.Y = CurrentYPos;
                Element.Size.X = Size.X - 40;
                if (!(Element.Position.X >= Position.X) || !(Element.Position.Y >= Position.Y)
                    || !(Element.Position.X <= (Position + Size).X) || !(Element.Position.Y <= (Position + Size).X))
                {
                    Element.Active = false;
                }
                else
                {
                    Element.Active = true;
                }

                CurrentYPos += Element.Size.Y + Offset;
            }
            base.Compose(ScreenSize);
        }
        public override bool OnWheel(MouseWheelEvent Event)
        {
            base.OnWheel(Event);
            if (contentHeight > Size.Y && MouseCheck.IsHovering((Vector2)Client.TheClient.ClientInterface.GetMousePosition(), Position, Position + Size))
            {
                scrollOffset -= Event.Delta.Y * 30;
                scrollOffset = Math.Max(0, Math.Min(scrollOffset, contentHeight - Size.Y));
                return true;
            }
            return false;
        }
        public override bool OnMouseDown(MouseDownEvent Event)
        {
            base.OnMouseDown(Event);

            if (!IsDragging && MouseCheck.IsHovering(Event.MousePosition,

                Position + new Vector2(Size.X - 36, scrollOffset / MaxScroll * (Size.Y - TabHeight)),
                Position + new Vector2(Size.X - 36, scrollOffset / MaxScroll * (Size.Y - TabHeight)) + new Vector2(32, TabHeight)))
            {
                IsDragging = true;
                DragStartPosition = Event.MousePosition.Y - (Position.Y + scrollOffset / MaxScroll * (Size.Y - TabHeight));
                return true;

            }
            return false;
        }
        public override bool OnMouseUp(MouseUpEvent Event)
        {
            base.OnMouseUp(Event);
            IsDragging = false;
            return false;
        }
    }
}
