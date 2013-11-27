using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants {
    public enum AntMode { Explore, Attack, Defend };

    class DecisionMaker {

        private const float k1 = 100;
        private const float k2 = 100;

        //private IGameState state;
        private List<Location> targets;

        private float px, py, pz;

        public DecisionMaker() {
            this.targets = new List<Location>();
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

            z = (int)(state.MyAnts.Count / 10);

            float sum = x + y + z;

            this.px = x / sum;
            this.py = y / sum;
            this.pz = z / sum;
        }


        public AntMode GetAntMode() {
            Random rand = new Random();
            double num = rand.NextDouble();
            if (num <= px)
                return AntMode.Explore;
            else if (num <= py)
                return AntMode.Attack;
            else
                return AntMode.Defend;
        }
    }
}
