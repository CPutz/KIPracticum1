using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {

			// loop through all my ants and try to give them orders
			foreach (Ant ant in state.MyAnts) {

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
			}


            //Find path example
            /*Search s = new Search(state, state.GetDistance);
            List<Location> path = s.AStar(state.MyAnts[0], state.FoodTiles[0]);

            IssueOrder(state, state.MyAnts[0], new List<Direction>(state.GetDirections(path[0], path[1]))[0]);*/
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