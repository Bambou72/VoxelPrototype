using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelPrototype.client.Render.UI;
namespace VoxelPrototype.client.GUI.Prototype
{
    public class UIManager
    {
        UIRenderer Renderer { get; } = new();
        Screen Screen = null;
        public UIManager()
        {
        }
        public void SetCurrentScreen(Screen? screen)
        {
            Screen = screen;
            if (screen != null)
            {
                Vector2i ScreenSize = Client.TheClient.ClientSize;
                Screen.Compose(ScreenSize);
            }
        }

        public void Update()
        {
            Vector2i ScreenSize = Client.TheClient.ClientSize;
            Vector2 MousePos = Client.TheClient.MousePosition;
            if (Screen != null)
            {
                Screen.Compose(ScreenSize);
                Screen.Update((Vector2i)MousePos);
            }
        }
        public void Render()
        {
            if (Screen != null)
            {
                Screen.Render(Renderer);
            }
            Renderer.FinishRendering();
        }
        public bool OnMouseMove(MouseMovedEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnMouseMove(Event))
                {
                    return true;
                }
            }
            return false;
        }
        public bool OnMouseWheel(MouseWheelEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnWheel(Event))
                {
                    return true;
                }
            }
            return false;
        }

        public bool OnMouseDown(MouseDownEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnMouseDown(Event))
                {
                    return true;
                }
            }
            return false;
        }
        public bool OnMouseUp(MouseUpEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnMouseUp(Event))
                {
                    return true;
                }
            }
            return false;
        }
        public bool OnMouseClicked(MouseClickedEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnMouseClicked(Event))
                {
                    return true;
                }
            }
            return false;

        }
        public bool OnKeyDown(KeyDownEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnKeyDown(Event))
                {
                    return true;
                }
            }
            return false;

        }
        public bool OnKeyPressed(KeyPressedEvent Event)
        {
            if (Screen != null)
            {
                if (Screen.OnKeyPressed(Event))
                {
                    return true;
                }
            }
            return false;

        }

    }
}
