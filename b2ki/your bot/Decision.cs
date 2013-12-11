using System;
using System.Collections.Generic;

namespace Ants {
    public enum AntMode { None, Explore, Attack, Defend };

    class DecisionMaker {

        //The format in which the ants will be standing around the hill when defending.
        //The order in which the locations are listed corresponds with the order in which the
        //ants will stand around the hill.
        private static readonly Location[] defendFormat = { new Location(1, 1), new Location(-1, -1),
                                                            new Location(1, -1), new Location(-1, 1),
                                                            new Location(2, 1), new Location(-2, -1),
                                                            new Location(1, -2), new Location(-1, 2),
                                                            new Location(1, 2), new Location(-1, -2),
                                                            new Location(2, -1), new Location(-2, 1),
                                                            new Location(2, 2), new Location(-2, -2),
                                                            new Location(2, -2), new Location(-2, 2) };

        private List<Location> defendPositions;

        //constants that influence AntMode probabilities
        private const float K1 = 150;
        private const float K2 = 75;
        private const float K3 = 10;

        private const int exploreDistance = 30;

        private List<Location> attackTargets;
        private Location attackTarget;

        private List<Location> explorables;

        private List<Ant> defending;

        private Random random;

        private float px, py, pz;

        public DecisionMaker() {
            this.attackTargets = new List<Location>();
            this.explorables = new List<Location>();
            this.random = new Random();

            this.defending = new List<Ant>();
        }

        public void AddTarget(Location target) {
            this.attackTargets.Add(target);
        }


        /// <summary>
        /// Updates priority parameters and much other stuff. Should be called every new turn.
        /// </summary>
        public void Update(IGameState state) {

            //initialize defend positions the first time
            //(we do it here, because if we did it in a init method, then the MyHills List would be empty,
            //because your hills get added in the first turn)
            if (defendPositions == null) {
                defendPositions = new List<Location>();

                foreach (Location offset in defendFormat) {
                    foreach (AntHill hill in state.MyHills) {
                        Location location = state.GetDestination(hill, offset);
                        if (state.GetIsPassable(location)) {
                            defendPositions.Add(location);
                        }
                    }
                }
            }


            this.UpdateProbabilities(state);
            this.UpdateExplorables(state);
            this.UpdateAttackTargets(state);
            this.UpdateDefendings(state);
        }


        /// <summary>
        /// Update the probabilities px, py and pz, that describe the priorities of respectivly exploring, attacking and defending.
        /// </summary>
        /// <param name="state">The gamestate.</param>
        private void UpdateProbabilities(IGameState state) {
            //determine the amount of fog of war
            int fog = 0;
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    if (!state.VisibilityMap[row, col]) {
                        fog++;
                    }
                }
            }


            //the fraction of the map that is fog
            float f = (float)fog / (state.Width * state.Height);
            float x, y, z;

            //The importance of Exploring.
            //Depends on the amount of fog of war with respect to the map-size.
            x = K1 * f;

            //The importance of Attacking.
            //Depens on the amount of fog of war with respect to the map-size and the number of possible targets.
            y = K2 * (1 - f) * attackTargets.Count;

            //The importance of Defending.
            //Depends on the size of the ant population and the number of ants that are defending with respect
            //to the maximal number of ants that can defend.
            z = K3 * (int)(state.MyAnts.Count / 10) * (1 - (float)defending.Count / defendPositions.Count);


            //calculate probibilities of choosing a certain AntMode
            float sum = x + y + z;
            if (sum != 0) {
                this.px = x / sum;
                this.py = y / sum;
                this.pz = z / sum;
            } else {
                //if all priorities are 0 (very little change to happen, because then there ís no fog,
                //then just create more attacking ants (because exploring isn't needed)
                x = 0;
                y = 1;
                z = 0;
            }
        }


        /// <summary>
        /// Adds and removes attackTargets, and chooses a current attackTarget if needed.
        /// </summary>
        /// <param name="state">The gamestate.</param>
        private void UpdateAttackTargets(IGameState state) {
            //check for new enemyhills, if one exists: add it to the targets list
            foreach (AntHill hill in state.EnemyHills) {
                if (!attackTargets.Contains(hill)) {
                    attackTargets.Add(hill);
                }
            }

            //check whether target is reached, if so: remove it
            List<Location> toRemove = new List<Location>();
            foreach (Ant ant in state.MyAnts) {
                foreach (Location t in attackTargets) {
                    if (ant.Equals(t)) {
                        if (attackTargets.Contains(t)) {
                            toRemove.Add(t);
                        }
                        if (attackTarget != null && attackTarget.Equals(t)) {
                            attackTarget = null;
                        }
                    }
                }
            }
            foreach (Location loc in toRemove) {
                attackTargets.Remove(loc);

                foreach (Ant ant in state.MyAnts) {
                    if (loc.Equals(ant.Target)) {
                        ant.Target = null;
                    }
                }
            }

            //choose new target if no current target exists
            if (attackTarget == null && attackTargets.Count != 0) {
                attackTarget = attackTargets[0];
            }
        }


        /// <summary>
        /// Updates what ants are defending where
        /// </summary>
        /// <param name="state">The gamestate.</param>
        private void UpdateDefendings(IGameState state) {

            //remove dead ants from defendPositions
            foreach (Ant dead in state.MyDeads) {
                foreach (Location location in defendPositions) {
                    if (dead.Equals(location)) {
                        defending.Remove(dead);
                    }
                }
            }
            
            for (int i = 0; i < defending.Count; ++i) {
                defending[i].Target = defendPositions[i];
            }
        }


        /// <summary>
        /// Calculates all explorables.
        /// </summary>
        /// <param name="state">The gamestate.</param>
        private void UpdateExplorables(IGameState state) {
            //clear and recalculate the explorable tiles
            explorables.Clear();
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    Location location = new Location(row, col);
                    //an explorable location is unoccupied, not vissible, and not attackable in one turn.
                    if (state.GetIsUnoccupied(location) && !state.VisibilityMap[row, col] && !state.GetIsAttackable(location)) {
                        explorables.Add(location);
                    }
                }
            }
        }


        /// <summary>
        /// Sets the AntMode of <paramref name="ant"/>, according to the priorities of each task.
        /// </summary>
        /// <param name="ant">The ant the Mode has to be set for.</param>
        public void SetAntMode(Ant ant) {
            double num = random.NextDouble();
            if (num <= px) {
                ant.Mode = AntMode.Explore;
            } else if (num <= px + py) {
                ant.Mode = AntMode.Attack;
            } else {
                if (defending.Count < defendPositions.Count) {
                    defending.Add(ant);
                    ant.Mode = AntMode.Defend;
                }
            }
        }


        /// <summary>
        /// Sets the target of <paramref name="ant"/>, according to it's AntMode.
        /// </summary>
        /// <param name="ant">The ant the target has to be set for.</param>
        /// <param name="state">The gamestate.</param>
        public void SetTarget(Ant ant, IGameState state) {
            switch (ant.Mode) {
                case AntMode.Explore:
                    //choose random explore tile
                    ant.Target = GetExplorable(ant, state);
                    break;
                case AntMode.Attack:
                    //if there is no current attackTarget or the ant waits for to long: just send it away.
                    //we do this because if the ant is waiting for too long, we just want it to leave, and
                    //not block other ants.
                    if (attackTarget == null || ant.WaitTime > 2) {
                        ant.Target = GetExplorable(ant, state);
                    } else {
                        ant.Target = attackTarget;
                    }
                    break;
                case AntMode.Defend:
                    //choose a defend position
                    if (defending.Contains(ant)) {
                        int index = defending.IndexOf(ant);
                        ant.Target = defendPositions[index];
                    }
                    break;
                default:
                    //choose a random explore tile
                    ant.Target = GetExplorable(ant, state);
                    break;
            }
        }


        /// <summary>
        /// Gets a explorable location, preferably within exploreDistance.
        /// </summary>
        /// <param name="ant">The ant to search an explorable for.</param>
        /// <param name="state">The gamestate.</param>
        /// <returns>A location that is not visible, passible, and cannot be attacked in one turn if
        /// there exists one, <c>null</c> otherwist.</returns>
        public Location GetExplorable(Ant ant, IGameState state) {
            Location location = null;

            if (explorables.Count > 0) {
                //try 5 times to find an explorable location within distance exploreDistance
                for (int i = 0; i < 10; ++i) {
                    location = explorables[random.Next(explorables.Count)];
                    if (state.GetDistance(location, ant) <= exploreDistance) {
                        return location;
                    }
                }

                //if no explorable location is found within distance exploreDistance, return a random explorable
                return explorables[random.Next(explorables.Count)];
            }
            return location;
        }


        /// <summary>
        /// Updates the secondary target property of the ant.
        /// </summary>
        /// <param name="ant">The ant to be updated.</param>
        /// <param name="state">The gamestate.</param>
        public void UpdateTarget2(Ant ant, IGameState state) {
            
            //defending ants should never get food or hills.
            if (ant.Mode != AntMode.Defend) {
                //if no secondairy target
                if (ant.Target2 == null) {

                    //search for a hill to attack
                    ant.Target2 = GetTargetFromMap(ant, state.EnemyHills, state.ViewRadius2, state.ViewRadius, state);
                    if (ant.Target2 == null) {
                        //if there's no hill to attack in radius, search for food

                        //ants in attack mode should not get food that's very far away
                        int foodRadius = (ant.Mode == AntMode.Attack) ? 2 : state.ViewRadius;
                        int foodRadius2 = (ant.Mode == AntMode.Attack) ? 4 : state.ViewRadius2;

                        ant.Target2 = GetTargetFromMap(ant, state.FoodTiles, foodRadius2, foodRadius, state);
                    }
                } else {
                    //check whether the food/hill tile still exists, if not: drop the target
                    Tile targetType = state.GetTileType(ant.Target2);
                    if (targetType != Tile.Food && targetType != Tile.Hill) {
                        ant.Target2 = null;
                    }
                }
            }
        }


        /// <summary>
        /// Gets the closest target from the <paramref name="map"/> within a certain <paramref name="radius"/>
        /// </summary>
        /// <typeparam name="T">Used to make the function work for foodmaps, and hillmaps.</Location></typeparam>
        /// <param name="ant">The ant that looks for a target.</param>
        /// <param name="map">The map containing all possible targets.</param>
        /// <param name="radius">The search radius as int.</param>
        /// <param name="radius2">The sqaure of <paramref name="radius"/>. We need <paramref name="radius"/> and <paramref name="radius2"/>,
        /// because <paramref name="radius"/> is saved as int, so we can't just pass <paramref name="radius"/>, and we don't want
        /// to calculate the sqaure root of <paramref name="radius2"/> every time this method is called because it is an 
        /// expensive operation.</param>
        /// <param name="state">The gamestate.</param>
        /// <returns>The closest target for <paramref name="ant"/> if it exists, <c>null</c> otherwise.</returns>
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
    }
}
