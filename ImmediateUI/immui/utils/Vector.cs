namespace ImmediateUI.immui.utils
{


    public class Vector<T>
    {
        public int Size { get; set; }
        public int Capacity { get;  set; }
        public T[] Data { get;  set; }

        public Vector()
        {
            Size = Capacity = 0;
            Data = null;
        }

        public Vector(Vector<T> src)
        {
            Size = Capacity = 0;
            Data = null;
            Assign(src);
        }

        public Vector<T> Assign(Vector<T> src)
        {
            Clear();
            Resize(src.Size);
            if (src.Data != null)
                Array.Copy(src.Data, Data, Size);
            return this;
        }




        public void Clear()
        {
            if (Data != null)
            {
                Size = Capacity = 0;
                Data = null;
            }
        }

        public void ClearDelete()
        {
            for (int n = 0; n < Size; n++)
                (Data[n] as IDisposable)?.Dispose(); // Assumes T is IDisposable
            Clear();
        }

        public void ClearDestruct()
        {
            for (int n = 0; n < Size; n++)
                Data[n] = default(T); // Calls destructor for value types
            Clear();
        }

        public bool IsEmpty() => Size == 0;
        public int GetSize() => Size;
        public int SizeInBytes() => Size * System.Runtime.InteropServices.Marshal.SizeOf<T>();
        public int MaxSize() => int.MaxValue / System.Runtime.InteropServices.Marshal.SizeOf<T>();
        public int GetCapacity() => Capacity;

        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= Size)
                    throw new ArgumentOutOfRangeException();
                return Data[i];
            }
            set
            {
                if (i < 0 || i >= Size)
                    throw new ArgumentOutOfRangeException();
                Data[i] = value;
            }
        }

        public T[] Begin() => Data;
        public T[] End() => Data == null ? null : new T[Size];
        public T Front()
        {
            if (Size <= 0)
                throw new InvalidOperationException("Vector is empty");
            return Data[0];
        }

        public T Back()
        {
            if (Size <= 0)
                throw new InvalidOperationException("Vector is empty");
            return Data[Size - 1];
        }

        public void Swap(Vector<T> rhs)
        {
            int tempSize = rhs.Size;
            rhs.Size = Size;
            Size = tempSize;

            int tempCap = rhs.Capacity;
            rhs.Capacity = Capacity;
            Capacity = tempCap;

            T[] tempData = rhs.Data;
            rhs.Data = Data;
            Data = tempData;
        }

        private int GrowCapacity(int sz)
        {
            int newCapacity = Capacity > 0 ? (Capacity + Capacity / 2) : 8;
            return newCapacity > sz ? newCapacity : sz;
        }

        public void Resize(int newSize)
        {
            if (newSize > Capacity)
                Reserve(GrowCapacity(newSize));
            Size = newSize;
        }

        public void Resize(int newSize, T value)
        {
            if (newSize > Capacity)
                Reserve(GrowCapacity(newSize));
            if (newSize > Size)
                for (int n = Size; n < newSize; n++)
                    Data[n] = value;
            Size = newSize;
        }

        public void Shrink(int newSize)
        {
            if (newSize > Size)
                throw new ArgumentException("New size must be smaller than the current size.");
            Size = newSize;
        }

        public void Reserve(int newCapacity)
        {
            if (newCapacity <= Capacity)
                return;

            T[] newData = new T[newCapacity];
            if (Data != null)
                Array.Copy(Data, newData, Size);
            Data = newData;
            Capacity = newCapacity;
        }

        public void ReserveDiscard(int newCapacity)
        {
            if (newCapacity <= Capacity)
                return;

            Data = new T[newCapacity];
            Capacity = newCapacity;
        }

        public void PushBack(T value)
        {
            if (Size == Capacity)
                Reserve(GrowCapacity(Size + 1));
            Data[Size++] = value;
        }

        public void PopBack()
        {
            if (Size <= 0)
                throw new InvalidOperationException("Vector is empty");
            Size--;
        }

        public void PushFront(T value)
        {
            if (Size == 0)
            {
                PushBack(value);
            }
            else
            {
                Insert(Data, value);
            }
        }

        public T[] Erase(T[] it)
        {
            int index = Array.IndexOf(Data, it);
            if (index < 0 || index >= Size)
                throw new ArgumentException("Iterator out of range");

            Array.Copy(Data, index + 1, Data, index, Size - index - 1);
            Size--;
            return Data;
        }

        public T[] Erase(T[] it, T[] itLast)
        {
            int index = Array.IndexOf(Data, it);
            int lastIndex = Array.IndexOf(Data, itLast);
            if (index < 0 || lastIndex < 0 || index >= Size || lastIndex > Size)
                throw new ArgumentException("Iterator out of range");

            int count = lastIndex - index;
            Array.Copy(Data, index + count, Data, index, Size - index - count);
            Size -= count;
            return Data;
        }

        public T[] EraseUnsorted(T[] it)
        {
            int index = Array.IndexOf(Data, it);
            if (index < 0 || index >= Size)
                throw new ArgumentException("Iterator out of range");

            if (index < Size - 1)
                Data[index] = Data[Size - 1];
            Size--;
            return Data;
        }

        public T[] Insert(T[] it, T value)
        {
            int index = Array.IndexOf(Data, it);
            if (index < 0 || index > Size)
                throw new ArgumentException("Iterator out of range");

            if (Size == Capacity)
                Reserve(GrowCapacity(Size + 1));
            Array.Copy(Data, index, Data, index + 1, Size - index);
            Data[index] = value;
            Size++;
            return Data;
        }

        public bool Contains(T value) => Array.Exists(Data, element => EqualityComparer<T>.Default.Equals(element, value));
        public T Find(T value) => Array.Find(Data, element => EqualityComparer<T>.Default.Equals(element, value));
        public int FindIndex(T value) => Array.IndexOf(Data, value);
        public bool FindErase(T value)
        {
            int index = FindIndex(value);
            if (index >= 0)
            {
                Erase(new T[] { Data[index] });
                return true;
            }
            return false;
        }

        public bool FindEraseUnsorted(T value)
        {
            int index = FindIndex(value);
            if (index >= 0)
            {
                EraseUnsorted(new T[] { Data[index] });
                return true;
            }
            return false;
        }

        public int IndexFromPtr(T[] it)
        {
            int index = Array.IndexOf(Data, it);
            if (index < 0 || index >= Size)
                throw new ArgumentException("Iterator out of range");
            return index;
        }
    }

}
