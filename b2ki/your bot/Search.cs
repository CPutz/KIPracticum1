using System;
using System.Collections.Generic;

namespace Ants {

    class Search {
        public delegate int Heuristic(Location A, Location B);
        public delegate bool Passible(Location A);

        public int Width { get; private set; }
        public int Height { get; private set; }

        private IGameState gameState;
        private Heuristic heuristic;
        private Passible isPassible;
        

        public Search(IGameState gameState, Heuristic heuristic, Passible isPassible) {
            this.gameState = gameState;
            this.Width = gameState.Width;
            this.Height = gameState.Height;
            this.heuristic = heuristic;
            this.isPassible = isPassible;
        }

        /// <summary>
        /// Searches for a path from <paramref name="source"/> to <paramref name="destination"/> using 
        /// the A* algorithm with <paramref name="heuristic"/> as the heuristic function.
        /// </summary>
        /// <param name="source">The start location.</param>
        /// <param name="destination">The destination location.</param>
        /// <returns>A Path from src to destination if it exists, otherwise <c>null</c></returns>
        public List<Location> AStar(Location source, Location destination) {

            if (source != null && destination != null) {

                AStarOpenSet openSet = new AStarOpenSet(this.Width, this.Height);
                AStarNode<Location>[,] closedSet = new AStarNode<Location>[this.Height, this.Width];

                AStarNode<Location> current = new AStarNode<Location>(source);
                current.G = 0;
                current.F = heuristic(source, destination);

                openSet.Insert(current);

                while (openSet.Size > 0) {
                    //Get node with lowest cost+heuristic and remove it from openSet.
                    current = openSet.ExtractMax();

                    //Best path found if destination is removed from the openSet.
                    if (current.Object.Equals(destination)) {
                        return GetPath(current);
                    }

                    //Node is closed when it is pulled from the openSet.
                    closedSet[current.Object.Row, current.Object.Col] = current;

                    foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
                        if (direction != Direction.None) {
                            
                            Location neighbour = gameState.GetDestination(current.Object, direction);

                            int G = current.G + 1; //distance between nodes is always 1
                            int F = G + heuristic(neighbour, destination);

                            //If neighbour in closedSet and F is worse than neighbour.F, or neighbour is not a passable block, then go to next neighbour.
                            if ((closedSet[neighbour.Row, neighbour.Col] != null && F >= closedSet[neighbour.Row, neighbour.Col].F) ||
                                !isPassible(neighbour))
                                continue;

                            bool notInOpen = !openSet.Contains(neighbour);
      
                            //If neighbour not in open (so never encountered or in closed), and if it's in closed, then if F is better
                            //than neighbour.F, then add neighbour to openSet, or change it's value in closedSet.
                            if (notInOpen || (closedSet[neighbour.Row, neighbour.Col] != null && F < closedSet[neighbour.Row, neighbour.Col].F)) {
                                AStarNode<Location> node = new AStarNode<Location>(neighbour);
                                node.G = G;
                                node.F = F;
                                node.cameFrom = current;

                                if (notInOpen) {
                                    openSet.Insert(node);
                                }
                                else {
                                    closedSet[neighbour.Row, neighbour.Col] = node;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines the path from current described by the parameter <paramref name="cameFrom"/> in the AStarWrapper. 
        /// </summary>
        /// <param name="current">The destionation location.</param>
        /// <returns>A path from a location to <paramref name="destination"/> described by <paramref name="cameFrom"/>.</returns>
        List<Location> GetPath(AStarNode<Location> current) {
            List<Location> path = new List<Location>();
            path.Add(current.Object);

            while (current.cameFrom != null) {
                current = current.cameFrom;
                path.Add(current.Object);
            }

            path.Reverse();
            return new List<Location>(path);
        }
    }

    class AStarNode<T> : IComparable {
        public readonly T Object;
        public int F { get; set; }
        public int G { get; set; }
        public AStarNode<T> cameFrom { get; set; }

        public AStarNode(T t) {
            this.Object = t;
        }

        int IComparable.CompareTo(object obj) {
            if (obj == null)
                return 1;

            AStarNode<T> t = obj as AStarNode<T>;
            if (t != null)
                return this.F - t.F;
            else
                throw new ArgumentException("Object is not a AStarNode<" + typeof(T).ToString() + ">");
        }
    }

    /// <summary>
    /// An AStarOpenSet contains is a queue (implemented as a heap), extended with a table
    /// that keeps track of which locations are in the set and which are not.
    /// </summary>
    class AStarOpenSet : MinHeap<AStarNode<Location>> {
        //gives whether a location (row, col) is in openSet.
        private bool[,] inOpen;

        public AStarOpenSet(int w, int h) : base(w * h) {
            inOpen = new bool[h, w];
        }

        public bool Contains(Location location) {
            return inOpen[location.Row, location.Col];
        }

        public override int Insert(AStarNode<Location> key) {
            Location location = key.Object;
            inOpen[location.Row, location.Col] = true;

            return base.Insert(key);
        }

        public override AStarNode<Location> ExtractMax() {
            AStarNode<Location> result = base.ExtractMax();

            Location location = result.Object;
            inOpen[location.Row, location.Col] = false;

            return result;
        }
    }
}
