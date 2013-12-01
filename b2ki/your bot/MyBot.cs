using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker decision;
        private List<Ant> onHill;

        public MyBot() : base() {
            this.decision = new DecisionMaker();
        }


        //Formation formation;


        //Ant onHill;
        //int time;


		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {

            this.decision.Update(state);
            onHill = new List<Ant>();

            Search search1 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsUnoccupied(location) && !state.GetIsAttackable(location); });
            Search search2 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsPassable(location) && !state.GetIsAttackable(location); });
            Search search3 = new Search(state, state.GetDistance, state.GetIsUnoccupied);
            Search search4 = new Search(state, state.GetDistance, state.GetIsPassable);

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

                foreach (AntHill hill in state.MyHills) {
                    if (ant.Equals(hill)) {
                        onHill.Add(ant);
                    }
                }

                if (ant.Mode == AntMode.None) {
                    ant.Mode = this.decision.GetAntMode();
                }


                /*if (ant.Equals(new Location(8, 16))) {
                    if (onHill == null || ant.AntNumber != onHill.AntNumber) {
                        onHill = ant;
                        time = 0;
                    } else {
                        time++;
                    }

                }*/


               /* if (ant.Mode == AntMode.Attack) {
                    if (!formation.Contains(ant) && formation.Size < 5) {
                        formation.Add(ant);
                    } else {
                        IssueOrder(state, ant, Direction.South);
                    }
                }

                if (ant.Mode != AntMode.Attack) {*/



                /*Location location = GetFoodHillTarget(ant, state);                

                if (location != null) {

                    //only get food if youre not gonna die for it, and if no other ant is in the way.
                    List<Location> path = search1.AStar(ant, location);

                    //only get food/hill when there exists a path to the food/hill which
                    //distance is less/eq to two times the distance between the ant and the food/hill
                    if (path != null && path.Count > 1 && path.Count <= state.GetDistance(ant, location) * 2) {
                        IssueOrder(state, ant, DirectionFromPath(path, state));
                        ant.Target2 = location;
                        path.RemoveAt(0); //ghetto
                        ant.Route2 = path;
                    } else {
                        location = null;
                    }
                }*/

                //ant.Target2 = GetFoodHillTarget(ant, state);

                if (ant.Target2 == null) {
                    int foodRadius2;
                    int foodRadius;

                    if (ant.Mode == AntMode.Attack) {
                        //foodRadius2 = state.ViewRadius2 / 4;
                        //foodRadius = state.ViewRadius / 2;
                        foodRadius = 2;
                        foodRadius2 = 4;
                    } else {
                        //foodRadius2 = state.ViewRadius2;
                        //foodRadius = state.ViewRadius;
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

                if (ant.Target2 != null) {
                    if (ant.Route2 == null) {

                        //only get food if youre not gonna die for it, and if no other ant is in the way.
                        ant.Route2 = search1.AStar(ant, ant.Target2);

                        //remove the last node if its food
                        if (ant.Route2 != null && state.FoodTiles.Contains(ant.Target2)) {
                            ant.Route2.RemoveAt(ant.Route2.Count - 1);
                        }

                        //only get food/hill when there exists a path to the food/hill which
                        //distance is less/eq to two times the distance between the ant and the food/hill
                        if (ant.Route2 != null && ant.Route2.Count > 1 && ant.Route2.Count > state.GetDistance(ant, ant.Target2) * 1.5) {
                            ant.Route2 = null;
                        }
                    }

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


                if (ant.Target2 == null) {

                    if (ant.Equals(ant.Target)) {
                        ant.Target = null;
                        ant.Route = null;
                    }

                    /*if (ant.Target != null && ant.Route != null && ant.Route.Count > 1) {
                        if (state.GetIsAttackable(ant.Route[1]) && ant.Mode != AntMode.Attack) {
                            ant.Route = null;
                        }
                    }*/

                    if (ant.Target == null || ant.IsWaitingFor > 10) {
                        ant.Target = decision.GetTarget(ant);
                        ant.Route = null;
                    }

                    if(ant.Route == null) {

                        //if route using search1 or search3 is null, then try search2 or search4
                        if (ant.Mode == AntMode.Attack) {
                            ant.Route = search3.AStar(ant, ant.Target);
                            /*if (ant.Route == null) {
                                ant.Route = search4.AStar(ant, ant.Target);
                            }*/
                        } else {
                            ant.Route = search1.AStar(ant, ant.Target);
                            /*if (ant.Route == null) {
                                ant.Route = search2.AStar(ant, ant.Target);
                            }
                            if (ant.Route == null) {
                                ant.Route = search3.AStar(ant, ant.Target);
                            }
                            if (ant.Route == null) {
                                ant.Route = search4.AStar(ant, ant.Target);
                            }*/
                        }
                    }


                    //IDEA: check whether route is ok for next k locations (k=10 for example).
                    if ((ant.Route != null && ant.Route.Count > 1 && !state.GetIsPassable(ant.Route[1]))) {
                        ant.Target = decision.GetTarget(ant);

                        //if route using search1 or search3 is null, then try search2 or search4
                        if (ant.Mode == AntMode.Attack) {
                            ant.Route = search3.AStar(ant, ant.Target);
                            /*if (ant.Route == null) {
                                ant.Route = search4.AStar(ant, ant.Target);
                            }*/
                        } else {
                            ant.Route = search1.AStar(ant, ant.Target);
                            /*if (ant.Route == null) {
                                ant.Route = search2.AStar(ant, ant.Target);
                            }*/
                        }
                    }

                    if (ant.Route != null && ant.Route.Count > 1) {
                        //if (state.GetIsUnoccupied(ant.Route[1])) {
                            IssueOrder(state, ant, DirectionFromPath(ant.Route, state));
                            ant.Route.RemoveAt(0); //ghetto
                        //}
                    }
                }
                //}



                if (state.TimeRemaining < 25) 
                    break;
            }

            HandleReservations(state);

            foreach (Ant ant in onHill) {
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

        /*private Location GetTargetFromMap(Ant ant, Map<Location> map, IGameState state) {
            int distance = int.MaxValue;
            Location location = null;

            bool getHill = false;

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
                        }
                    }
                }
            }

            return location;
        }*/


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