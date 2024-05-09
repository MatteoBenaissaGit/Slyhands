using System;
using Board;
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
            Obstacle = new TileElement();
            Character = new TileElement();
        }
        
        [field:SerializeField] public SlotType Type { get; set; }
        [field:SerializeField] public string SlotTypeReferenceId { get; set; }
        [field:SerializeField] public Orientation Orientation { get; set; }
        [field:SerializeField] public Vector3Int Coordinates { get; set; }
        
        [field:SerializeField] public TileElement Obstacle { get; set; }
        [field:SerializeField] public TileElement Character { get; set; }
    }
    
    /// <summary>
    /// This class stores the data of an element on the tile
    /// </summary>
    [Serializable]
    public class TileElement
    {
        public TileElement()
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
        /// <summary>
        /// This action get invoked on any slot actions
        /// </summary>
        public Action<SlotAction, bool> OnSlotAction { get; set; }
        public SlotData Data { get; set; }

        public SlotController(BoardController board, Vector3Int coordinates, SlotData predefinedData = null) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Slot;
            
            Data = predefinedData ?? new SlotData();
            Data.Coordinates = coordinates;
            
            OnSlotAction += SlotActionController;
        }

        private void SlotActionController(SlotAction action, bool isValid)
        {
            switch (action)
            {
                case SlotAction.None:
                    break;
                case SlotAction.Hovered:
                    break;
                case SlotAction.Selected:
                    break;
                case SlotAction.Walkable:
                    break;
                case SlotAction.Attackable:
                    break;
                case SlotAction.GetDestroyed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}