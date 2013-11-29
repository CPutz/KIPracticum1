using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker decision;

        public MyBot() {
            this.decision = new DecisionMaker();
        }


        //Formation formation;



		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {
            this.decision.Update(state);

            
            Search s = new Search(state, state.GetDistance);

            /*foreach (Location loc in state.MyDeads) {
                Ant ant = new Ant(loc.Row, loc.Col, 0, 0); //ghetto!!!
                if (formation.Contains(ant)) {
                    formation.Remove(ant);
                }
            }

            if (formation == null) {
                formation = new Formation();
                formation.Orientation = Direction.North;
            }

            Ant leader = formation.Leader;
            if (leader != null) {
                leader.Target = new Location(15, 0);

                if (formation.Size == 5 && formation.InFormation(state)) {
                    Location loc = state.GetDestination(leader, Direction.South);
                    if (state.GetIsUnoccupied(loc)) {
                        IssueOrder(state, leader, Direction.South);
                    }
                } else {
                    List<Location> path2 = s.AStar(leader, leader.Target);
                    if (path2 != null && path2.Count > 1 && state.GetIsUnoccupied(path2[1])) {
                        IssueOrder(state, leader, DirectionFromPath(path2, state));
                    }
                }
            }

            Ant prev = null;
            foreach (Ant ant in formation) {
                if (ant != leader) {
                    ant.Target = state.GetDestination(prev.Target, formation.Orientation);
                    List<Location> path = s.AStar(ant, ant.Target);
                    if (path != null && path.Count > 1 && state.GetIsUnoccupied(path[1])) {
                        IssueOrder(state, ant, DirectionFromPath(path, state));
                    }
                }
                prev = ant;
            }*/
            



            foreach (Ant ant in state.MyAnts) {
                ant.IsWaitingFor++;

                if (ant.Mode == AntMode.None) {
                    ant.Mode = this.decision.GetAntMode();
                }

               /* if (ant.Mode == AntMode.Attack) {
                    if (!formation.Contains(ant) && formation.Size < 5) {
                        formation.Add(ant);
                    } else {
                        IssueOrder(state, ant, Direction.South);
                    }
                }

                if (ant.Mode != AntMode.Attack) {*/



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

                        if (ant.Target == null || ant.Route == null || ant.IsWaitingFor > 1) {
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
                //}

                //if (state.TimeRemaining < 10) 
                //    break;
            }
		}


        private Direction DirectionFromPath(List<Location> path, IGameState state) {
            if (path == null || path.Count <= 1)
                return Direction.None;
            else
                return new List<Direction>(state.GetDirections(path[0], path[1]))[0];
        }

		
		public static void Main (string[] args) {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
            while (!System.Diagnostics.Debugger.IsAttached) { }
#endif

			new Ants().PlayGame(new MyBot());
		}

	}
	
}