using System;
using System.Collections.Generic;

namespace Ants {
    class Formation : IEnumerable<Ant> {

        public Direction Orientation { get; set; }
        public Ant Leader { get { return this.ants.First; } }
        public int Size { get { return this.ants.Size; } }

        private LinkedList<Ant> ants;


        public Formation() {
            this.ants = new LinkedList<Ant>();
        }

        public void Add(Ant ant) {
            this.ants.Add(ant);
        }

        public void Remove(Ant ant) {
            this.ants.Remove(ant);
        }

        public bool Contains(Ant ant) {
            return (this.ants.Search(ant) != null);
        }

        //returns whether the group is currently in formation
        public bool InFormation(IGameState state) {
            Ant prev = null;
            foreach (Ant ant in ants) {
                if (ant != this.Leader) {
                    if (!state.GetDestination(prev, this.Orientation).Equals(ant)) {
                        return false;
                    }
                }
                prev = ant;
            }

            return true;
        }

        public IEnumerator<Ant> GetEnumerator() {
            foreach (Ant ant in ants) {
                if (ant == null) {
                    break;
                }
                yield return ant;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
