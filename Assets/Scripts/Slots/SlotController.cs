using System;
using Board;
using Board.Characters;
using LevelEditor;
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
    }
}