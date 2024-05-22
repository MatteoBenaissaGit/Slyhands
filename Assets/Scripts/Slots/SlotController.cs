using System;
using Board;
using Board.Characters;
using LevelEditor;
using Players;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Slots
{
    /// <summary>
    /// The different action the slot can execute
    /// </summary>
    public enum SlotAction
    {
        None = 0,
        Hovered = 1,
        Selected = 2,
        Walkable = 3,
        Attackable = 4,
        GetDestroyed = 5
    }

    /// <summary>
    /// The different types of slot
    /// </summary>
    public enum SlotType
    {
        Base = 0,
        Ramp = 1,
        NotWalkable = 2
    }

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

    /// <summary>
    /// This class stores the slot data
    /// </summary>
    [Serializable]
    public class SlotData
    {
        public SlotData()
        {
            Obstacle = new SlotElement();
            LevelEditorCharacter = new SlotElement();
        }
        
        [field:SerializeField] public SlotType Type { get; set; }
        [field:SerializeField] public string SlotTypeReferenceId { get; set; }
        [field:SerializeField] public Orientation Orientation { get; set; }
        [field:SerializeField] public Vector3Int Coordinates { get; set; }
        
        [field:SerializeField] public SlotElement Obstacle { get; set; }
        [field:SerializeField] public SlotElement LevelEditorCharacter { get; set; }
        
        public BoardCharacterController Character { get; set; }
        
        //values for pathfinding search
        public SlotController PathfindingParent { get; set; }
        public int PathfindingCost { get; set; }
    }
    
    /// <summary>
    /// This class stores the data of an element on the tile
    /// </summary>
    [Serializable]
    public class SlotElement
    {
        public SlotElement()
        {
            Orientation = Orientation.North; //all objects face north by default
        }
        
        public bool Has { get => string.IsNullOrEmpty(PrefabId) == false; }
        [field:SerializeField] public string PrefabId { get; set; } 
        [field:SerializeField] public Orientation Orientation { get; set; }
        [field:SerializeField] public Team Team { get; set; }
        [field:SerializeField] public Vector3Int[] Road { get; set; }
    }
    
    /// <summary>
    /// This class handle the control of the slot and the functional logic in it
    /// </summary>
    public class SlotController : BoardEntity
    {
        public SlotData Data { get; set; }
        
        public SlotLocation Location
        {
            get
            {
                SlotLocation slotLocation = Board.Data.SlotLocations[Coordinates.x, Coordinates.y, Coordinates.z];
                if (slotLocation == null)
                {
                    throw new Exception("no slot location found for the current slot coordinates");
                }
                return slotLocation;
            }
        }

        public SlotController(BoardController board, Vector3Int coordinates, SlotData predefinedData = null) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Slot;
            
            Data = predefinedData ?? new SlotData();
            Data.Coordinates = coordinates;
        }

        /// <summary>
        /// Return if the slot is accessible from the given slot
        /// </summary>
        /// <param name="fromSlot">The slot to check the accessibility from</param>
        /// <returns>is the slot accessible from this slot ?</returns>
        public bool IsAccessibleFromSlot(SlotController fromSlot)
        {
            bool isThereAnObstacle = Data.Obstacle.Has;
            bool isThereACharacter = Data.Character != null;
            if (isThereACharacter || isThereAnObstacle || Data.Type == SlotType.NotWalkable)
            {
                return false;
            }

            if (Coordinates.y + 1 < Board.Data.BoardSize.y)
            {
                SlotController slotUp = Board.GetSlotFromCoordinates(Coordinates + new Vector3Int(0, 1, 0));
                if (slotUp != null)
                {
                    return false;
                }
            }
            
            if (fromSlot == null)
            {
                return true;
            }
            
            bool isSlotOnOtherHeight = Data.Coordinates.y != fromSlot.Data.Coordinates.y && fromSlot.Data.Type != SlotType.Ramp && Data.Type != SlotType.Ramp;
            if (isSlotOnOtherHeight)
            {
                return false;
            }
            
            if (Data.Type == SlotType.Ramp || fromSlot.Data.Type == SlotType.Ramp)
            {
                Orientation orientation = Data.Orientation;
                if (fromSlot.Data.Type == SlotType.Ramp)
                {
                    orientation = fromSlot.Data.Orientation;
                }
                int xDirectionFromSlotToMe = Mathf.Abs(Coordinates.x - fromSlot.Coordinates.x);
                int zDirectionFromSlotToMe = Mathf.Abs(Coordinates.z - fromSlot.Coordinates.z);

                bool isOrientationXAccessible = (orientation == Orientation.East && xDirectionFromSlotToMe == 1) ||
                                                (orientation == Orientation.West && xDirectionFromSlotToMe == 1);
                bool isOrientationZAccessible = (orientation == Orientation.North && zDirectionFromSlotToMe == 1) ||
                                                (orientation == Orientation.South && zDirectionFromSlotToMe == 1);

                if (isOrientationXAccessible == false && isOrientationZAccessible == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}