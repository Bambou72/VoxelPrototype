namespace Crecerelle.Utils
{
    public static class ListExtensions
    {
        public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }
        public static void MoveItemAtIndexToFront<T>(this List<T> list, T value)
        {
            int Index = list.IndexOf(value);
            if (Index != -1)
            {
                list.RemoveAt(Index);
                list.Insert(0, value);
            }
        }
        public static void MoveItemUp<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, item);
        }
        public static void MoveItemUp<T>(this List<T> list, T value)
        {
            int Index = list.IndexOf(value);
            if (Index != -1 && Index != 0)
            {
                list.RemoveAt(Index);
                list.Insert(Index - 1, value);
            }
        }
    }
}
