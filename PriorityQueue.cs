using System;
using System.Collections.Generic;
using System.Text;

namespace PacmanGame
{
    // Min-Heap based Priority Queue
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> m_items;
        private int ValidCount;

        public PriorityQueue()
        {
            m_items = new List<T>();
            ValidCount = 0;
        }

        public void Push(T item)
        {
            if (m_items.Count == ValidCount)
                m_items.Add(item);
            else if (m_items.Count > ValidCount)
                m_items[ValidCount] = item;

            HeapifyUp(ValidCount);
            ValidCount++;
        }

        public void Pop()
        {
            if (ValidCount > 0)
            {
                ValidCount--;
                m_items[0] = m_items[ValidCount];
                HeapifyDown(0);
            }
        }

        public T Top()
        {
            if (ValidCount == 0)
                throw new Exception("Queue is empty");

            return m_items[0];
        }

        public bool Empty()
        {
            return ValidCount == 0;
        }

        // O(n) approach (slow)
        // TODO: Improve when time allows.
        public void Update(T item)
        {
            int index = -1;
            for (int i = 0; i < ValidCount; ++i)
                if (item.CompareTo(m_items[i]) == 0)
                {
                    index = i;
                    break;
                }
                    
            if (index >= 0)
            {
                // Logically, only one of the following will execute.
                HeapifyUp(index);
                HeapifyDown(index);
            }
        }

        public void Debug()
        {
            for (int i = 0; i < ValidCount; ++i)
                Console.Write(m_items[i] + " ");
            Console.WriteLine();
        }

        private void HeapifyUp(int index)
        {
            int parent = (index - 1) / 2;
            T copy = m_items[index];

            while (index > 0 && m_items[parent].CompareTo(copy) > 0)
            {
                m_items[index] = m_items[parent];

                index = parent;
                parent = (parent - 1) / 2;
            }

            m_items[index] = copy;
        }

        private void HeapifyDown(int index)
        {
            T copy = m_items[index];

            int left = index * 2 + 1;
            int right = index * 2 + 2;

            while (left < ValidCount)
            {
                int min = left;
                if (right < ValidCount && m_items[left].CompareTo(m_items[right]) > 0)
                    min = right;

                if (copy.CompareTo(m_items[min]) > 0)
                {
                    m_items[index] = m_items[min];
                    index = min;
                }
                else break;

                left = index * 2 + 1;
                right = index * 2 + 2;
            }

            m_items[index] = copy;
        }

        
    }
}
