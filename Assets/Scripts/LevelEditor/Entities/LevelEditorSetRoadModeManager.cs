using System.Collections.Generic;
using Board;
using Inputs;
using LevelEditor.ActionButtons;
using Slots;
using UnityEngine;

namespace LevelEditor.Entities
{
    public class LevelEditorSetRoadModeManager
    {
        public LevelEditorCharacter CurrentCharacter { get; private set; }

        private Vector3Int[] _currentRoadPositions;
        private LineRenderer _roadLineRenderer;

        public void StartMode(LevelEditorCharacter character)
        {
            CurrentCharacter = character;
            _currentRoadPositions = new Vector3Int[]{CurrentCharacter.Coordinates};

            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(slotLocation.SlotView.LevelEditorCharacterOnSlot == CurrentCharacter);
            }
            
            LevelEditorManager.Instance.UI.Shortcuts.SetShortcuts(LevelEditorActionButtonType.SetRoad);

            _roadLineRenderer = Object.Instantiate(LevelEditorManager.Instance.RoadLinePrefab);
            _roadLineRenderer.startColor = CurrentCharacter.GetTeam().TeamColor;
            _roadLineRenderer.endColor = CurrentCharacter.GetTeam().TeamColor;
            
            InputManager.Instance.LevelEditorInput.OnCameraMoved += _ => UpdateRoad();
        }

        public void ExitMode()
        {
            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(true);
            }
            
            Object.Destroy(_roadLineRenderer);
            _currentRoadPositions = null;
            
            InputManager.Instance.LevelEditorInput.OnCameraMoved -= _ => UpdateRoad();

            //TODO save road for character
        }
        
        private void UpdateRoad()
        {
            BoardController board = LevelEditorManager.Instance.Board;
            
            if (_currentRoadPositions == null 
                || _currentRoadPositions.Length == 0 
                || board.CurrentHoveredLocation == null
                || board.CurrentHoveredLocation.CanEntityBePlacedHere(BoardEntitySuperType.Character) == false
                || board.CurrentHoveredLocation.SlotView.Controller.Data.Obstacle.Has)
            {
                return;
            }

            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < _currentRoadPositions.Length; i++)
            {
                if (i > 0)
                {
                    positions.Remove(positions[^1]);
                }
                positions.Add(board.GetCoordinatesToWorldPosition(_currentRoadPositions[i]));
                if (i + 1 < _currentRoadPositions.Length)
                {
                    SlotController startSlot = board.GetSlotFromCoordinates(_currentRoadPositions[i]);
                    SlotController endSlot = board.GetSlotFromCoordinates(_currentRoadPositions[i+1]);
                    List<SlotController> pathSlots = board.GetPathFromSlotToSlot(startSlot, endSlot);
                    
                    pathSlots.ForEach(x => positions.Add(board.GetCoordinatesToWorldPosition(x.Coordinates)));
                }
            }
            SlotController currentHoveredStartSlot = board.GetSlotFromCoordinates(_currentRoadPositions[^1]);
            List<SlotController> currentHoveredPathSlots = board.GetPathFromSlotToSlot(currentHoveredStartSlot, board.CurrentHoveredLocation.SlotView.Controller);
            currentHoveredPathSlots.ForEach(x => positions.Add(board.GetCoordinatesToWorldPosition(x.Coordinates)));
            
            _roadLineRenderer.positionCount = positions.Count;
            
            for (int i = 0; i < positions.Count; i++)
            {
                _roadLineRenderer.SetPosition(i, positions[i] + new Vector3(0,0.1f,0));
            }
        }
    }
}