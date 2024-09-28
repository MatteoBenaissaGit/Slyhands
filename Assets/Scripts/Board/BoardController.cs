using System;
using System.Collections.Generic;
using Board.Characters;
using Data.Prefabs;
using GameEngine;
using LevelEditor;
using LevelEditor.Entities;
using LevelEditor.LoadAndSave;
using Players;
using Sirenix.OdinInspector;
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
        [field:SerializeField] public int Height { get; private set; } //Y
        [field:SerializeField] public int Length { get; private set; } //Z
        public Vector3Int BoardSize => new Vector3Int(Width, Height, Length);
        public SlotLocation[,,] SlotLocations { get; set; }
        [field:SerializeField] public Vector3Int PlayerStartCoordinates { get; set; }
    }
    
    /// <summary>
    /// This class handle the board management and store the slots on it
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        #region Properties
        
        public BoardData Data { get; private set; }
        
        public SlotLocation CurrentSelectedLocation
        {
            get => _currentSelectedLocation;
            set
            {
                _currentSelectedLocation?.SetSelected(false);
                _currentSelectedLocation = value;
                _currentSelectedLocation?.SetSelected(true);
            }
        }
        private SlotLocation _currentSelectedLocation;

        public SlotLocation CurrentHoveredLocation
        {
            get => _currentHoveredLocation;
            set
            {
                _currentHoveredLocation?.SetHovered(false);
                _currentHoveredLocation = value;
                _currentHoveredLocation?.SetHovered(true);
            }
        }


        private SlotLocation _currentHoveredLocation;

        public Vector3 WorldCenter => GetCoordinatesToWorldPosition(new Vector3(Data.BoardSize.x - 1,Data.BoardSize.y - 1,Data.BoardSize.z - 1) / 2f);
            
        #endregion

        #region Private values
        
        [SerializeField, TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red"), Required] private SlotView _slotViewPrefab;
        [SerializeField, TabGroup("TabGroup1", "References"), Required] private SlotLocation _slotLocationPrefab;

        private Transform _slotParent;
        private Vector2Int[] _directions = { new (0,1), new (0,-1), new (1,0), new (-1,0), };
        
        #endregion

        #region Common

        /// <summary>
        /// This method get the desired world position related to a board coordinates
        /// </summary>
        /// <param name="coordinates">The coordinates you need to translate in world position</param>
        /// <returns>A Vector3 of the world position associated to the given coordinates</returns>
        public Vector3 GetCoordinatesToWorldPosition(Vector3 coordinates)
        {
            return transform.position + new Vector3(coordinates.x, coordinates.y, coordinates.z);
        }
        
        /// <summary>
        /// Get a slot in the data from a defined coordinates
        /// </summary>
        /// <param name="currentRoadPosition">The coordinate to get the slot from</param>
        /// <returns>The slot controller at this coordinate</returns>
        public SlotController GetSlotFromCoordinates(Vector3Int currentRoadPosition)
        {
            return Data.SlotLocations[currentRoadPosition.x, currentRoadPosition.y, currentRoadPosition.z]?.SlotView?.Controller;
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
        /// Get all the neighbors of a defined slot
        /// </summary>
        /// <param name="slot">the slot to check from</param>
        /// <returns>The list of its neighbors</returns>
        private List<SlotController> GetNeighborsOfSlot(SlotController slot, bool checkForRampsUp = false)
        {
            List<SlotController> neighborsSlots = new List<SlotController>();
            foreach (Vector2Int direction in _directions)
            {
                Vector3Int nextCoordinates = new Vector3Int(slot.Coordinates.x + direction.x, slot.Coordinates.y, slot.Coordinates.z + direction.y);
                if (Data.BoardSize.x <= nextCoordinates.x || nextCoordinates.x < 0 ||
                    Data.BoardSize.y <= nextCoordinates.y || nextCoordinates.y < 0 ||
                    Data.BoardSize.z <= nextCoordinates.z || nextCoordinates.z < 0)
                {
                    continue;
                }
                
                SlotController neighborSlot = Data.SlotLocations[nextCoordinates.x, nextCoordinates.y, nextCoordinates.z]?.SlotView?.Controller;
                
                //if there is no neighbor slot
                if (neighborSlot == null)
                {
                    //but if slot is a ramp, check below where the ramp is leading
                    if (slot.Data.Type == SlotType.Ramp && slot.Coordinates.y - 1 >= 0)
                    {
                        Vector2Int rampDirection = GetDirectionOfOrientation(slot);
                        SlotLocation rampLeadingLocation = Data.SlotLocations[
                            slot.Coordinates.x + rampDirection.x, 
                            slot.Coordinates.y - 1, 
                            slot.Coordinates.z + rampDirection.y];
                        
                        if (rampLeadingLocation != null 
                            && rampLeadingLocation.SlotView != null 
                            && neighborsSlots.Contains(rampLeadingLocation.SlotView.Controller) == false)
                        {
                            neighborsSlots.Add(rampLeadingLocation.SlotView.Controller);
                        }
                    }

                    if (checkForRampsUp == false)
                    {
                        continue;
                    }
                }
                
                if (checkForRampsUp)
                {
                    //if there is a ramp up above the next direction, add it to the neighbors
                    if (nextCoordinates.y + 1 < Data.BoardSize.y)
                    {
                        SlotLocation nextCoordinatesUpperSlotLocation = Data.SlotLocations[nextCoordinates.x, nextCoordinates.y + 1, nextCoordinates.z];
                        if (nextCoordinatesUpperSlotLocation != null 
                            && nextCoordinatesUpperSlotLocation.SlotView != null 
                            && nextCoordinatesUpperSlotLocation.SlotView.Controller.Data.Type == SlotType.Ramp)
                        {
                            if (slot.Data.Type == SlotType.Ramp)
                            {
                                //TODO check if im also a slot that slot direction == upperSlot direction (double stairs)
                            }
                            
                            neighborsSlots.Add(nextCoordinatesUpperSlotLocation.SlotView.Controller);
                            
                            continue;
                        }
                    }
                }

                if (neighborSlot != null && neighborSlot.IsAccessibleFromSlot(null))
                {
                    neighborsSlots.Add(neighborSlot);
                }
            }
            
            return neighborsSlots;
        }

        /// <summary>
        /// Using the A* algorithm, get the path from a character to a slot
        /// </summary>
        /// <param name="startSlot">the character from which the path start</param>
        /// <param name="endSlot">the slot to reach</param>
        /// <returns>return a list of the slots composing the path in order</returns>
        public List<SlotController> GetPathFromSlotToSlot(SlotController startSlot, SlotController endSlot)
        {
            List<SlotController> path = new List<SlotController>();

            startSlot.Data.PathfindingCost = 0;
            startSlot.Data.PathfindingParent = null;
            
            List<SlotController> openSlots = new List<SlotController> { startSlot };
            List<SlotController> closedSlots = new List<SlotController>();
            
            while (openSlots.Count > 0)
            {
                //get the first slot in open list
                SlotController currentSlot = openSlots[0];
                openSlots.RemoveAt(0);
                if (closedSlots.Contains(currentSlot))
                {
                    continue;
                }
                closedSlots.Add(currentSlot);
                
                //we found the end slot
                if (currentSlot == endSlot)
                {
                    SlotController currentSlotParent = currentSlot;
                    while (currentSlotParent != startSlot && currentSlotParent.Data.PathfindingParent != null)
                    {
                        path.Add(currentSlotParent);
                        currentSlotParent = currentSlotParent.Data.PathfindingParent;
                    }
                    path.Reverse();
                    break;
                }
                
                //get the neighbors of the current slot and add them to the open list
                List<SlotController> neighbors = GetNeighborsOfSlot(currentSlot, true);
                foreach (SlotController neighbor in neighbors)
                {
                    int newCost = currentSlot.Data.PathfindingCost + 1;

                    //if the slot is not accessible or in closed : continue
                    bool isAccessible = neighbor.IsAccessibleFromSlot(currentSlot);
                    if (isAccessible == false || closedSlots.Contains(neighbor))
                    {
                        continue;
                    }
                    
                    //if the cost from my case if lower than the one already set, update it
                    if (openSlots.Contains(neighbor) == false || newCost < neighbor.Data.PathfindingCost)
                    {
                        neighbor.Data.PathfindingCost = newCost;
                        neighbor.Data.PathfindingParent = currentSlot;
                        openSlots.Add(neighbor);
                    }
                }
            }

            closedSlots.ForEach(x => x.Data.PathfindingParent = null);
            closedSlots.ForEach(x => x.Data.PathfindingCost = int.MaxValue);
            return path;
        }
        
        private static Vector2Int GetDirectionOfOrientation(SlotController slot)
        {
            Vector2Int rampDirection = new Vector2Int(0, 0);
            switch (slot.Data.Orientation)
            {
                case WorldOrientation.Orientation.North:
                    rampDirection = new Vector2Int(0, 1);
                    break;
                case WorldOrientation.Orientation.South:
                    rampDirection = new Vector2Int(0, -1);
                    break;
                case WorldOrientation.Orientation.East:
                    rampDirection = new Vector2Int(1, 0);
                    break;
                case WorldOrientation.Orientation.West:
                    rampDirection = new Vector2Int(-1, 0);
                    break;
            }

            return rampDirection;
        }
        
        #endregion

        #region Board creation, load ans slot management methods

        /// <summary>
        /// Initialize the board data and setup the slot parent
        /// </summary>
        /// <param name="width">the width of the board</param>
        /// <param name="length">the length of the board</param>
        /// <param name="height">the height of the board</param>
        protected void InitializeBoardData(int width, int length, int height)
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
        public void CreateBlankBoard(int width, int height, int length)
        {
            ClearBoard();
            InitializeBoardData(width, length, height);
            ForEachCoordinatesOnBoard(coordinates => CreateSlotLocationAt(coordinates.x, coordinates.y, coordinates.z));
            
            LevelEditorManager.Instance?.UI.SetHeightSlider(height);
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
            Data.SlotLocations[x, y, z] = Instantiate(_slotLocationPrefab, GetCoordinatesToWorldPosition(new Vector3Int(x,y,z)), Quaternion.identity);
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
            foreach (SlotLocation location in  Data.SlotLocations)
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
        protected void ClearBoardSlot(Vector3Int coordinates)
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
            slotView.transform.position = GetCoordinatesToWorldPosition(slot.Coordinates) + new Vector3(0,slotViewOffsetY,0);
            
            Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z].SetSlotViewOnLocation(slotView);
            
            return (slot,slotView);
        }
        
        /// <summary>
        /// This method extend the board of 1 row in the desired direction
        /// </summary>
        /// <param name="orientation">the direction to extend</param>
        public void ExtendBoard(WorldOrientation.Orientation orientation)
        {
            Debug.Log(orientation + " extend");
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
                    Data.SlotLocations[coordinate.x,coordinate.y,coordinate.z].SetUsable(coordinate.y == height));
        }

        #endregion


        #region Game methods

        /// <summary>
        /// Load a game level to be played in the game scene
        /// </summary>
        /// <param name="level">The level to load</param>
        public void LoadGameLevel(LevelData level)
        {
            LoadBoardFromLevelData(level);
            ClearAllUnusedSlotLocation();
            SetSlotLocationsUsable(true);
            SetGameCharacters();
        }
        
        /// <summary>
        /// Clear all of the slot locations where there is no slot view
        /// </summary>
        private void ClearAllUnusedSlotLocation()
        {
            ForEachCoordinatesOnBoard(coordinates =>
            {
                SlotLocation slotLocation = Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
                if (slotLocation.SlotView != null)
                {
                    return;
                }
                ClearBoardSlot(slotLocation.Coordinates);
            });
        }
        
        /// <summary>
        /// Set all of the slot locations usable or not
        /// </summary>
        /// <param name="isUsable">Are they usable or not ?</param>
        private void SetSlotLocationsUsable(bool isUsable)
        {
            ForEachCoordinatesOnBoard(
                coordinates => Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z]?.SetUsable(isUsable));
        }

        /// <summary>
        /// Replace the level editor characters on the board with board characters
        /// </summary>
        private void SetGameCharacters()
        {
            ForEachCoordinatesOnBoard(coordinates =>
            {
                //check if the slot has a character
                SlotLocation slotLocation = Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
                if (slotLocation.SlotView == null)
                {
                    return;
                }
                if (slotLocation.SlotView.Controller.Data.LevelEditorCharacter.Has == false)
                {
                    return;
                }
                
                PrefabsData prefabsData = GameManager.Instance.PrefabsData;
                
                //character controller
                SlotElement levelEditorCharacterElement = slotLocation.SlotView.Controller.Data.LevelEditorCharacter;
                GameObject levelEditorCharacterPrefab = prefabsData.GetPrefab(slotLocation.SlotView.Controller.Data.LevelEditorCharacter.PrefabId);
                Team team = GameManager.Instance.TeamsData.Teams.Find(x => x.TeamNumber == levelEditorCharacterElement.Team.TeamNumber);
                CharacterType type = levelEditorCharacterPrefab.GetComponent<LevelEditorCharacter>().Type;
                BoardCharacterController characterController = new BoardCharacterController(this, coordinates, team, type);
                characterController.GameplayData.Orientation = levelEditorCharacterElement.Orientation;
                
                //character view
                string characterPrefabId = prefabsData.GetPrefabSecondId(levelEditorCharacterPrefab);
                GameObject characterPrefab = prefabsData.GetPrefab(characterPrefabId);
                BoardCharacterView characterView = Instantiate(characterPrefab, slotLocation.transform.position, Quaternion.identity).GetComponent<BoardCharacterView>();
                if (characterView == null)
                {
                    throw new Exception("Character prefab has no BoardCharacterView component");
                }
                characterView.Initialize(characterController);

                //manage team & player
                if (team == null)
                {
                    throw new Exception($"Team {team.TeamNumber} not found");
                }
                team.Characters.Add(characterController);
                
                //manage slot data & remove level editor character
                slotLocation.SlotView.DestroyCharacterOnSlot();
                slotLocation.SlotView.Controller.Data.Character = characterController;
            });
        }

        /// <summary>
        /// Get all the accessible slots by a board character
        /// </summary>
        /// <param name="characterController">The character to check for</param>
        /// <returns>A list of all the slots accessible</returns>
        public List<SlotController> GetAccessibleSlotsByCharacter(BoardCharacterController characterController)
        {
            HashSet<SlotController> accessibleSlots = new HashSet<SlotController>();

            FindAccessibleSlotFromSlot(characterController.CurrentSlot, characterController.GameplayData.CurrentMovementPoints + 1, ref accessibleSlots, true);

            List<SlotController> accessibleSlotsList = new List<SlotController>();
            foreach (SlotController slot in accessibleSlots)
            {
                int pathCount = GetPathFromSlotToSlot(characterController.CurrentSlot, slot).Count;
                if (pathCount <= characterController.GameplayData.CurrentMovementPoints)
                {
                    accessibleSlotsList.Add(slot);
                }
            }
            
            return accessibleSlotsList;
        }

        /// <summary>
        /// A recursive method that check accessible slots from a slot and add them to an slot list
        /// </summary>
        /// <param name="slot">the slot to check</param>
        /// <param name="movementAmount">the amount of movement permitted from the slot</param>
        /// <param name="slotAccessibleHashSet">the current accessible slot list</param>
        /// <param name="isBaseSlot">is this the base slot from which the check has started ?</param>
        private void FindAccessibleSlotFromSlot(SlotController slot, int movementAmount, ref HashSet<SlotController> slotAccessibleHashSet, bool isBaseSlot = false)
        {
            //if the slot is already in the list or the movement amount is 0, stop
            if (movementAmount <= 0)
            {
                return;
            }
            
            movementAmount--;
            
            //if its the base slot, add it and continue searching neighbors
            if (isBaseSlot)
            {
                slotAccessibleHashSet.Add(slot);
                goto NeighborsSearch;
            }
            
            //if im blocking, dont add me and stop
            if (slot.IsAccessibleFromSlot(null) == false)
            {
                return;
            } 
            
            //if there is a slot up
            if (slot.Coordinates.y + 1 < Data.BoardSize.y)
            {
                SlotLocation upperSlotLocation = Data.SlotLocations[slot.Coordinates.x, slot.Coordinates.y + 1, slot.Coordinates.z];
                if (upperSlotLocation != null && upperSlotLocation.SlotView != null )
                {
                    //if its a ramp continue searching neighbors on the level its leading to
                    if (upperSlotLocation.SlotView.Controller.Data.Type == SlotType.Ramp)
                    {
                        List<SlotController> upSlotNeighbors = GetNeighborsOfSlot(upperSlotLocation.SlotView.Controller);
                        foreach (SlotController upSlotNeighbor in upSlotNeighbors)
                        {
                            FindAccessibleSlotFromSlot(upSlotNeighbor, movementAmount, ref slotAccessibleHashSet);
                        }
                    }
                    return;
                }
            }

            if (slot.Data.Type == SlotType.Base)
            {
                slotAccessibleHashSet.Add(slot);
            }

            //search the neighbors
            NeighborsSearch:
            List<SlotController> neighbors = GetNeighborsOfSlot(slot, true);
            foreach (SlotController neighbor in neighbors)
            {
                FindAccessibleSlotFromSlot(neighbor, movementAmount, ref slotAccessibleHashSet);
            }
        }

        #endregion
    }
}