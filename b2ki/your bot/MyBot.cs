using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

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


        private AntMode GetAntMode(IGameState state) {
            float x, y, z;

            //extremely slow!
            int fog = 0;
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    if (!state.GetIsVisible(new Location(row, col))) {
                        fog++;
                    }
                }
            }
            float fogFraction = (float)fog / (state.Width * state.Height);

            x = 100 * fogFraction;

            y = 0; //depends on number of targethills and fogFraction

            z = (int)(state.MyAnts.Count / 10);

            float sum = x + y + z;

            float px, py, pz;
            px = x / sum;
            py = y / sum;
            //pz = z / sum;


            Random rand = new Random();
            double num = rand.NextDouble();
            if (num <= px)
                return AntMode.Explore;
            else if (num <= py)
                return AntMode.Attack;
            else
                return AntMode.Defend;
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