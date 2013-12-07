using System;
using System.Collections.Generic;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(IGameState state);

		protected void IssueOrder(IGameState state, Ant ant, Direction direction) {
            if (direction != Direction.None) {
                //only walk if there's no other ant standing in that location
                if (state.GetIsUnoccupied(state.GetDestination(ant, direction))) {
                    ant.WaitTime = 0;
                    //tell the gamestate that the ant will be moved next turn
                    state.MoveAnt(ant, direction);
                    System.Console.Out.WriteLine("o {0} {1} {2}", ant.Row, ant.Col, direction.ToChar());
                }
            }
		}
	}
}