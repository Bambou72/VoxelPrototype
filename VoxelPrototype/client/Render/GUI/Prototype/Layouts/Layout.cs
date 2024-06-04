using OpenTK.Mathematics;
using VoxelPrototype.client.Render.GUI.Prototype;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.Render.GUI.Prototype.Layouts
{
    internal class Layout : UIElement
    {
        internal List<UIElement> Elements = new();
        public void AddElement(UIElement element)
        {
            Elements.Add(element);
        }
        public override void Render(UIRenderer Renderer)
        {
            foreach (UIElement element in Elements)
            {
                element.Render(Renderer);
            }
        }
        public virtual void Compose(Vector2i ScreenSize)
        {
            foreach (UIElement Element in Elements)
            {
                if (Element is Layout)
                {
                    var Lay = (Layout)Element;
                    Lay.Compose(ScreenSize);
                }
            }
        }
        public override void Update(Vector2 MousePos)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {
                    element.Update(MousePos);
                }
            }
        }
        public override bool OnMouseMove(MouseMovedEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnMouseMove(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool OnWheel(MouseWheelEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnWheel(Event))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        public override bool OnMouseDown(MouseDownEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {
                    if (element.OnMouseDown(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool OnMouseUp(MouseUpEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnMouseUp(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool OnMouseClicked(MouseClickedEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnMouseClicked(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool OnKeyDown(KeyDownEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnKeyDown(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override bool OnKeyPressed(KeyPressedEvent Event)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    if (element.OnKeyPressed(Event))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
