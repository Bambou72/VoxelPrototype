using Crecerelle.Renderer;
using Crecerelle.Utils;
using OpenTK.Mathematics;
namespace Crecerelle
{
    public class UIManager
    {
        public static Vector2i ClientSize = Vector2i.Zero;
        public static Vector2i MousePos = Vector2i.Zero;
        IUIRenderer Renderer;
        List<GUI> GUIS;
        public bool IsDraging = false;
        public bool InputCaptured = false;
        public UIManager(IUIRenderer Renderer)
        {
            this.Renderer = Renderer;
            GUIS = new();
        }
        public void Update(Vector2i clientSize, Vector2i mousePos)
        {
            ClientSize = clientSize;
            MousePos = mousePos;
            InputCaptured = false;
            float zorder = -1f;
            for (int i = 0; i < GUIS.Count; i++)
            {
                GUI UI = GUIS[i];
                UI.ZOrder = zorder;

                if (UI.Fullscreen)
                {
                    UI.Size = ClientSize;
                }
                if (UI.Show || UI.CaptInput)
                {
                    if(UI.CaptInput)
                    {
                        UI.Update(this);
                    }
                    if(UI.Show)
                    {
                        UI.Render(Renderer);
                    }
                    zorder -= 0.01f;
                }
            }

        }
        public void Render()
        {
            Renderer.Render();
        }
        public void AddUI(GUI UI)
        {
            GUIS.Add(UI);
        }
        public void SetFrontUI(GUI UI)
        {
            GUIS.MoveItemAtIndexToFront(UI);
        }
        public void MoveUpUI(GUI UI)
        {
            GUIS.MoveItemUp(UI);
        }
    }
}
