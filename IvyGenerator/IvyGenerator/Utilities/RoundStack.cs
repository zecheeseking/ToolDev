using System;
using GalaSoft.MvvmLight;

namespace IvyGenerator.Utilities
{
    [Serializable]
    public class RoundStack<T>
    {
        private T[] items;

        private int top = 1;
        private int bottom = 0;

        public bool IsFull
        {
            get { return top == bottom; }
        }

        public int Count
        {
            get
            {
                int count = top - bottom - 1;
                if (count < 0)
                    count += items.Length;
                return count;
            }
        }

        public int Capacity
        {
            get
            {
                return items.Length - 1;
            }
        }

        public RoundStack(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("Capacity needs to be at least 1");
            items = new T[capacity + 1];
        }

        public T Pop()
        {
            if (Count > 0)
            {
                T removed = items[top];
                items[top--] = default(T);
                if (top < 0)
                    top += items.Length;
                return removed;
            }
            else
                throw new InvalidOperationException("Cannot pop from empty stack");
        }

        public void Push(T item)
        {
            if (IsFull)
            {
                bottom++;
                if (bottom >= items.Length)
                    bottom -= items.Length;
            }
            if (++top >= items.Length)
                top -= items.Length;
            items[top] = item;
        }

        public T Peek()
        {
            return items[top];
        }

        public void Clear()
        {
            if (Count > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = default(T);
                }
                top = 1;
                bottom = 0;
            }
        }
    }
}