using System;

namespace Ants {
	public abstract class Bot {

		public abstract void DoTurn(IGameState state);

		protected void IssueOrder(IGameState state, Ant ant, Direction direction) {
            if (direction != Direction.None) {
                ant.IsWaitingFor = 0;
                state.MoveAnt(ant, direction);
                System.Console.Out.WriteLine("o {0} {1} {2}", ant.Row, ant.Col, direction.ToChar());
            }
		}
	}
}