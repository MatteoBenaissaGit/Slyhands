using UnityEngine;

namespace Slots
{
    public class WorldOrientation
    {
        /// <summary>
        /// The four orientation of an entity
        /// </summary>
        public enum Orientation
        {
            North = 0, // 0° // z+1
            East = 1, // 90° // x+1
            South = 2, // 180° // z-1
            West = 3 // 270° // x-1
        }

        public static Vector2Int GetDirection(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.North:
                    return new Vector2Int(0, 1);
                case Orientation.East:
                    return new Vector2Int(1, 0);
                case Orientation.South:
                    return new Vector2Int(0, -1);
                case Orientation.West:
                    return new Vector2Int(-1, 0);
                default:
                    return Vector2Int.zero;
            }
        }
    }
}