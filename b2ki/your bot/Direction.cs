using System;

namespace Ants {

	public enum Direction {
		North,
		South,
		East,
		West,
        None
	}

	public static class DirectionExtensions {

        public static Direction GetPerpendicular(this Direction self) {
            switch (self) {
                case Direction.East:
                    return Direction.North;

                case Direction.North:
                    return Direction.West;

                case Direction.West:
                    return Direction.South;

                case Direction.South:
                    return Direction.East;

                default:
                    throw new ArgumentException("Unknown direction", "self");
            }
        }

        public static Direction GetFlipped(this Direction self) {
            switch (self) {
                case Direction.North:
                    return Direction.South;

                case Direction.South:
                    return Direction.North;

                case Direction.East:
                    return Direction.West;

                case Direction.West:
                    return Direction.East;

                default:
                    throw new ArgumentException("Unknown direction", "self");
            }
        }

		public static char ToChar (this Direction self) {
			switch (self)
			{
				case Direction.East:
					return 'e';

				case Direction.North:
					return 'n';

				case Direction.South:
					return 's';

				case Direction.West:
					return 'w';

				default:
                    throw new ArgumentException("Unknown direction", "self");
			}
		}
	}
}