using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants {
    class Formation : Ant, IEnumerable<Ant> {

        private LinkedList<Ant> ants;

        public Direction Orientation { get; private set; }
        public int Size { get { return ants.Size; } }

        public Formation(Ant leader)
            : base(leader.Row, leader.Col, leader.Team, leader.AntNumber) {

            this.ants = new LinkedList<Ant>();
            this.ants.Add(leader);
            this.Orientation = Direction.East;
            leader.InFormation = true;
        }

        public void Add(Ant ant) {
            ants.Add(ant);
            ant.InFormation = true;
        }

        public bool Contains(Ant ant) {
            return (ants.Search(ant) != null);
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
