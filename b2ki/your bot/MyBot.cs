using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker dicisionMaker;

        public MyBot() {
            this.dicisionMaker = new DecisionMaker();
        }


		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {


            List<Location> ExplorableTiles = new List<Location>();
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    Location location = new Location(row, col);
                    if (state.GetIsUnoccupied(location) && !state.VisibilityMap[row, col]) {
                        ExplorableTiles.Add(location);
                    }
                }
            }


            Random rand = new Random();
            Search s = new Search(state, state.GetDistance);

            foreach (Ant ant in state.MyAnts) {

                if (ant.Equals(ant.Target)) {
                    ant.Target = null;
                    ant.Route = null;
                }

                if (ant.Route == null || ant.IsWaitingFor > 3) {
                    if (ExplorableTiles.Count > 0)
                        ant.Target = ExplorableTiles[rand.Next(ExplorableTiles.Count)];
                    ant.Route = s.AStar(ant, ant.Target);
                }

                if ((ant.Route != null && !state.GetIsPassable(ant.Route[1]))) {
                    if (ExplorableTiles.Count > 0)
                        ant.Target = ExplorableTiles[rand.Next(ExplorableTiles.Count)];
                    ant.Route = s.AStar(ant, ant.Target);
                }

                //path = s.AStar(ant, ant.Target);
                if (ant.Route != null && ant.Route.Count > 1) {
                    if (!state.GetIsUnoccupied(ant.Route[1])) {
                        ant.IsWaitingFor++;
                    } else {
                        IssueOrder(state, ant, DirectionFromPath(ant.Route, state));
                        ant.Route.RemoveAt(0); //ghetto
                        ant.IsWaitingFor = 0;
                    }
                }

                //if (state.TimeRemaining < 10) 
                //    break;
            }


            //testing
            /*for (int i = 0; i < state.MyAnts.Count; ++i) {
                Ant ant = state.MyAnts[i];
                if (ant.AntNumber < state.FoodTiles.Count) {
                    ant.Target = state.FoodTiles[ant.AntNumber];
                    List<Location>  path = s.AStar(ant, ant.Target);
                    IssueOrder(state, ant, DirectionFromPath(path, state));
                } else {
                    if (state.GetIsUnoccupied(state.GetDestination(ant, Direction.East)))
                        IssueOrder(state, ant, Direction.East);
                }
            }*/
		}

        private Location GetFogTarget(IGameState state, Random rand) {
            Location location;

            //do {
                int row = rand.Next(0, state.Height);
                int col = rand.Next(0, state.Width);
                location = new Location(row, col);
            //} while (state.GetIsVisible(location));

            return location;
        }


        private Direction DirectionFromPath(List<Location> path, IGameState state) {
            if (path == null || path.Count <= 1)
                return Direction.None;
            else
                return new List<Direction>(state.GetDirections(path[0], path[1]))[0];
        }

		
		public static void Main (string[] args) {
/*#if DEBUG
            System.Diagnostics.Debugger.Launch();
            while (!System.Diagnostics.Debugger.IsAttached) { }
#endif*/

			new Ants().PlayGame(new MyBot());
		}

	}
	
}