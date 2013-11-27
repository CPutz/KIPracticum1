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


                bool getFood = false;
                bool getHill = false;

                int distance = int.MaxValue;
                Location location = ant;

                int foodRadius2 = state.ViewRadius2;
                int foodRadius = state.ViewRadius;

                for (int r = -1 * foodRadius; r <= foodRadius; ++r) {
                    for (int c = -1 * foodRadius; c <= foodRadius; ++c) {
                        int square = r * r + c * c;
                        if (square <= foodRadius2) {
                            Location loc = state.GetDestination(ant, new Location(r, c));

                            if (state.EnemyHills.Contains(loc)) {
                                int tempDistance = state.GetDistance(ant, loc);
                                if (!getHill || tempDistance < distance) {
                                    location = loc;
                                }

                                getHill = true;

                            } else if (state.FoodTiles.Contains(loc) && !getHill) {
                                int tempDistance = state.GetDistance(ant, loc);

                                if (tempDistance < distance) {
                                    distance = tempDistance;
                                    location = loc;
                                }
                                getFood = true;
                            }
                        }
                    }
                }

                if (getFood || getHill) {
                    List<Location> path = s.AStar(ant, location);
                    IssueOrder(state, ant, DirectionFromPath(path, state));
                } else {
                    if (ant.Equals(ant.Target)) {
                        ant.Target = null;
                        ant.Route = null;
                    }

                    if (ant.Route == null || ant.IsWaitingFor > 3) {
                        if (ExplorableTiles.Count > 0)
                            ant.Target = ExplorableTiles[rand.Next(ExplorableTiles.Count)];
                        ant.Route = s.AStar(ant, ant.Target);
                    }

                    if ((ant.Route != null && ant.Route.Count > 1 && !state.GetIsPassable(ant.Route[1]))) {
                        if (ExplorableTiles.Count > 0)
                            ant.Target = ExplorableTiles[rand.Next(ExplorableTiles.Count)];
                        ant.Route = s.AStar(ant, ant.Target);
                    }

                    if (ant.Route != null && ant.Route.Count > 1) {
                        if (!state.GetIsUnoccupied(ant.Route[1])) {
                            ant.IsWaitingFor++;
                        } else {



                            //check whether ant is too close to enemy
                            bool tooClose = false;
                            bool retreat = false;
                            /*int closeRadius2 = (state.AttackRadius + 2) * (state.AttackRadius + 2);
                            int retreatRadius2 = (state.AttackRadius + 1) * (state.AttackRadius + 1);
                            int closeRadius = (int)Math.Sqrt(closeRadius2);
                            Direction retreatDirection = 0;

                            for (int r = -1 * closeRadius; r <= closeRadius; ++r) {
                                for (int c = -1 * closeRadius; c <= closeRadius; ++c) {
                                    int square = r * r + c * c;
                                    if (square <= closeRadius2) {
                                        Location loc = state.GetDestination(ant.Route[1], new Location(r, c));

                                        //add visible locations to visibilitymap
                                        if (state.EnemyMap[loc.Row, loc.Col]) {
                                            tooClose = true;

                                            if (square <= retreatRadius2) {
                                                retreat = true;
                                                retreatDirection = new List<Direction>(state.GetDirections(ant, loc))[0];
                                            }
                                        }
                                    }
                                }
                            }*/

                            IssueOrder(state, ant, DirectionFromPath(ant.Route, state));
                            ant.Route.RemoveAt(0); //ghetto
                            ant.IsWaitingFor = 0;

                            /*else if (retreat) {
                                if (state.GetIsUnoccupied(state.GetDestination(ant, retreatDirection))) {
                                    IssueOrder(state, ant, retreatDirection);
                                }
                            }*/
                        }
                    }
                }

                //if (state.TimeRemaining < 10) 
                //    break;
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

		
		public static void Main (string[] args) {
/*#if DEBUG
            System.Diagnostics.Debugger.Launch();
            while (!System.Diagnostics.Debugger.IsAttached) { }
#endif*/

			new Ants().PlayGame(new MyBot());
		}

	}
	
}