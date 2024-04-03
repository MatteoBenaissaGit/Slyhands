using System;
using LevelEditor;
using LevelEditor.LoadAndSave;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Slots;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// This class hold the board data and the slot locations in it
    /// </summary>
    [Serializable]
    public class BoardData
    {
        public BoardData(Vector3Int dimensions)
        {
            Width = dimensions.x;
            Height = dimensions.y;
            Length = dimensions.z;
            SlotLocations = new SlotLocation[Width, Height, Length];
        }
        
        [field:SerializeField] public int Width { get; private set; } //X
        [field:SerializeField] public int Length { get; private set; } //Z
        [field:SerializeField] public int Height { get; private set; } //Y
        
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

        #endregion

        /// <summary>
        /// Initialize the board data and setup the slot parent
        /// </summary>
        /// <param name="width">the width of the board</param>
        /// <param name="length">the length of the board</param>
        /// <param name="height">the height of the board</param>
        private void InitializeBoardData(int width, int length, int height)
        {
            Data = new BoardData(new Vector3Int(width,height, length));
            
            _slotParent = new GameObject("SlotParent").transform;
            _slotParent.parent = transform;
        }
        
        /// <summary>
        /// Create a new blank board filled with slots
        /// </summary>
        /// <param name="width">the width of the board</param>
        /// <param name="length">the length of the board</param>
        /// <param name="height">the height of the board</param>
        public void CreateBlankBoard(int width, int length, int height)
        {
            ClearBoard();
            InitializeBoardData(width, length, height);
            ForEachCoordinatesOnBoard(coordinates => CreateSlotLocationAt(coordinates.x, coordinates.y, coordinates.z));
        }

        /// <summary>
        /// Create a slot location at the desired position
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        private void CreateSlotLocationAt(int x, int y, int z)
        {
            Data.SlotLocations[x, y, z] = Instantiate(_slotLocationPrefab, GetCoordinatesToWorldPosition(new Vector3Int(x,y,z)), Quaternion.identity);
            Data.SlotLocations[x, y, z].transform.parent = _slotParent;
            Data.SlotLocations[x, y, z].Coordinates = new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Load a level data and create its board 
        /// </summary>
        /// <param name="data"></param>
        public void LoadBoardFromLevelData(LevelData data)
        {
            //TODO clear slot ???
            CreateBlankBoard(data.BoardData.Width, data.BoardData.Height, data.BoardData.Width);
            foreach (SlotData slotData in data.SlotDataList)
            {
                CreateSlotAt(slotData.Coordinates, slotData);
            }
        }
        
        /// <summary>
        /// Clear the current board
        /// </summary>
        private void ClearBoard()
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
        /// Create a slot at defined coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates to create the slot at</param>
        /// <param name="data">The data to put on the slot created, create a new one if null</param>
        /// <returns>The slot created</returns>
        public SlotController CreateSlotAt(Vector3Int coordinates, SlotData data = null)
        {
            SlotController slot = new SlotController(this, coordinates);
            if (data != null)
            {
                slot.Data = data;
            }
            
            SlotView slotView = Instantiate(_slotViewPrefab, _slotParent, true);
            slotView.Initialize(slot);
            float slotViewOffsetY = -0.5f;
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates) + new Vector3(0,slotViewOffsetY,0);
            
            Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z].SetSlotViewOnLocation(slotView);
            
            return slot;
        }

        /// <summary>
        /// Show the slots at a specific height and hide the other slot floors
        /// </summary>
        public void ViewSlotsAtHeight(int height)
        {
            ForEachCoordinatesOnBoard(
                coordinate => 
                    Data.SlotLocations[coordinate.x,coordinate.y,coordinate.z].SetEditable(coordinate.y == height));
        }
    }
}