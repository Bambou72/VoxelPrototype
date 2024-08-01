namespace VoxelPrototype.client.ui.utils
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
        public static int CalculateSizeExternal(string Text, int Scale = 2)
        {
            var RealFont = Client.TheClient.FontManager.GetFont("voxelprototype:font/default");
            int size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                size += (RealFont.GetChar(Text[i]).Width + 1) * Scale;
            }
            return size;
        }
        public static int CalculateSize(string Font, string Text, int Scale = 2)
        {
            var RealFont = Client.TheClient.FontManager.GetFont(Font);
            int size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                size += (RealFont.GetChar(Text[i]).Width + 1) * Scale;
            }
            return size;
        }
        public static int CalculateVerticalSizeExternal(string Font, string Text, int Scale = 2)
        {
            var RealFont = Client.TheClient.FontManager.GetFont(Font);
            int size = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                var ch = RealFont.GetChar(Text[i]);
                size = Math.Max(size, ch.Height * Scale);
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
        public static int CalculateVerticalSizeExternal(string Text, int Scale = 2)
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
