using OpenTK.Mathematics;
using VoxelPrototype.utils;
using VoxelPrototype.client.ui.renderer;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.utils;
namespace VoxelPrototype.client.ui.elements
{
    internal class CheckBox : Element
    {
        bool Check;
        string Label;
        private Func<bool> GetCheck;
        private Action<bool> SetCheck;
        public CheckBox(string label, Action<bool> setCheck, Func<bool> getCheck = null)
        {
            Label = label;
            Size = new(30);
            GetCheck = getCheck;
            SetCheck = setCheck;
        }

        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, IsHovered ? UIStyle.HoveredCheckBox : UIStyle.BaseCheckBox);
            Renderer.RenderText(Label, Position + new Vector2i(Size.X + 10, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Label) / 2));
            if (Check)
            {
                Renderer.RenderTextureQuad(Position + new Vector2i(3), Size - new Vector2i(6), UIStyle.ActiveCheckBox);
            }

        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            base.Update(MState, KSate);
            if (GetCheck != null)
            {
                Check = GetCheck();
            }
            if (IsHovered && MState.IsButtonPressed(MouseButton.Left))
            {
                Check = !Check;
                SetCheck(Check);
            }
        }
    }
}
