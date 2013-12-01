using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants {
    public enum AntMode { None, Explore, Attack, Defend };

    class DecisionMaker {

        private const float k1 = 100;
        private const float k2 = 100;

        private List<Location> targets;
        private List<Location> explorableTiles;

        private List<Formation> formations;

        private Location target;

        private Random random;

        private float px, py, pz;

        public DecisionMaker() {
            this.targets = new List<Location>();
            this.explorableTiles = new List<Location>();
            this.random = new Random();
        }

        public void AddTarget(Location target) {
            this.targets.Add(target);
        }


        /// <summary>
        /// Updates priority parameters. Should be called every new turn.
        /// </summary>
        public void Update(IGameState state) {
            float x, y, z;

            int fog = 0;
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    if (!state.VisibilityMap[row, col]) {
                        fog++;
                    }
                }
            }

            float fogFraction = (float)fog / (state.Width * state.Height);

            x = k1 * fogFraction;

            y = k2 * (1 - fogFraction) * targets.Count;

            //z = (int)(state.MyAnts.Count / 10);
            z = 0;

            float sum = x + y + z;

            this.px = x / sum;
            this.py = y / sum;
            this.pz = z / sum;

            //check for new enemyhills
            foreach (AntHill hill in state.EnemyHills) {
                if (!targets.Contains(hill)) {
                    targets.Add(hill);
                }
            }

            //calculate the explorable tiles
            for (int row = 0; row < state.Height; ++row) {
                for (int col = 0; col < state.Width; ++col) {
                    Location location = new Location(row, col);
                    if (state.GetIsUnoccupied(location) && !state.VisibilityMap[row, col] && !state.GetIsAttackable(location)) {
                        explorableTiles.Add(location);
                    }
                }
            }

            //check whether target is reached
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

            //choose new target
            if (target == null && targets.Count != 0) {
                target = targets[0];
            }
        }


        public AntMode GetAntMode() {
            double num = random.NextDouble();
            if (num <= px)
                return AntMode.Explore;
            else if (num <= px + py)
                return AntMode.Attack;
            else
                return AntMode.Defend;
        }


        public Location GetTarget(Ant ant) {
            switch (ant.Mode) {
                case AntMode.Explore:
                    if (explorableTiles.Count > 0) {
                        return explorableTiles[random.Next(explorableTiles.Count)];
                    }
                    break;
                case AntMode.Attack:
                    if (target != null) {
                        return target;
                    } else if (explorableTiles.Count > 0) {
                        return explorableTiles[random.Next(explorableTiles.Count)];
                    }
                    break;
            }

            return ant;
        }
    }
}
