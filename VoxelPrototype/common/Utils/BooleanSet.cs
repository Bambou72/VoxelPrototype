using System.Collections;
namespace VoxelPrototype.common.Utils
{
    public class BooleanSet : ISet<bool>
    {
        public static readonly BooleanSet IMMUTABLE = new BooleanSet();
        private BooleanSet() { }
        private class BooleansIterator : IEnumerator<bool>
        {
            private byte next = 0;
            public bool MoveNext()
            {
                return next < 2;
            }
            public void Reset()
            {
                next = 0;
            }
            object IEnumerator.Current => Current;
            public bool Current
            {
                get
                {
                    if (next > 1) throw new InvalidOperationException();
                    return (next++) == 1;
                }
            }
            public void Dispose()
            {
                // Dispose method if necessary
            }
        }
        public int Count => 2;
        public bool IsReadOnly => true;
        public bool Contains(bool item)
        {
            return item == true || item == false;
        }
        public IEnumerator<bool> GetEnumerator()
        {
            return new BooleansIterator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Add(bool item)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public void Clear()
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public void CopyTo(bool[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index is out of range.");
            if (array.Length - arrayIndex < 2)
                throw new ArgumentException("Not enough space in array.");
            array[arrayIndex] = false;
            array[arrayIndex + 1] = true;
        }
        public void ExceptWith(IEnumerable<bool> other)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public void IntersectWith(IEnumerable<bool> other)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public bool IsProperSubsetOf(IEnumerable<bool> other)
        {
            return other.Contains(false) && other.Contains(true);
        }
        public bool IsProperSupersetOf(IEnumerable<bool> other)
        {
            return Count == 2 && other.Count() > 0 && !other.Contains(false) && !other.Contains(true);
        }
        public bool IsSubsetOf(IEnumerable<bool> other)
        {
            return other.Contains(false) && other.Contains(true);
        }
        public bool IsSupersetOf(IEnumerable<bool> other)
        {
            return Count == 2 && !other.Contains(false) && !other.Contains(true);
        }
        public bool Overlaps(IEnumerable<bool> other)
        {
            return other.Contains(false) || other.Contains(true);
        }
        public bool Remove(bool item)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public bool SetEquals(IEnumerable<bool> other)
        {
            return other.Contains(false) && other.Contains(true) && other.Count() == 2;
        }
        public void SymmetricExceptWith(IEnumerable<bool> other)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        public void UnionWith(IEnumerable<bool> other)
        {
            throw new NotSupportedException("Can't change boolean set structure.");
        }
        bool ISet<bool>.Add(bool item)
        {
            throw new NotImplementedException();
        }
    }
}
