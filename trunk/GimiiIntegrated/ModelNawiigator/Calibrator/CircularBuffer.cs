using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAExample
{
    public class CircularBuffer<T>
    {
        private T[] data;
        private int capacity;

        private int first = 0;
        private int last = -1;

        public CircularBuffer(int capacity)
        {
            this.capacity = capacity;
            data = new T[capacity];
        }

        public bool Empty
        {
            get { return last == -1; }
        }

        public int Capacity
        {
            get { return capacity; }
        }

        public int Count
        {
            get
            {
                if(last == -1)
                {
                    return 0;
                }
                else if (last >= first)
                {
                    return last - first + 1;
                }
                else
                {
                    return capacity;
                }
            }
        }

        public void add(T item)
        {
            if (last == -1)
            {
                first = 0;
                last = 0;
                data[0] = item;
            }
            else
            {
                last = (last + 1) % capacity;
                data[last] = item;
                if (last > first)
                {
                    //NOOP
                }
                else
                {
                    first = (first + 1) % capacity;
                }
            }
        }

        public void clear()
        {
            first = 0;
            last = -1;
        }

        public T this[int index]
        {
            set {
                if (last == -1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (last > first && (first + index) % capacity > last)
                {
                    throw new ArgumentOutOfRangeException();
                }
                data[(first + index) % capacity] = value; 
            }
            get {
                if (last == -1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (last > first && (first + index) % capacity > last)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return data[(first + index) % capacity]; 
            }
        }

    }
}
