using System;
using System.Collections.Generic;

namespace Ants {

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



    class LinkedList<T> : IEnumerable<T>
    {
        private LLElement<T> head;
        private LLElement<T> last;

        public int Size { get; private set; }

        public LinkedList()
        {
            this.Size = 0;
        }

        public void Add(T key)
        {
            LLElement<T> k = new LLElement<T>(key);
            if (this.head != null)
                this.last.Next = k;
            else
                this.head = k;
            this.last = k;

            this.Size++;
        }

        /*public bool Remove(T key) {
            LLElement<T> x = head;
            LLElement<T> prev = null;

            while (x != null) {
                if (x.Value.Equals(key)) {
                    if (prev != null)
                        prev.Next = x.Next;
                    else
                        head = x.Next;

                    return true;
                }

                prev = x;
                x = x.Next;
            }

            return false;
        }*/

        public T Search(T key)
        {
            LLElement<T> x = this.head;

            while (x != null)
            {
                if (x.Value.Equals(key))
                    return x.Value;

                x = x.Next;
            }

            return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            LLElement<T> x = this.head;

            while (x != null)
            {
                yield return x.Value;
                x = x.Next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class LLElement<T> {
        private LLElement<T> next;
        private T val;

        public LLElement(T key) {
            val = key;
        }

        public LLElement<T> Next {
            get { return next; }
            set { next = value; }
        }

        public T Value {
            get { return val; }
        }
    }
}
