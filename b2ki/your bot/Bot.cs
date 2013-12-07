using System;
using System.Collections.Generic;

namespace Ants {
	public abstract class Bot {

        //private List<Reservation> reservations;

        public Bot() {
            //reservations = new List<Reservation>();
        }

		public abstract void DoTurn(IGameState state);

        /*public void StartNewTurn() {
            reservations.Clear();
        }*/

		protected void IssueOrder(IGameState state, Ant ant, Direction direction) {
            if (direction != Direction.None) {
                //only walk if there's no other ant standing in that location
                if (state.GetIsUnoccupied(state.GetDestination(ant, direction))) {
                    ant.WaitTime = 0;
                    state.MoveAnt(ant, direction);
                    System.Console.Out.WriteLine("o {0} {1} {2}", ant.Row, ant.Col, direction.ToChar());
               /* } else {
                    reservations.Add(new Reservation(ant, direction));
                    ant.WaitTime++;*/
                }
            }
		}

        /*protected void IssueOrder(IGameState state, Reservation reservation) {
            //only walk if there's no other ant standing in that location
            if (reservation.Direction != Direction.None &&
                state.GetIsUnoccupied(state.GetDestination(reservation.Ant, reservation.Direction))) {
                    reservation.Ant.WaitTime = 0;
                    state.MoveAnt(reservation.Ant, reservation.Direction);
                    System.Console.Out.WriteLine("o {0} {1} {2}", reservation.Ant.Row, reservation.Ant.Col, reservation.Direction.ToChar());
            } 
        }

        protected void HandleReservations(IGameState state) {
            foreach (Reservation reservation in reservations) {
                IssueOrder(state, reservation);
            }
        }*/
	}

    /*public class Reservation {
        public Direction Direction { get; private set; }
        public Ant Ant { get; private set; }

        public Reservation(Ant ant, Direction direction) {
            this.Ant = ant;
            this.Direction = direction;
        }
    }*/
}