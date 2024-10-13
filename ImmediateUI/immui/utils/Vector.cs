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

        public void PushBack(T value)
        {
            if (Size == Capacity)
                Reserve(GrowCapacity(Size + 1));
            Data[Size++] = value;
        }
    }
}
