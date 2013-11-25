using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {


            List<Location> ExplorableTiles = new List<Location>();
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    Location location = new Location(row, col);
                    if (state.GetIsPassable(location)) { //&& !state.VisibilityMap[row, col]) {
                        ExplorableTiles.Add(location);
                    }
                }
            }


            Random rand = new Random();
            Search s = new Search(state, state.GetDistance);

            foreach (Ant ant in state.MyAnts) {

                List<Location> path;

                while ((path = s.AStar(ant, ant.Target)) == null || path.Count == 1) {
                //if (ant.Target == null) {
                    ant.Target = ExplorableTiles[rand.Next(0, ExplorableTiles.Count)];
                }
                //path = s.AStar(ant, ant.Target);

                IssueOrder(state, ant, DirectionFromPath(path, state));

                if (state.TimeRemaining < 1000) break;
            }
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