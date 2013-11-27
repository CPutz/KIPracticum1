using System;
using System.Collections.Generic;

namespace Ants {

    /// <summary>
    /// Just a List for Locations that is extended with a boolean map for a fast Contains function.
    /// </summary>
    public class Map<T> : IEnumerable<T> where T : Location {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private bool[,] map;
        private List<T> items;

        public Map(int width, int height) : base() {
            this.Width = width;
            this.Height = height;

            this.map = new bool[Height, Width];
            this.items = new List<T>();
        }

        public bool Contains(Location loc) {
            return map[loc.Row, loc.Col];
        }

        public bool Contains(int row, int col) {
            return map[row, col];
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
}
