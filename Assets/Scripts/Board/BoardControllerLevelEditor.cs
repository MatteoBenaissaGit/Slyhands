using System;
using LevelEditor;
using LevelEditor.LoadAndSave;
using Slots;
using UnityEngine;

namespace Board
{
    public partial class BoardController
    {
        #region Board creation, load ans slot management methods

        /// <summary>
        /// Initialize the board data and setup the slot parent
        /// </summary>
        /// <param name="width">the width of the board</param>
        /// <param name="length">the length of the board</param>
        /// <param name="height">the height of the board</param>
        protected void InitializeBoardData(int width, int length, int height)
        {
            Data = new BoardData(new Vector3Int(width, height, length));

            _slotParent = new GameObject("SlotParent").transform;
            _slotParent.parent = transform;
        }

        /// <summary>
        /// Create a new blank board filled with slots
        /// </summary>
        /// <param name="width">the width of the board</param>
        /// <param name="length">the length of the board</param>
        /// <param name="height">the height of the board</param>
        public void CreateBlankBoard(int width, int height, int length)
        {
            ClearBoard();
            InitializeBoardData(width, length, height);
            ForEachCoordinatesOnBoard(coordinates => CreateSlotLocationAt(coordinates.x, coordinates.y, coordinates.z));

            LevelEditorManager.Instance?.UI.SetHeightSlider(height);
            LevelEditorManager.Instance?.UI.LoadMenu.ResetLastLoadedLevel();
            LevelEditorManager.Instance?.ExtendButtons.Initialize(this);
        }

        /// <summary>
        /// Create a slot location at the desired position
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        private void CreateSlotLocationAt(int x, int y, int z)
        {
            Data.SlotLocations[x, y, z] = Instantiate(_slotLocationPrefab, GetCoordinatesToWorldPosition(new Vector3Int(x, y, z)), Quaternion.identity);
            Data.SlotLocations[x, y, z].transform.parent = _slotParent;
            Data.SlotLocations[x, y, z].Coordinates = new Vector3Int(x, y, z);
            Data.SlotLocations[x, y, z].name = $"SlotLocation {x},{y},{z}";
        }

        /// <summary>
        /// Load a level data and create its board 
        /// </summary>
        /// <param name="data"></param>
        public void LoadBoardFromLevelData(LevelData data)
        {
            CreateBlankBoard(data.BoardData.Width, data.BoardData.Height, data.BoardData.Length);
            foreach (SlotData slotData in data.SlotDataList)
            {
                CreateSlotAt(slotData.Coordinates, slotData);
            }

            //load roads
            foreach (SlotLocation location in Data.SlotLocations)
            {
                if (location.SlotView == null || location.SlotView.LevelEditorCharacterOnSlot == null)
                {
                    continue;
                }

                location.SlotView.LevelEditorCharacterOnSlot.SetRoad(location.SlotView.Controller.Data.LevelEditorCharacter.Road);
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
        /// Clear a slot at defined coordinates
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
        public (SlotController controller, SlotView view) CreateSlotAt(Vector3Int coordinates, SlotData data = null)
        {
            SlotController slot = new SlotController(this, coordinates);
            if (data != null)
            {
                slot.Data = data;
            }

            SlotView slotView = Instantiate(_slotViewPrefab, _slotParent, true);
            slotView.Initialize(slot);
            float slotViewOffsetY = -0.5f;
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates) + new Vector3(0, slotViewOffsetY, 0);

            Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z].SetSlotViewOnLocation(slotView);

            return (slot, slotView);
        }

        /// <summary>
        /// This method extend the board of 1 row in the desired direction
        /// </summary>
        /// <param name="orientation">the direction to extend</param>
        public void ExtendBoard(WorldOrientation.Orientation orientation)
        {
            Vector2Int direction = WorldOrientation.GetDirection(orientation);

            //create new data with new size
            BoardData oldData = Data;
            BoardData newData = new BoardData(oldData.Size + new Vector3Int(Math.Abs(direction.x), 0, Math.Abs(direction.y)));

            //import the old array in it with direction offset*
            ForEachCoordinatesOnBoard(coordinates =>
            {
                Vector3Int newCoordinates = coordinates;
                if (direction.x < 0) newCoordinates.x++;
                if (direction.y < 0) newCoordinates.z++;
                newData.SlotLocations[newCoordinates.x, newCoordinates.y, newCoordinates.z] = oldData.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
                SlotLocation slot = newData.SlotLocations[newCoordinates.x, newCoordinates.y, newCoordinates.z];
                slot.Coordinates = newCoordinates;
                if (slot.SlotView != null && slot.SlotView.Controller != null)
                {
                    slot.SlotView.Controller.Data.Coordinates = newCoordinates;
                }
            });

            //set this new data as the data
            Data = newData;

            //create new slots at the offset raw
            ForEachCoordinatesOnBoard(coordinates =>
            {
                int x = coordinates.x;
                int y = coordinates.y;
                int z = coordinates.z;
                if (Data.SlotLocations[x, y, z] == null)
                {
                    CreateSlotLocationAt(x, y, z);
                    if (y != LevelEditorManager.Instance.UI.HeightSlider.CurrentHeight)
                    {
                        Data.SlotLocations[x, y, z].SetUsable(false);
                    }
                }
                else
                {
                    Data.SlotLocations[x, y, z].gameObject.name = "SlotLocation " + x + "," + y + "," + z;
                }

                Data.SlotLocations[x, y, z].transform.position = GetCoordinatesToWorldPosition(new Vector3Int(x, y, z));
            });
        }

        #endregion

        #region Level Editor Board methods

        /// <summary>
        /// Show the slots at a specific height and hide the other slot floors
        /// </summary>
        public void ViewSlotsAtHeight(int height)
        {
            ForEachCoordinatesOnBoard(
                coordinate =>
                    Data.SlotLocations[coordinate.x, coordinate.y, coordinate.z].SetUsable(coordinate.y == height));
        }

        #endregion
    }
}