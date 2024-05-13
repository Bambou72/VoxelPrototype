using VoxelPrototype.client.Render.Text;
namespace VoxelPrototype.client.Render.UI
{
    public static class TextSizeCalculator
    {
        public static float CalculateSize(Font Font, float Scale, string Text)
        {
            Scale = Scale / Font.FontSize;
            float size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                size += (Font.GetCharacter(Text[i]).Advance >> 6) * Scale;
            }
            return size;
        }
        public static float CalculateVerticalSize(Font Font, float Scale, string Text)
        {
            Scale = Scale / Font.FontSize;
            
            float size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                var ch = Font.GetCharacter(Text[i]);
                size =  Math.Max(size,(ch.Size.Y)* Scale);
            }
            return size *1.3f;
        }
    }
}
