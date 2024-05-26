namespace VoxelPrototype.common.Utils
{
    public static class ListExtensions
    {
        /// <summary>
        /// Moves an object in the list to the first position.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to modify.</param>
        /// <param name="item">The object to move to the first position.</param>
        public static void MoveToFirstPosition<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index != -1)
            {
                list.RemoveAt(index);
                list.Insert(0, item);
            }
        }

        /// <summary>
        /// Moves an object in the list up by one position.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to modify.</param>
        /// <param name="item">The object to move up.</param>
        public static void MoveUp<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index > 0)
            {
                list.RemoveAt(index);
                list.Insert(index - 1, item);
            }
        }

        /// <summary>
        /// Moves an object in the list down by one position.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to modify.</param>
        /// <param name="item">The object to move down.</param>
        public static void MoveDown<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index != -1 && index < list.Count - 1)
            {
                list.RemoveAt(index);
                list.Insert(index + 1, item);
            }
        }
    }
}
