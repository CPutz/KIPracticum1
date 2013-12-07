using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private List<Location> targets;
        private List<Location> explorables;

        private List<Ant> defending;

        private Location target;

        private Random random;

        private float px, py, pz;

        public DecisionMaker() {
            this.targets = new List<Location>();
            this.explorables = new List<Location>();
            this.random = new Random();

            this.defending = new List<Ant>();
        }

        public void AddTarget(Location target) {
            this.targets.Add(target);
        }


        /// <summary>
        /// Updates priority parameters and much other stuff. Should be called every new turn.
        /// </summary>
        public void Update(IGameState state) {

            //initialize defend positions the first time
            //(you do it here, because if we did it in a init method, then the MyHills List would be empty,
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


            //determine the amount of fog of war
            int fog = 0;
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    if (!state.VisibilityMap[row, col]) {
                        fog++;
                    }
                }
            }

            float fogFraction = (float)fog / (state.Width * state.Height);

            float x, y, z;

            //The importance of Exploring.
            //Depends on the amount of fog of war with respect to the map-size.
            x = K1 * fogFraction;

            //The importance of Attacking.
            //Depens on the amount of fog of war with respect to the map-size and the number of possible targets.
            y = K2 * (1 - fogFraction) * targets.Count;

            //The importance of Defending.
            //Depends on the size of the ant population and the number of ants that are defending with respect
            //to the maximal number of ants that can defend.
            z = K3 * (int)(state.MyAnts.Count / 10) * (1 - (float)defending.Count / defendPositions.Count);


            //x = 0;
            //y = 1;
            //z = 0;


            //calculate probibilities of choosing a certain AntMode
            float sum = x + y + z;
            if (sum != 0) {
                this.px = x / sum;
                this.py = y / sum;
                this.pz = z / sum;
            }



            //check for new enemyhills, if one exists: add it to the targets list
            foreach (AntHill hill in state.EnemyHills) {
                if (!targets.Contains(hill)) {
                    targets.Add(hill);
                }
            }

            //clear and recalculate the explorable tiles
            explorables.Clear();
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    Location location = new Location(row, col);
                    if (state.GetIsUnoccupied(location) && !state.VisibilityMap[row, col] && !state.GetIsAttackable(location)) {
                        explorables.Add(location);
                    }
                }
            }

            //check whether target is reached, if so: clear it
            List<Location> toRemove = new List<Location>();
            foreach (Ant ant in state.MyAnts) {
                foreach (Location t in targets) {
                    if (ant.Equals(t)) {
                        if (targets.Contains(t)) {
                            toRemove.Add(t);
                        }
                        if (target.Equals(t)) {
                            target = null;
                        }
                    }
                }
            }
            foreach (Location loc in toRemove) {
                targets.Remove(loc);
            }

            //choose new target if no current target exists
            if (target == null && targets.Count != 0) {
                target = targets[0];
            }

            this.UpdateDefendings(state);
        }


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
                    if (target != null) {
                        ant.Target = target;
                    } else {
                        ant.Target = GetExplorable(ant, state);
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
        /// Gets a explorable location.
        /// </summary>
        /// <returns>A location that is not visible, passible, and cannot be attacked in one turn if
        /// there exists one, <c>null</c> otherwist.</returns>
        public Location GetExplorable(Ant ant, IGameState state) {
            Location location = null;

            if (explorables.Count > 0) {
                for (int i = 0; i < 5; ++i) {
                    location = explorables[random.Next(explorables.Count)];
                    if (state.GetDistance(location, ant) <= 30) {
                        return location;
                    }
                }

                return explorables[random.Next(explorables.Count)];
            }
            return location;
        }
    }
}
