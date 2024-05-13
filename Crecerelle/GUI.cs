using Crecerelle.Elements;
namespace Crecerelle
{
    public class GUI : UIElementHolder
    {
        public bool Show;
        public bool CaptInput;
        public bool Fullscreen;
        public bool Active 
        {
            get 
            {
                return Show && CaptInput;    
            }
            set
            {
                Show = value;
                CaptInput = value;
            }
        }
        public List<UIElement> Elements = new();
        public void AddUIElement(UIElement element)
        {
            Elements.Add(element);
        }
    }
}
