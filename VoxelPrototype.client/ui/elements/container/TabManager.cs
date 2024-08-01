using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
using VoxelPrototype.client.ui.utils;
namespace VoxelPrototype.client.ui.elements.container
{
    internal class TabManager : Element
    {
        List<TabPage> Tabs = new();
        TabPage ActiveTabPage;
        int TabSelectorHeight = 50;
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, new(Size.X, TabSelectorHeight), UIStyle.TabSelectorFrame);
            foreach (TabPage p in Tabs)
            {
                Renderer.RenderTextureQuad(p.TabSelectorPos, p.TabSelectorSize, p.TabHovered ? UIStyle.TabSelectorHovered : UIStyle.TabSelectorBase);
                Renderer.RenderTextCentered(p.Name, new Vector2i(p.TabSelectorPos.X + p.TabSelectorSize.X / 2, p.TabSelectorPos.Y + (TabSelectorHeight - 5) / 2 + p.Name.CalculateVerticalSize() / 2));
            }
            if (ActiveTabPage != null)
            {
                ActiveTabPage.Render(Renderer, ScreenSize, ProjectionMatrix);
            }
        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            if (ActiveTabPage == null)
            {
                if (Tabs.Count > 0)
                {
                    ActiveTabPage = Tabs[0];
                }
            }

            if (ActiveTabPage != null)
            {
                ActiveTabPage.Position = new(Position.X , Position.Y + TabSelectorHeight +20);
                ActiveTabPage.Size = new(Size.X - Position.X, Size.Y - (Position.Y + TabSelectorHeight));
                ActiveTabPage.ComputeLayout();
                ActiveTabPage.Update(MState, KSate);

            }

            int XPos = Position.X + 5;
            foreach (TabPage p in Tabs)
            {
                int TabSize = p.Name.CalculateSize() + 20;
                bool Hovered = MouseCheck.IsHovering(Client.TheClient.MousePosition, new Vector2(XPos, Position.Y + 5), new Vector2(XPos, Position.Y + 5) + new Vector2(TabSize, TabSelectorHeight - 5));
                if (Hovered && MState.IsButtonPressed(MouseButton.Left))
                {
                    ActiveTabPage = p;
                }
                p.TabHovered = Hovered;
                p.TabSelectorPos = new Vector2i(XPos, Position.Y + 5);
                p.TabSelectorSize = new(TabSize, TabSelectorHeight - 5);
                XPos += TabSize + 5;
            }
        }
        public void AddTab(TabPage p)
        {
            Tabs.Add(p);
        }
    }
    internal class TabPage : Container
    {
        internal string Name;
        internal Vector2i TabSelectorPos;
        internal Vector2i TabSelectorSize;
        internal bool TabHovered;
        public override void ComputeLayout()
        {
            foreach (var Child in Children)
            {
                Child.Position = Position;
                Child.Size = Size;
            }
            base.ComputeLayout();
        }
    }
}
