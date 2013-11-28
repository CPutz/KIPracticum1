using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker decision;

        public MyBot() {
            this.decision = new DecisionMaker();
        }


		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {
            this.decision.Update(state);

            
            Search s = new Search(state, state.GetDistance);

            foreach (Ant ant in state.MyAnts) {
                ant.IsWaitingFor++;

                if (ant.Col == 0 || ant.Col == 71 || ant.Col == 70) {

                }


                if (ant.Mode == AntMode.None) {
                    ant.Mode = this.decision.GetAntMode();
                }

                bool getFood = false;
                bool getHill = false;

                int distance = int.MaxValue;
                Location location = ant;

                int foodRadius2;
                int foodRadius;

                if (ant.Mode == AntMode.Attack) {
                    foodRadius2 = state.ViewRadius2 / 4;
                    foodRadius = state.ViewRadius / 2;
                } else {
                    foodRadius2 = state.ViewRadius2;
                    foodRadius = state.ViewRadius;
                }

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
                    if (state.GetIsUnoccupied(location)) {
                        List<Location> path = s.AStar(ant, location);
                        IssueOrder(state, ant, DirectionFromPath(path, state));
                    } else {
                        getFood = false;
                        getHill = false;
                    }
                } else if (!getHill && !getFood) {

                    if (ant.Equals(ant.Target)) {
                        ant.Target = null;
                        ant.Route = null;
                    }

                    if (ant.Route == null || ant.Target == null || ant.IsWaitingFor > 1) {
                        ant.Target = decision.GetTarget(ant);
                        ant.Route = s.AStar(ant, ant.Target);
                    }

                    //IDEA: check whether route is ok for next k locations (k=10 for example).
                    if ((ant.Route != null && ant.Route.Count > 1 && !state.GetIsPassable(ant.Route[1]))) {
                        ant.Target = decision.GetTarget(ant);
                        ant.Route = s.AStar(ant, ant.Target);
                    }

                    if (ant.Route != null && ant.Route.Count > 1) {

                        if (state.GetIsUnoccupied(ant.Route[1])) {
                            IssueOrder(state, ant, DirectionFromPath(ant.Route, state));
                            ant.Route.RemoveAt(0); //ghetto
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