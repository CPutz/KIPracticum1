using System.Collections.Generic;

namespace Ants {

	public interface IGameState {

		/// <summary>
		/// Gets the width of the map.
		/// </summary>
		int Width { get; }

		/// <summary>
		/// Gets the height of the map.
		/// </summary>
		int Height { get; }

		/// <summary>
		/// Gets the allowed load time in milliseconds.
		/// </summary>
		int LoadTime { get; }

		/// <summary>
		/// Gets the allowed turn time in milliseconds.
		/// </summary>
		int TurnTime { get; }

		/// <summary>
		/// Gets the allowed turn time remaining in milliseconds.
		/// </summary>
		int TimeRemaining { get; }

		/// <summary>
		/// Gets the ant view range radius squared.
		/// </summary>
		int ViewRadius2 { get; }

        /// <summary>
        /// Gets the squareroot of ViewRadius2 as integer.
        /// </summary>
        int ViewRadius { get; }

		/// <summary>
		/// Gets the ant attack range radius squared.
		/// </summary>
		int AttackRadius2 { get; }

        /// <summary>
        /// Gets the squareroot of AttackRadius2 as integer.
        /// </summary>
        int AttackRadius { get; }

		/// <summary>
		/// Gets the ant hill spawn radius squared.
		/// </summary>
		int SpawnRadius2 { get; }

        /// <summary>
        /// Gets the current turn number.
        /// </summary>
        int Turn { get; }

		/// <summary>
		/// Gets a list of your currently visible friendly hills.
		/// </summary>
		List<AntHill> MyHills { get; }

		/// <summary>
		/// Gets a list of your ants.
		/// </summary>
		List<Ant> MyAnts { get; }

		/// <summary>
		/// Gets a list of currently visible enemy ants.
		/// </summary>
		Map<Ant> EnemyAnts { get; }

		/// <summary>
		/// Gets a list of currently visible enemy hills.
		/// </summary>
		Map<AntHill> EnemyHills { get; }

        /// <summary>
        /// Gets a list of currently-visible locations where ants of our team died last turn.
        /// </summary>
        List<Ant> MyDeads { get; }

		/// <summary>
		/// Gets a list of currently-visible locations where enemy ants died last turn.
		/// </summary>
		List<Location> EnemyDeads { get; }

		/// <summary>
		/// Gets a list of food tiles visible this turn.
		/// </summary>
		Map<Location> FoodTiles { get; }

        /// <summary>
        /// Gets whether a location is visible or not.
        /// </summary>
        bool[,] VisibilityMap { get; }

        /// <summary>
        /// Gets whether an ant could be attack at a specific location
        /// </summary>
        bool[,] EnemyAttackMap { get; }

		/// <summary>
		/// Gets the <see cref="Tile"/> for the <paramref name="location"/>.
		/// </summary>
		Tile this [Location location] { get; }

		/// <summary>
		/// Gets the <see cref="Tile"/> for the <paramref name="row"/> and <paramref name="col"/>.
		/// </summary>
		Tile this [int row, int col] { get; }

		/// <summary>
		/// Gets whether <paramref name="location"/> is passable or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
		/// <seealso cref="GameState.GetIsUnoccupied"/>
		bool GetIsPassable (Location location);

		/// <summary>
		/// Gets whether <paramref name="location"/> is occupied or not.
		/// </summary>
		/// <param name="location">The location to check.</param>
		/// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
		bool GetIsUnoccupied (Location location);

        /// <summary>
        /// Gets whether <paramref name="location"/> could be attacked in the next turn.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location could be attacked in the next turn, <c>false</c> otherwise.</returns>
        bool GetIsAttackable (Location location);

		/// <summary>
		/// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
		/// </summary>
		/// <param name="location">The starting location.</param>
		/// <param name="direction">The direction to move.</param>
		/// <returns>The new location, accounting for wrap around.</returns>
		Location GetDestination (Location location, Direction direction);

        /// <summary>
        /// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
        /// </summary>
        /// <param name="location">The starting location.</param>
        /// <param name="direction">The direction to move.</param>
        /// <returns>The new location, accounting for wrap around.</returns>
        Location GetDestination (Location location, Location direction);

		/// <summary>
		/// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The first location to measure with.</param>
		/// <param name="loc2">The second location to measure with.</param>
		/// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
		int GetDistance (Location loc1, Location loc2);

		/// <summary>
		/// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
		/// </summary>
		/// <param name="loc1">The location to start from.</param>
		/// <param name="loc2">The location to determine directions towards.</param>
		/// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
		ICollection<Direction> GetDirections (Location loc1, Location loc2);

        /// <summary>
        /// Tells the gamestate that an ant is moved next turn, in Direction <paramref name="direction"/>. 
        /// This method should always be called when an ant is moved.
        /// </summary>
        /// <param name="ant">The ant to be moved</param>
        /// <param name="direction">The direction the ant is moved in</param>
        void MoveAnt(Ant ant, Direction direction);

        /// <summary>
        /// Returns the position that the Ant <paramref name="ant"/> will be in the next turn.
        /// </summary>
        /// <param name="ant">The ant to be checked</param>
        /// <returns>the position that the Ant <paramref name="ant"/> will be in the next turn.</returns>
        Location GetNextTurnLocation(Ant ant);

        /// <summary>
        /// Gets the Tile Type of <paramref name="location"/>
        /// </summary>
        /// <param name="location">The location to be checked.</param>
        /// <returns>The type of the tile on Location <paramref name="location"/></returns>
        Tile GetTileType(Location location);

		bool GetIsVisible(Location loc);
	}
}