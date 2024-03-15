using System;
using LevelEditor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Slots;
using UnityEngine;

namespace Board
{
    public class BoardData
    {
        public BoardData(Vector3Int dimensions)
        {
            Width = dimensions.x;
            Height = dimensions.y;
            Length = dimensions.z;
            SlotLocations = new SlotLocation[Width, Height, Length];
        }
        
        public int Width { get; private set; } //X
        public int Length { get; private set; } //Z
        public int Height { get; private set; } //Y
        
        public Vector3Int BoardSize => new Vector3Int(Width, Height, Length);

        /// <summary>
        /// The array containing all board's slots
        /// </summary>
        public SlotLocation[,,] SlotLocations { get; set; }
    }
    
    /// <summary>
    /// This class handle the board management and store the slots on it
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        #region Properties
        
        public BoardData Data { get; private set; }

        #endregion

        #region Private values
        
        
        [SerializeField, TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red"), Required]
        private SlotView _slotViewPrefab;
        [SerializeField, TabGroup("TabGroup1", "References"), Required] 
        private SlotLocation _slotLocationPrefab;

        private Transform _slotParent;
        
        #endregion

        #region Common

        /// <summary>
        /// This class get the desired world position related to a board coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates you need to translate in world position</param>
        /// <returns>A Vector3 of the world position associated to the given coordinates</returns>
        public Vector3 GetCoordinatesToWorldPosition(Vector3Int coordinates)
        {
            return transform.position + new Vector3(coordinates.x, coordinates.y, coordinates.z);
        }

        #endregion

        public void CreateBoard(int width, int length, int height)
        {
            ClearBoardSlots();
            InitializeBoardData(width, length, height);
            ForEachCoordinatesOnBoard(coordinates => CreateSlotLocationAt(coordinates.x, coordinates.y, coordinates.z));
        }

        private void InitializeBoardData(int width, int length, int height)
        {
            Data = new BoardData(new Vector3Int(width,height, length));
            
            _slotParent = new GameObject("SlotParent").transform;
            _slotParent.parent = transform;
        }

        private void CreateSlotLocationAt(int x, int y, int z)
        {
            Data.SlotLocations[x, y, z] = Instantiate(_slotLocationPrefab, GetCoordinatesToWorldPosition(new Vector3Int(x,y,z)), Quaternion.identity);
            Data.SlotLocations[x, y, z].transform.parent = _slotParent;
            Data.SlotLocations[x, y, z].Coordinates = new Vector3Int(x, y, z);
        }
        
        /// <summary>
        /// Clear all the slots on current board
        /// </summary>
        private void ClearBoardSlots()
        {
            ForEachCoordinatesOnBoard(ClearBoardSlot);
        }

        /// <summary>
        /// Cleat a slot at defined coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates of the slot to clear</param>
        private void ClearBoardSlot(Vector3Int coordinates)
        {
            SlotLocation slot = Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
            if (slot == null)
            {
                return;
            }
            slot.DestroySlotViewOnLocation();
            Destroy(slot.gameObject);
        }

        /// <summary>
        /// Make an action for each coordinates of the board
        /// </summary>
        /// <param name="actionToExecute">The action to make</param>
        public void ForEachCoordinatesOnBoard(Action<Vector3Int> actionToExecute)
        {
            if (Data == null)
            {
                return;
            }
            
            for (int y = 0; y < Data.Height; y++) //height
            {
                for (int x = 0; x < Data.Width; x++) //width
                {
                    for (int z = 0; z < Data.Length; z++) //length
                    {
                        actionToExecute.Invoke(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        /// <summary>
        /// Create a slot at defined coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates to create the slot at</param>
        /// <returns>The slot created</returns>
        public SlotController CreateSlotAt(Vector3Int coordinates)
        {
            SlotController slot = new SlotController(this, coordinates);
            
            SlotView slotView = Instantiate(_slotViewPrefab, _slotParent, true);
            slotView.Initialize(slot);
            float slotViewOffsetY = -0.5f;
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates) + new Vector3(0,slotViewOffsetY,0);
            
            Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z].SetSlotOnLocation(slotView);
            
            return slot;
        }

        /// <summary>
        /// Show the slots at a specific height and hide the other slot floors
        /// </summary>
        public void ViewSlotsAtHeight(int height)
        {
            
        }
    }
}