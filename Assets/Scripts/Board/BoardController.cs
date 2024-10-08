using System;
using System.Collections.Generic;
using System.Linq;
using Board.Characters;
using Data.Prefabs;
using GameEngine;
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
        public Vector3Int Size => new Vector3Int(Width, Height, Length);
        public SlotLocation[,,] SlotLocations { get; set; }
        [field:SerializeField] public Vector3Int PlayerStartCoordinates { get; set; }
    }
    
    /// <summary>
    /// This class handle the board management and store the slots on it
    /// </summary>
    public partial class BoardController : MonoBehaviour
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

        public Vector3 WorldCenter => GetCoordinatesToWorldPosition(new Vector3(Data.Size.x - 1,Data.Size.y - 1,Data.Size.z - 1) / 2f);
            
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
        /// <param name="coordinate">The coordinate to get the slot from</param>
        /// <returns>The slot controller at this coordinate</returns>
        public SlotController GetSlotFromCoordinates(Vector3Int coordinate)
        {
            if (IsCoordinatesInBoard(coordinate) == false)
            {
                return null;
            }
            
            return Data.SlotLocations[coordinate.x, coordinate.y, coordinate.z]?.SlotView?.Controller;
        }
        
        public bool IsCoordinatesInBoard(Vector3Int coordinates)
        {
            return coordinates.x >= 0 && coordinates.x < Data.Width &&
                   coordinates.y >= 0 && coordinates.y < Data.Height &&
                   coordinates.z >= 0 && coordinates.z < Data.Length;
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
        /// <param name="checkForRampsUp"></param>
        /// <returns>The list of its neighbors</returns>
        private List<SlotController> GetNeighborsOfSlot(SlotController slot, bool checkForRampsUp = false, params PathFindingOption[] options)
        {
            List<SlotController> neighborsSlots = new List<SlotController>();
            foreach (Vector2Int direction in _directions)
            {
                Vector3Int nextCoordinates = new Vector3Int(slot.Coordinates.x + direction.x, slot.Coordinates.y, slot.Coordinates.z + direction.y);
                if (Data.Size.x <= nextCoordinates.x || nextCoordinates.x < 0 ||
                    Data.Size.y <= nextCoordinates.y || nextCoordinates.y < 0 ||
                    Data.Size.z <= nextCoordinates.z || nextCoordinates.z < 0)
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
                    if (nextCoordinates.y + 1 < Data.Size.y)
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

                if (neighborSlot != null && neighborSlot.IsAccessibleFromSlot(null, options))
                {
                    neighborsSlots.Add(neighborSlot);
                }
            }
            
            return neighborsSlots;
        }
        
        public List<SlotController> GetPath(Vector3Int controllerCoordinates, Vector3Int enemyAttackedCoordinates, params PathFindingOption[] options)
        {
            var startSlot = GetSlotFromCoordinates(controllerCoordinates);
            var endSlot = GetSlotFromCoordinates(enemyAttackedCoordinates);
            return GetPath(startSlot, endSlot, options);
        }

        /// <summary>
        /// Using the A* algorithm, get the path from a slot to another slot
        /// </summary>
        /// <param name="startSlot">the character from which the path start</param>
        /// <param name="endSlot">the slot to reach</param>
        /// <param name="options">specific options for the pathfinding search</param>
        /// <returns>return a list of the slots composing the path in order</returns>
        public List<SlotController> GetPath(SlotController startSlot, SlotController endSlot, params PathFindingOption[] options)
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
                List<SlotController> neighbors = GetNeighborsOfSlot(currentSlot, true, options);
                foreach (SlotController neighbor in neighbors)
                {
                    int newCost = currentSlot.Data.PathfindingCost + 1;

                    //if the slot is not accessible or in closed : continue
                    bool isAccessible = neighbor.IsAccessibleFromSlot(currentSlot, options);
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
                Team team = GameManager.Instance.Teams.ToList().Find(x => x.Number == levelEditorCharacterElement.Team.Number);
                CharacterType type = levelEditorCharacterPrefab.GetComponent<LevelEditorCharacter>().Type;
                BoardCharacterController characterController = new BoardCharacterController(this, coordinates, team, type, levelEditorCharacterElement.Orientation);
                
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
                    throw new Exception($"Team {team.Number} not found");
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
        /// <param name="fromSlot">The slot from which to search the accessibles</param>
        /// <param name="movementPoints">The movement points available, by default will use the character data points</param>
        /// <returns>A list of all the slots accessible</returns>
        public List<SlotController> GetAccessibleSlotsBySlot(SlotController fromSlot, int movementPoints)
        {
            HashSet<SlotController> accessibleSlots = new HashSet<SlotController>();

            int movementAmount = movementPoints + 1;
            FindAccessibleSlotFromSlot(fromSlot, movementAmount, ref accessibleSlots, true);

            List<SlotController> accessibleSlotsList = new List<SlotController>();
            foreach (SlotController slot in accessibleSlots)
            {
                int pathCount = GetPath(fromSlot, slot).Count;
                if (pathCount <= movementPoints)
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
            if (slot.Coordinates.y + 1 < Data.Size.y)
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
        
        public bool GetClosestToSlotFromSlot(SlotController targetSlot, SlotController fromSlot, out SlotController slot)
        {
            List<SlotController> path = GetPath(fromSlot, targetSlot, PathFindingOption.IgnoreCharacters);
            for (int i = path.Count - 1; i >= 0; i--)
            {
                SlotController pathSlot = path[i];
                if (pathSlot.IsAccessible)
                {
                    slot = pathSlot;
                    return true;
                }
            }
            
            slot = null;
            return false;
        }

        #endregion

        
    }

    public enum PathFindingOption
    {
        None = 0,
        IgnoreCharacters = 1,
        IgnoreObstacles = 2,
    }
}