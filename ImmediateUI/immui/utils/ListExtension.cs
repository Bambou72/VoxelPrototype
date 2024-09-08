namespace ImmediateUI.immui.utils
{
    public static class ListExtension
    {
        public static void Resize<T>(this List<T> list, int NewSize)
        {
            if (list.Count < NewSize)
            {
                list.AddRange(Enumerable.Repeat(default(T), NewSize - list.Count));

            }
        }
    }
}
