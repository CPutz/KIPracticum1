using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker decision;
        //private List<Ant> onHill;

        public MyBot() : base() {
            this.decision = new DecisionMaker();
        }


		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {

            this.decision.Update(state);
            //onHill = new List<Ant>();

            Search search1 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsUnoccupied(location) && !state.GetIsAttackable(location); });
            Search search2 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsPassable(location) && !state.GetIsAttackable(location); });
            Search search3 = new Search(state, state.GetDistance, state.GetIsUnoccupied);
            Search search4 = new Search(state, state.GetDistance, state.GetIsPassable);


            foreach (Formation formation in decision.Formations) {

                if (!formation.IsForming) {
                    Ant leader = formation.Leader;

                    if (leader != null) {
                        if (leader.Route == null) {
                            leader.Route = search4.AStar(leader, leader.Target);
                        }
                        if (leader.Route != null) {
                            //then the leader can walk
                            if (leader.Route.Count > 1) {

                                Direction direction = DirectionFromPath(leader.Route, state);

                                if (state.GetIsUnoccupied(leader.Route[1])) {
                                    IssueOrder(state, leader, direction);
                                    leader.Route.RemoveAt(0); //ghetto

                                    Ant last = null;
                                    foreach (Ant ant in formation) {
                                        if (!ant.Equals(leader)) {
                                            Location nextTurnLocation = state.GetNextTurnLocation(last);
                                            Location location = state.GetDestination(nextTurnLocation, formation.Orientation);
                                            List<Location> path = search3.AStar(ant, location, 2);
                                            if (path == null) {
                                                path = search4.AStar(ant, nextTurnLocation, 2);
                                            }
                                            IssueOrder(state, ant, DirectionFromPath(path, state));
                                        }
                                        last = ant;
                                    }
                                } else {
                                    //if the formation is travalling in the same direction as it's orientation, switch leaders
                                    //because otherwise, the leader will always walk into the formation, forming problems
                                    if (formation.Orientation == direction) {
                                        formation.Reverse();

                                        //change orientation because the formation is flipped
                                        switch (formation.Orientation) {
                                            case Direction.North:
                                                formation.Orientation = Direction.South;
                                                break;
                                            case Direction.South:
                                                formation.Orientation = Direction.North;
                                                break;
                                            case Direction.East:
                                                formation.Orientation = Direction.West;
                                                break;
                                            case Direction.West:
                                                formation.Orientation = Direction.East;
                                                break;
                                        }
                                    }
                                }
                            } 
                        }
                    }
                } else {
                    foreach (Ant ant in formation) {
                        DoStuffToAnt(ant, state, search1, search2, search3);
                    }
                }

                if (state.TimeRemaining < 10)
                    break;
            }

            foreach (Ant ant in state.MyAnts) {

                if (ant.Formation == null) {
                    DoStuffToAnt(ant, state, search1, search2, search3);
                }

                if (state.TimeRemaining < 10) 
                    break;
            }

            if (state.TimeRemaining >= 10) {
                HandleReservations(state);
            }

            /*foreach (Ant ant in onHill) {
                bool b = false;
                foreach (AntHill hill in state.MyHills) {
                    if (state.GetNextTurnLocation(ant).Equals(hill)) {
                        b = true;
                        break;
                    }
                }

                //ant stands on a hill
                if (b) {
                    foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
                        if (direction != Direction.None) {
                            Location location = state.GetDestination(ant, direction);
                            if (state.GetIsUnoccupied(location)) {
                                IssueOrder(state, ant, direction);
                                break;
                            }
                        }
                    }
                }
            }*/
		}


        private void DoStuffToAnt(Ant ant, IGameState state, Search search1, Search search2, Search search3) {
            ant.WaitTime++;

            /*foreach (AntHill hill in state.MyHills) {
                if (ant.Equals(hill)) {
                    onHill.Add(ant);
                }
            }*/

            if (ant.Mode == AntMode.None) {
                this.decision.SetAntMode(ant);
            }


            //defending ants should never get food or hills.
            if (ant.Mode != AntMode.Defend) {
                if (ant.Target2 == null) {
                    int foodRadius2;
                    int foodRadius;

                    if (ant.Mode == AntMode.Attack) {
                        foodRadius = 2;
                        foodRadius2 = 4;
                    } else {
                        foodRadius = 5;
                        foodRadius2 = 25;
                    }

                    Location foodLocation = GetTargetFromMap(ant, state.FoodTiles, foodRadius2, foodRadius, state);
                    Location hillLocation = GetTargetFromMap(ant, state.EnemyHills, state.ViewRadius2, state.ViewRadius, state);

                    if (hillLocation != null) {
                        ant.Target2 = hillLocation;
                    } else {
                        ant.Target2 = foodLocation;
                    }
                }

                //an ant should not get food if it is in a formation that is not forming,
                //because then the formation will go out of formation
                /*if (!ant.Formation.IsForming) {
                    ant.Target2 = null;
                }*/

                if (ant.Target2 != null) {

                    if (ant.Route2 == null) {
                        //only get food if youre not gonna die for it, and if no other ant is in the way.
                        //and there exists a path to the food/hill which distance is less/eq to 1.5 times 
                        //the distance between the ant and the food/hill.
                        ant.Route2 = search1.AStar(ant, ant.Target2, (int)(state.GetDistance(ant, ant.Target2) * 1.5));

                        //remove the last node if its food because we don't need to stand on it to get it.
                        if (ant.Route2 != null && state.FoodTiles.Contains(ant.Target2)) {
                            ant.Route2.RemoveAt(ant.Route2.Count - 1);
                        }
                    }

                    //if the route2 is null after recalculation, drop the target
                    if (ant.Route2 == null) {
                        ant.Target2 = null;
                    } else {
                        //only get food if no other ant is blocking it
                        if (ant.Route2.Count > 1 && state.GetIsUnoccupied(ant.Route2[1])) {
                            IssueOrder(state, ant, DirectionFromPath(ant.Route2, state));
                            ant.Route2.RemoveAt(0); //ghetto
                        } else {
                            ant.Target2 = null;
                            ant.Route2 = null;
                        }
                    }
                }
            }


            if (ant.Target2 == null) {

                if (ant.Equals(ant.Target)) {
                    ant.Target = null;
                    ant.Route = null;
                }

                //if an ant in not in attack mode, avoid getting killed by calculating a better route
                if (ant.Route != null && ant.Route.Count > 1) {
                    if (state.GetIsAttackable(ant.Route[1]) && ant.Mode != AntMode.Attack) {
                        ant.Route = null;
                    }
                }

                //if an ant has no target or waited for too long, get a new target (unless the ant is in a formation that is not forming)
                if (ant.Target == null || ant.WaitTime > 2) {
                    decision.SetTarget(ant, state);
                    ant.Route = null;
                }

                //if an ant has no route or the current route is not passable (crosses water), recalculate it
                if (ant.Route == null || ant.Route.Count > 1 && !state.GetIsPassable(ant.Route[1])) {

                    int distance = state.GetDistance(ant, ant.Target);

                    if (ant.Mode == AntMode.Attack) {
                        //calculate route using search1, if it fails, try search3
                        ant.Route = search1.AStar(ant, ant.Target, distance * 2);
                        if (ant.Route == null) {
                            ant.Route = search3.AStar(ant, ant.Target, distance * 2);
                        }
                    } else {
                        //calculate route using search1, if it fails, try search2
                        ant.Route = search1.AStar(ant, ant.Target, distance * 2);
                        if (ant.Route == null) {
                            ant.Route = search2.AStar(ant, ant.Target, distance * 2);
                        }
                    }
                }

                //if the route is valid, 
                if (ant.Route != null && ant.Route.Count > 1) {
                    IssueOrder(state, ant, DirectionFromPath(ant.Route, state));
                    ant.Route.RemoveAt(0); //ghetto
                }
            }
        }


        private Location GetTargetFromMap<T>(Ant ant, Map<T> map, int radius2, int radius, IGameState state) where T : Location {
            int distance = int.MaxValue;
            Location location = null;

            for (int r = -1 * radius; r <= radius; ++r) {
                for (int c = -1 * radius; c <= radius; ++c) {
                    int square = r * r + c * c;
                    if (square <= radius2) {
                        Location loc = state.GetDestination(ant, new Location(r, c));

                        if (map.Contains(loc)) {
                            int tempDistance = state.GetDistance(ant, loc);
                            if (tempDistance < distance) {
                                location = loc;
                            }
                        }
                    }
                }
            }

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