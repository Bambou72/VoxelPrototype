using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
namespace VoxelPrototype.client.ui.elements.container
{
    public class Container : Element
    {
        public List<Element> Children = new();


        public virtual Vector2i GetAvailableSpace()
        {
            return Size;
        }
        public virtual void ComputeLayout()
        {
            foreach (Element child in Children)
            {
                if (child is Container)
                {
                    Container UIChild = (Container)child;
                    UIChild.ComputeLayout();
                }
            }

        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            foreach (Element child in Children)
            {
                 child.Update(MState, KSate);
            }
           
        }

        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            foreach (Element child in Children)
            {
                child.Render(Renderer, ScreenSize, ProjectionMatrix);
            }
        }
    }
}
