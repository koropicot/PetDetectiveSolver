using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetDetectiveSolver
{
    public class PriorityQueue<T>
    {
        private List<T> array;
        private IComparer<T> comparer 
            = Comparer<T>.Create((x,y) => Comparer<T>.Default.Compare(y,x));

        public int Count { get { return array.Count; } }

        public PriorityQueue() : this(new List<T>(), Comparer<T>.Default) { }

        public PriorityQueue(IComparer<T> comparer) : this(new List<T>(), comparer) { }

        public PriorityQueue(IEnumerable<T> source) : this(source, Comparer<T>.Default) { }

        public PriorityQueue(IEnumerable<T> source, IComparer<T> comparer) : this(source.ToList(), comparer) { }

        private PriorityQueue(List<T> source, IComparer<T> comparer)
        {
            array = source;
            this.comparer = comparer;
            array.Sort(Comparer<T>.Create((x,y)=>comparer.Compare(y,x)));
        }

        public void Enqueue(T item)
        {
            array.Add(default(T));
            var current = array.Count - 1;
            while (current != 0)  //入れる場所まで辿る
            {
                var parent = (current - 1) / 2;
                if (comparer.Compare(array[parent], item) >= 0)
                    break;
                array[current] = array[parent];
                current = parent;
            }
            array[current] = item;
        }

        public T Dequeue()
        {
            if (array.Count == 0)
                throw new InvalidOperationException("The queue is empty.");
            var ret = array[0];
            var last = array[array.Count - 1];
            array.RemoveAt(array.Count - 1);
            var lastIndex = array.Count - 1;

            var current = 0;
            while (current * 2 + 1 <= lastIndex)
            {
                var childL = current * 2 + 1;
                var childR = childL + 1;
                var child = (childR > lastIndex) || (comparer.Compare(array[childL], array[childR]) >= 0) ? childL : childR;
                if (comparer.Compare(last, array[child]) >= 0)
                    break;
                array[current] = array[child];
                current = child;
            }
            if(array.Count > 0)
                array[current] = last;

            return ret;
        }
    }
}
