using System;

namespace Ants {
    class Formation {

        public Direction Orientation { get; private set; }

        private LinkedList<Ant> ants;

        public Formation() { }

        public void Add(Ant ant) {
            ants.Add(ant);
        }

        public void Remove(Ant ant) {
            ants.Remove(ant);
        }

        public int Size() {
            return ants.Size;
        }
    }
}
