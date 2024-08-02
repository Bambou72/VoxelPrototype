using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
using VoxelPrototype.client.ui.utils;
namespace VoxelPrototype.client.ui.elements.container
{
    internal class Panel : Container
    {
        bool Scrollable;
        int ScrollOffset = 0;
        bool Drag;

        int ThumbHeight
        {
            get
            {
                int Value = Math.Max(1, Math.Min(Size.Y, (int)((Size.Y / (float)ContentHeight) *Size.Y)));
                return Value;
            }
        }
        int MaxScroll => ContentHeight - Size.Y;
        int ContentHeight => Children.Sum(Element => Element.Size.Y) + (Children.Count - 1) * Padding.Top+ Padding.Bottom;
        public Panel(bool scrollable = false)
        {
            this.Padding = Padding;
            Scrollable = scrollable;
        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            base.Update(MState, KSate);
            if(Scrollable)
            {
                Vector2i ScrollbarPosition = Position + new Vector2i(Size.X - (UIStyle.ScrollbarWidth + 8), 0);
                Vector2i ScrollbarSize = new Vector2i(UIStyle.ScrollbarWidth + 8, Size.Y);
                if(IsHovered)
                {
                    if(MState.ScrollDelta.Y !=0)
                    {
                        ScrollOffset -= (int)MState.ScrollDelta.Y * 25;
                        ScrollOffset = Math.Max(0, Math.Min(ScrollOffset, MaxScroll));
                        //ComputeLayout();
                    }
                    if (!MState.IsButtonDown(MouseButton.Left))
                    {
                        Drag = false;
                    }
                    if (Drag)
                    {
                        ScrollOffset += (int)(MState.Delta.Y *((float)ContentHeight)/Size.Y);
                        ScrollOffset = Math.Max(0, Math.Min(ScrollOffset, MaxScroll));
                        //ComputeLayout();
                    }
                    if (MouseCheck.IsHovering(MState.Position, ScrollbarPosition, ScrollbarPosition + ScrollbarSize))
                    {
                        if (!Drag && MState.IsButtonDown(MouseButton.Left))
                        {
                            Drag = true;
                        }
                    }
                }
            }
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            if (Scrollable)
            {
                RenderScrollBar(Renderer);
                Renderer.StartScissor(Position, new Vector2i(Size.X - (UIStyle.ScrollbarWidth + 8), Size.Y));
            }
            foreach (Element child in Children)
            {
                if (DoBoxesIntersect(Position, Size, child.Position, child.Size))
                {
                    child.Render(Renderer, ScreenSize, ProjectionMatrix);
                }
            }
            if (Scrollable)
            {
                Renderer.EndScissor();
            }
        }
        public void RenderScrollBar(UIRenderer Renderer)
        {
            Vector2i ScrollbarPosition = Position + new Vector2i(Size.X - (UIStyle.ScrollbarWidth + 8),0);
            Vector2i ScrollbarSize = new Vector2i(UIStyle.ScrollbarWidth + 8, Size.Y);
            Renderer.RenderTextureQuad(ScrollbarPosition, ScrollbarSize, UIStyle.ScrollPanelFrame);
            Renderer.RenderTextureQuad(ScrollbarPosition + new Vector2i(4,(int)( (ScrollOffset/(float)MaxScroll) * (Size.Y -ThumbHeight))), new Vector2i(UIStyle.ScrollbarWidth, ThumbHeight), UIStyle.ScrollPanelBase);
        }
        public override Vector2i GetAvailableSpace()
        {
            if(Scrollable)
            {
                return new Vector2i(Size.X - (UIStyle.ScrollbarWidth + 8), Size.Y);
            }
            return base.GetAvailableSpace();
        }
        public override void ComputeLayout()
        {
            int CurrentYPos = Position.Y - ScrollOffset;
            foreach (Element Element in Children)
            {
                CurrentYPos += Element.Padding.Top;
                Element.Position.X = Position.X +Element.Padding.Left;
                Element.Position.Y = CurrentYPos;
                if(Element.ParentSizing)
                {
                    Element.Size.X = GetAvailableSpace().X - Element.Padding.Right -Element.Padding.Right;
                }
                CurrentYPos += Element.Size.Y + Padding.Bottom;
            }
            base.ComputeLayout();
        }
        public static bool DoBoxesIntersect(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            float left1 = pos1.X;
            float right1 = pos1.X + size1.X;
            float top1 = pos1.Y;
            float bottom1 = pos1.Y + size1.Y;
            float left2 = pos2.X;
            float right2 = pos2.X + size2.X;
            float top2 = pos2.Y;
            float bottom2 = pos2.Y + size2.Y;
            bool intersect = !(right1 < left2 ||left1 > right2 ||   bottom1 < top2 ||  top1 > bottom2); 
            return intersect;
        }

    }
}
