using System;
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

        public static Orientation GetOrientation(Vector3 direction)
        {
            if ((int)direction.x > 0)
            {
                return Orientation.East;
            }
            if ((int)direction.x < 0)
            {
                return Orientation.West;
            }
            if ((int)direction.z > 0)
            {
                return Orientation.North;
            }
            if ((int)direction.z < 0)
            {
                return Orientation.South;
            }
            throw new Exception("Invalid direction");
        }

        public static Vector3Int TransposeVectorToOrientation(Vector3Int coordinates, Orientation gameplayDataOrientation)
        {
            switch (gameplayDataOrientation)
            {
                case Orientation.North:
                    return new Vector3Int(coordinates.x, coordinates.y, coordinates.z);
                case Orientation.East:
                    return new Vector3Int(coordinates.z, coordinates.y, -coordinates.x);
                case Orientation.South:
                    return new Vector3Int(-coordinates.x, coordinates.y, -coordinates.z);
                case Orientation.West:
                    return new Vector3Int(-coordinates.z, coordinates.y, coordinates.x);
                default:
                    return Vector3Int.zero;
            }
        }
    }
}