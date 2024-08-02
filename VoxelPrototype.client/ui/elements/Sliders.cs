using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelPrototype.client.ui.renderer;
using VoxelPrototype.client.ui.utils;

namespace VoxelPrototype.client.ui.elements
{
    internal class SliderFloat : Element
    {
        float Value;
        float Min;
        float Max;
        float Step;
        float Range
        {
            get
            {
                return Max - Min;
            }
        }
        string Label;
        private Func<float> GetValue;
        private Action<float> SetValue;

        public SliderFloat(string label, Action<float> setValue, Func<float> getValue = null, float min = 0, float max = 100, float step = 1)
        {
            Label = label;
            GetValue = getValue;
            SetValue = setValue;
            Min = min;
            Max = max;
            Step = step;
            Size = new(500, 30);
            Value = Min;
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.BaseSlider);
            Renderer.RenderText(Label, Position + new Vector2i(Size.X + 10, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Label) / 2));
            //GL.Scissor(Position.X , ScreenSize.Y-( Position.Y+Size.Y), Size.X, Size.Y);
            //GL.Enable(EnableCap.ScissorTest);
            Renderer.RenderTextureQuad(Position + new Vector2i((int)(Value / Max * Size.X - 5), 0), new Vector2i(8, Size.Y), UIStyle.SliderHandle);
            //GL.Disable(EnableCap.ScissorTest);
            Renderer.RenderTextCentered(Value.ToString(), Position + new Vector2i(Size.X / 2, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Label) / 2));
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }
        public static float StepToNearest(float value, float step)
        {
            float steps = (float)Math.Round(value / step);
            return steps * step;
        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            if (GetValue != null)
            {
                Value = GetValue();
            }
            if (MouseCheck.IsHovering(Client.TheClient.MousePosition, Position, Position + Size) && MState.IsButtonDown(MouseButton.Left))
            {
                float rawValue = Min + (Client.TheClient.MousePosition.X - Position.X) * Range / Size.X;
                Value = MathF.Max(Min, MathF.Min(Max, StepToNearest(rawValue, Step)));
                SetValue(Value);
            }
            base.Update(MState, KSate);
        }
        static float GetClosestNumber(float value, float step)
        {
            // Get the absolute values of our arguments
            var absValue = Math.Abs(value);
            step = Math.Abs(step);

            // Determing the numbers on either side of value
            var low = absValue - absValue % step;
            var high = low + step;

            // Return the closest one, multiplied by -1 if value < 0
            var result = absValue - low < high - absValue ? low : high;
            return result * Math.Sign(value);
        }
    }
    internal class SliderInt : Element
    {
        int Value;
        int Min;
        int Max;
        int Step;
        int Range
        {
            get
            {
                return Max - Min;
            }
        }
        string Label;
        private Func<int> GetValue;
        private Action<int> SetValue;

        public SliderInt(string label, Action<int> setValue, Func<int> getValue = null, int min = 0, int max = 100, int step = 1)
        {
            Label = label;
            GetValue = getValue;
            SetValue = setValue;
            Min = min;
            Max = max;
            Step = step;
            Size = new(500, 30);
            Value = Min;
        }
        public override void Render(UIRenderer Renderer, Vector2i ScreenSize, Matrix4 ProjectionMatrix)
        {
            Renderer.RenderTextureQuad(Position, Size, UIStyle.BaseSlider);
            Renderer.RenderText(Label, Position + new Vector2i(Size.X + 10, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Label) / 2));
            //GL.Scissor(Position.X, ScreenSize.Y - (Position.Y + Size.Y), Size.X, Size.Y);
            //GL.Enable(EnableCap.ScissorTest);
            Renderer.RenderTextureQuad(Position + new Vector2i((int)(Value / (float)Max * Size.X) - 5, 0), new Vector2i(8, Size.Y), UIStyle.SliderHandle);
            //GL.Disable(EnableCap.ScissorTest);
            Renderer.RenderTextCentered(Value.ToString(), Position + new Vector2i(Size.X / 2, Size.Y / 2 + TextSizeCalculator.CalculateVerticalSize(Label) / 2));
            base.Render(Renderer, ScreenSize, ProjectionMatrix);
        }
        public static int StepToNearest(int value, int step)
        {
            int steps = value / step;
            return steps * step;
        }
        public override void Update(MouseState MState, KeyboardState KSate)
        {
            if (GetValue != null)
            {
                Value = GetValue();
            }
            if (MouseCheck.IsHovering(Client.TheClient.MousePosition, Position, Position + Size) && MState.IsButtonDown(MouseButton.Left))
            {
                int rawValue = Min + ((int)Client.TheClient.MousePosition.X - Position.X) * Range / Size.X;
                Value = Math.Max(Min, Math.Min(Max, StepToNearest(rawValue, Step)));
                SetValue(Value);
            }
            base.Update(MState, KSate);
        }
        static float GetClosestNumber(float value, float step)
        {
            // Get the absolute values of our arguments
            var absValue = Math.Abs(value);
            step = Math.Abs(step);

            // Determing the numbers on either side of value
            var low = absValue - absValue % step;
            var high = low + step;

            // Return the closest one, multiplied by -1 if value < 0
            var result = absValue - low < high - absValue ? low : high;
            return result * Math.Sign(value);
        }
    }
}
