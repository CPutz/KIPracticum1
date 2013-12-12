using System;
using System.Collections.Generic;

namespace Ants {

	class MyBot : Bot {

        private DecisionMaker decision;

        public MyBot() : base() {
            this.decision = new DecisionMaker();
        }


		// DoTurn is run once per turn
		public override void DoTurn (IGameState state) {

            this.decision.Update(state);

            //searches a path that cannot be attacked in one turn, and that does look whether another ant is in the way.
            Search search1 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsUnoccupied(location) && !state.GetIsAttackable(location); });

            //searches a path that cannot be attacked in one turn, but that does not look whether another ant is in the way
            //so the path is planned like all friendly ants do not exist.
            Search search2 = new Search(state, state.GetDistance,
                (Location location) => { return state.GetIsPassable(location) && !state.GetIsAttackable(location); });

            //search for a path that takes in account where other friendly ants are, but does not look at enemy ants.
            Search search3 = new Search(state, state.GetDistance, state.GetIsPassable);
            

            foreach (Ant ant in state.MyAnts) {

                //check whether a defend ant is blocking hill because it cannot go to its defend position.
                if (ant.Mode == AntMode.Defend && ant.WaitTime > 5) {
                    foreach (AntHill hill in state.MyHills) {
                        if (ant.Equals(hill)) {
                            decision.ReportInvalidDefendPosition(ant.Target);
                            
                            //reset ant
                            ant.Target = null;
                            ant.Route = null;
                            ant.Mode = AntMode.None;
                        }
                    }
                }

                ant.WaitTime++;

                if (ant.Mode == AntMode.None) {
                    this.decision.SetAntMode(ant);
                }


                //updates the secondary target of the ant, so gets hill or food target when needed
                decision.UpdateTarget2(ant, state);


                if (ant.Target2 != null) {

                    if (ant.Route2 == null) {
                        //only get food if youre not gonna die for it, and if no other ant is in the way (use search1).
                        //and there exists a path to the food/hill which distance is less than 1.5 times 
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
                            IssueOrder(state, ant, GetDirectionFromPath(ant.Route2, state));
                            ant.Route2.RemoveAt(0);

                                //original route has become invalid
                                ant.Route = null;
                        } else {
                            ant.Target2 = null;
                            ant.Route2 = null;
                        }
                    }
                }
                


                //if there exists no secundairy target, then get the primary target.
                //it is possible that there exists no target, and then no target is found.
                //therefore it we can't use an else construction but should again check if (ant.Target2 == null.)
                if (ant.Target2 == null) {

                    //if we reached our target, drop it.
                    if (ant.Equals(ant.Target)) {
                        ant.Target = null;
                        ant.Route = null;
                    }

                    //if an ant in not in attack mode, avoid getting killed by not taking this route.
                    //we need to check this each turn because our routes are calculated only one time
                    //and enemy ants may be moved.
                    if (ant.Route != null && ant.Route.Count > 1) {
                        if (state.GetIsAttackable(ant.Route[1]) && ant.Mode != AntMode.Attack) {
                            ant.Route = null;
                        }
                    }

                    //if an ant has no target or waited for too long, get a new target
                    if (ant.Target == null || ant.WaitTime > 2) {
                        decision.SetTarget(ant, state);
                        ant.Route = null;
                    }

                    //if an ant has no route or the current route is not passable (crosses water), recalculate it.
                    //we have to check whether a route is passable, because at the beginning of the match, we don't 
                    //know the map looks like. we could plan a route that later turns out to cross water.
                    if (ant.Target != null && (ant.Route == null || ant.Route.Count > 1 && !state.GetIsPassable(ant.Route[1]))) {

                        int distance = state.GetDistance(ant, ant.Target);

                        if (ant.Mode == AntMode.Attack) {
                            //calculate route using search1, if it fails, try search3
                            ant.Route = search1.AStar(ant, ant.Target, distance * 2);
                            if (ant.Route == null) {
                                ant.Route = search3.AStar(ant, ant.Target, distance * 2);
                            }
                        } else {
                            //calculate route using search1, if it fails, try search2
                            ant.Route = search1.AStar(ant, ant.Target, distance * 3);
                            if (ant.Route == null) {
                                ant.Route = search2.AStar(ant, ant.Target, distance * 3);
                            }
                        }
                    }

                    //if we have a valid route, and there is no ant in the way, then issueorder.
                    if (ant.Route != null) {
                        if (ant.Route.Count > 1 && state.GetIsUnoccupied(ant.Route[1])) {
                            IssueOrder(state, ant, GetDirectionFromPath(ant.Route, state));
                            ant.Route.RemoveAt(0);
                        }
                    }
                }

                if (state.TimeRemaining < 20) 
                    break;
            }
		}

        private Direction GetDirectionFromPath(List<Location> path, IGameState state) {
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