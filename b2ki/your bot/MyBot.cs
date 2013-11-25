using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        Formation f;
        Ant leader;
        bool isMade = false;

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {

			// loop through all my ants and try to give them orders
			/*foreach (Ant ant in state.MyAnts) {

				// try all the directions
				foreach (Direction direction in Ants.Aim.Keys) {

					// GetDestination will wrap around the map properly
					// and give us a new location
					Location newLoc = state.GetDestination(ant, direction);

					// GetIsPassable returns true if the location is land
					if (state.GetIsPassable(newLoc)) {
                        IssueOrder(state, ant, direction);
						// stop now, don't give 1 and multiple orders
						break;
					}
				}
				
				// check if we have time left to calculate more orders
				if (state.TimeRemaining < 10) break;
			}*/

            if (state.MyAnts.Count == 4) {
                int test = 2;
                test *= 4;
            }

            if (!isMade) {
                leader = state.MyAnts[0];
                f = new Formation(leader);
                isMade = true;
            }

            foreach (Ant ant in state.MyAnts) {
                /*if (!f.Contains(ant)) {
                    f.Add(ant);
                }*/
                if (!ant.InFormation) {
                    f.Add(ant);
                }
            }

            Search s = new Search(state, state.GetDistance);

            if (state.FoodTiles.Count > 0) {
                List<Location> path = s.AStar(leader, state.FoodTiles[0]);
                IssueOrder(state, leader, DirectionFromPath(path, state));
            }

            Ant last = leader;
            foreach (Ant ant in f) {
                if (ant != leader) {
                    List<Location> path = s.AStar(ant, state.GetDestination(last, f.Orientation));
                    IssueOrder(state, ant, DirectionFromPath(path, state));
                }
                last = ant;
            }


            //Find path example
            /*Search s = new Search(state, state.GetDistance);
            List<Location> path = s.AStar(state.MyAnts[0], state.FoodTiles[0]);

            IssueOrder(state, state.MyAnts[0], new List<Direction>(state.GetDirections(path[0], path[1]))[0]);*/
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