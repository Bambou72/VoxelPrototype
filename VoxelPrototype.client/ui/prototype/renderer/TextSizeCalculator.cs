namespace VoxelPrototype.client.ui.prototype.renderer
{
    public static class TextSizeCalculator
    {
        public static int CalculateSize(this string str, int Scale = 2)
        {
            var RealFont = Client.TheClient.FontManager.GetFont("voxelprototype:font/default");
            int size = 0;
            for (int i = 0; i < str.Length; i++)
            {
                size += (RealFont.GetChar(str[i]).Width + 1) * Scale;
            }
            return size;
        }
        public static int CalculateVerticalSize(this string Text, int Scale = 2)
        {
            var RealFont = Client.TheClient.FontManager.GetFont("voxelprototype:font/default");
            int size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                var ch = RealFont.GetChar(Text[i]);
                size = Math.Max(size, ch.Height * Scale);
            }
            return size;
        }
    }
}
