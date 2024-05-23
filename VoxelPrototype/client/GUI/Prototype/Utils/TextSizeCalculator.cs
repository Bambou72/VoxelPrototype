using VoxelPrototype.client.Render.Text;
namespace VoxelPrototype.client.GUI.Prototype.Utils
{
    public static class TextSizeCalculator
    {
        public static float CalculateSize(string Font, float Scale, string Text)
        {
            var RealFont = Client.TheClient.ResourcePackManager.GetFont(Font);

            Scale = Scale / RealFont.FontSize;
            float size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                size += (RealFont.GetCharacter(Text[i]).Advance >> 6) * Scale;
            }
            return size;
        }
        public static float CalculateVerticalSize(string Font, float Scale, string Text)
        {
            var RealFont = Client.TheClient.ResourcePackManager.GetFont(Font);
            Scale = Scale / RealFont.FontSize;

            float size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                var ch = RealFont.GetCharacter(Text[i]);
                size = Math.Max(size, ch.Size.Y * Scale);
            }
            return size;
        }
    }
}
