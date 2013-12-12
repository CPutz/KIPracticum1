using System;
using System.Collections.Generic;

namespace Ants {

    /// <summary>
    /// Just a List for Locations that is extended with a boolean map for a fast Contains function.
    /// </summary>
    public class Map<T> : IEnumerable<T> where T : Location {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Count { get { return items.Count; } }

        private bool[,] map;
        private List<T> items;

        public Map(int width, int height)
            : base() {
            this.Width = width;
            this.Height = height;

            this.map = new bool[Height, Width];
            this.items = new List<T>();
        }

        public T this[int index] {
            get { return items[index]; }
        }

        public bool Contains(Location loc) {
            return map[loc.Row, loc.Col];
        }

        public bool Contains(int row, int col) {
            return map[row, col];
        }

        public int IndexOf(T item) {
            return items.IndexOf(item);
        }

        public void Add(T item) {
            if (!map[item.Row, item.Col]) {
                map[item.Row, item.Col] = true;
                items.Add(item);
            } else {
                throw new ArgumentException("There already exists an element with this Location");
            }
        }

        public void Remove(T item) {
            map[item.Row, item.Col] = false;
            items.Remove(item);
        }

        public void Clear() {
            map = new bool[Height, Width];
            items.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (T item in items) {
                if (item == null) {
                    break;
                }
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }


    class MinHeap<T> : Heap<T> where T : IComparable {
        public MinHeap(int maxLength) : base(maxLength) { }

        protected override bool Compare(T A, T B) {
            if (A.CompareTo(B) <= 0)
                return true;
            else 
                return false;
        }
    }

    abstract class Heap<T> {
        protected T[] A;
        public int Size { get; private set; }

        public Heap(int maxLength) {
            A = new T[maxLength];
            this.Size = 0;
        }

        //returns true if A is greater than B
        protected abstract bool Compare(T A, T B);

        //Inserts an element in the heap
        //and returns the elements new index
        virtual public int Insert(T key) {
            this.Size++;
            return this.IncreaseKey(this.Size - 1, key);
        }

        //Changes the value of the element on index i to key
        public virtual void ChangeKey(int i, T key) {
            if (Compare(key, A[i])) {
                if (Compare(A[i], key)) {
                    A[i] = key;
                    return; //if key == A[i], only change element for key
                } else {
                    IncreaseKey(i, key);
                }
            } else {
                DecreaseKey(i, key);
            }
        }

        //Extracts the element on index i
        public virtual T Extract(int i) {
            T val = A[i];

            this.Size--;
            //Swap(i, this.mSize);
            ChangeKey(i, A[Size]);

            return val;
        }

        //Extracts the element with the largest value
        public virtual T ExtractMax() {
            T val = A[0];

            this.Size--;
            Swap(0, this.Size);
            Heapify(0);

            return val;
        }

        private void Heapify(int i) {
            int l = Left(i);
            int r = Right(i);
            int largest;

            if (l < Size && Compare(A[l], A[i]))
                largest = l;
            else
                largest = i;

            if (r < Size && Compare(A[r], A[largest]))
                largest = r;

            if (largest != i) {
                Swap(i, largest);
                Heapify(largest);
            }
        }

        //Increases the value of index i to value key
        //and returns the elements new index
        protected int IncreaseKey(int i, T key) {
            A[i] = key;

            while (i > 0 && Compare(A[i], A[Parent(i)])) {
                Swap(Parent(i), i);
                i = Parent(i);
            }

            return i;
        }

        //Decreases the value of index i to value key
        //and returns the elements new index
        protected int DecreaseKey(int i, T key) {
            A[i] = key;

            int r = Right(i);
            int l = Left(i);

            while (((l < Size) && (Compare(A[l], A[i])) || ((r < Size) && Compare(A[r], A[i])))) {
                int largest;
                if ((r < Size) && Compare(A[r], A[l]))
                    largest = r;
                else
                    largest = l;
                Swap(i, largest);
                i = largest;
                r = Right(i);
                l = Left(i);
            }

            return i;
        }

        //swaps element on index i and element on index j
        protected virtual void Swap(int i, int j) {
            T temp = A[i];
            A[i] = A[j];
            A[j] = temp;
        }

        private int Parent(int i) {
            return (int)((i + 1) / 2) - 1;
        }

        private int Left(int i) {
            return 2 * (i + 1) - 1;
        }

        private int Right(int i) {
            return 2 * (i + 1);
        }
    }
}
