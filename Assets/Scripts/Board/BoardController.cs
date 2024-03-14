using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Slots;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// This class handle the board management and store the slots on it
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// The (X,Y) size of the board 
        /// </summary>
        public Vector2Int BoardSize
        {
            get => new Vector2Int(_width, _length);
        }

        /// <summary>
        /// The array containing all board's slots
        /// </summary>
        public SlotController[,] Slots { get; private set; } = new SlotController[,] { };

        #endregion

        #region Private values
        
        [SerializeField, TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red"), Required]
        private SlotView _slotViewPrefab;
        [SerializeField, TabGroup("TabGroup1", "References"), Required, ChildGameObjectsOnly]
        private Transform _slotParentTransform;
        
        private int _width;
        private int _length;

        #endregion

        #region MonoBehaviour methods

        private void Start()
        {
        }

        #endregion

        #region Common

        /// <summary>
        /// This class get the desired world position related to a board coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates you need to translate in world position</param>
        /// <returns>A Vector3 of the world position associated to the given coordinates</returns>
        public Vector3 GetCoordinatesToWorldPosition(Vector2Int coordinates)
        {
            return transform.position + new Vector3(coordinates.x, 0, coordinates.y);
        }

        #endregion

        /// <summary>
        /// Clear all the slots on current board
        /// </summary>
        private void ClearBoard()
        {
            if (Slots == null)
            {
                goto createNewArray;
            }
            
            foreach (SlotController slot in Slots)
            {
                slot.OnSlotAction.Invoke(SlotAction.GetDestroyed, true);
            }

            createNewArray:
            Slots = new SlotController[_width, _length];
        }

        /// <summary>
        /// Make an action for each coordinates of the board
        /// </summary>
        /// <param name="actionToExecute">The action to make</param>
        public void ForEachCoordinatesOnBoard(Action<Vector2Int> actionToExecute)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _length; y++)
                {
                    actionToExecute.Invoke(new Vector2Int(x,y));
                }
            }
        }

        /// <summary>
        /// Create a slot at defined coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates to create the slot at</param>
        /// <returns>The slot created</returns>
        private SlotController CreateSlotAt(Vector2Int coordinates)
        {
            SlotController slot = new SlotController(this, coordinates);
            SlotView slotView = Instantiate(_slotViewPrefab, _slotParentTransform, true);
            slotView.Initialize(slot);
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates);
            return slot;
        }
    }
}