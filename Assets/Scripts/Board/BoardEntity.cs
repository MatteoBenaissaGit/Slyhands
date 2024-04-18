using LevelEditor;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// The super type represent the base type of each entity, it is the most global qualifier for an entity
    /// </summary>
    public enum BoardEntitySuperType
    {
        None = 0,
        Slot = 1,
        Character = 2,
        Building = 3,
        Obstacle = 4
    }

    /// <summary>
    /// This is the base class for any entity on the board
    /// </summary>
    public abstract class BoardEntity
    {
        /// <summary>
        /// The X,Y and Z coordinates of the entity on the board
        /// </summary>
        public Vector3Int Coordinates { get; protected set; }

        /// <summary>
        /// The super type of the entity
        /// </summary>
        public BoardEntitySuperType SuperType { get; protected set; }

        /// <summary>
        /// The board on which is the entity
        /// </summary>
        public BoardController Board { get; private set; }

        public BoardEntity(BoardController board, Vector3Int coordinates)
        {
            Coordinates = coordinates;
            Board = board;
        }
    }
}