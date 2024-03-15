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
        public BoardData(Vector2Int dimensions)
        {
            Width = dimensions.x;
            Length = dimensions.y;
            SlotLocations = new SlotLocation[Width, Length];
        }
        
        public int Width { get; private set; }
        public int Length { get; private set; }
        
        public Vector2Int BoardSize => new Vector2Int(Width, Length);

        /// <summary>
        /// The array containing all board's slots
        /// </summary>
        public SlotLocation[,] SlotLocations { get; set; }
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
        
        [SerializeField, TabGroup("TabGroup1", "Dimensions"), Range(0, 256)] 
        private int _width;
        [SerializeField, TabGroup("TabGroup1", "Dimensions"), Range(0, 256)] 
        private int _length;

        [SerializeField, TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red"), Required]
        private SlotView _slotViewPrefab;
        [SerializeField, TabGroup("TabGroup1", "References"), Required] 
        private SlotLocation _slotLocationPrefab;

        private Transform _slotParent;
        
        #endregion

        private void Start()
        {
            Initialize(_width, _length);
        }

        public void Initialize(int width, int length)
        {
            Data = new BoardData(new Vector2Int(width, length));
            
            _slotParent = new GameObject("SlotParent").transform;
            _slotParent.parent = transform;
        }

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

        public void CreateBoardSlots()
        {
            ClearBoardSlots();
            ForEachCoordinatesOnBoard(coordinates => CreateSlotLocationAt(coordinates.x, coordinates.y));
        }

        private void CreateSlotLocationAt(int x, int y)
        {
            Data.SlotLocations[x, y] = Instantiate(_slotLocationPrefab, GetCoordinatesToWorldPosition(new Vector2Int(x,y)), Quaternion.identity);
            Data.SlotLocations[x, y].transform.parent = _slotParent;
            Data.SlotLocations[x, y].Coordinates = new Vector2Int(x, y);
        }
        
        /// <summary>
        /// Clear all the slots on current board
        /// </summary>
        private void ClearBoardSlots()
        {
            foreach (SlotLocation slot in Data.SlotLocations)
            {
                if (slot == null || slot.gameObject == null)
                {
                    continue;
                }
                slot.DestroySlotViewOnLocation();
                Destroy(slot.gameObject);
            }
            Data.SlotLocations = new SlotLocation[_width, _length];
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
        public SlotController CreateSlotAt(Vector2Int coordinates)
        {
            SlotController slot = new SlotController(this, coordinates);
            
            SlotView slotView = Instantiate(_slotViewPrefab, _slotParent, true);
            slotView.Initialize(slot);
            float slotViewOffsetY = -0.5f;
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates) + new Vector3(0,slotViewOffsetY,0);
            
            Data.SlotLocations[coordinates.x, coordinates.y].SetSlotOnLocation(slotView);
            
            return slot;
        }
    }
}