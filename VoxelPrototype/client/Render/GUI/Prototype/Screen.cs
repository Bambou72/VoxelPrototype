using OpenTK.Mathematics;
using VoxelPrototype.client.Render.GUI.Prototype.Layouts;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.Render.GUI.Prototype
{
    public class Screen
    {
        internal List<UIElement> Elements = new();
        internal Screen Parent;
        internal Vector2 Size;
        internal virtual void Render(UIRenderer Renderer)
        {
            foreach (UIElement element in Elements)
            {
                element.Render(Renderer);
            }
        }
        internal void Close()
        {
            //Client.TheClient.UIManager.SetCurrentScreen(null);
            //Client.TheClient.UIManager.SetCurrentScreen(Parent);
        }
        public void AddElement(UIElement element)
        {
            Elements.Add(element);
        }
        public virtual void Compose(Vector2i ScreenSize)
        {
            Size = ScreenSize;
            foreach (UIElement Element in Elements)
            {
                if (Element is Layout)
                {
                    var Lay = (Layout)Element;
                    Lay.Compose(ScreenSize);
                }
            }
        }
        public void Update(Vector2i MousePos)
        {
            foreach (UIElement element in Elements)
            {
                if (element.Active)
                {

                    element.Update(MousePos);
                }
            }
        }
        public virtual bool OnMouseMove(MouseMovedEvent Event)
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
        public virtual bool OnWheel(MouseWheelEvent Event)
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
        public virtual bool OnMouseDown(MouseDownEvent Event)
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
        public virtual bool OnMouseUp(MouseUpEvent Event)
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
        public virtual bool OnMouseClicked(MouseClickedEvent Event)
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
        public virtual bool OnKeyDown(KeyDownEvent Event)
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
        public virtual bool OnKeyPressed(KeyPressedEvent Event)
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
