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
    /// This class stores the slot data
    /// </summary>
    [Serializable]
    public class SlotData
    {
        [field:SerializeField] public Vector3Int Coordinates { get; set; }
        public bool HasObstacle { get => ObstaclePrefab != null; }
        [field:SerializeField] public GameObject ObstaclePrefab { get; set; }
        public bool HasEntity { get => EntityPrefab != null; }
        [field:SerializeField] public GameObject EntityPrefab { get; set; }
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