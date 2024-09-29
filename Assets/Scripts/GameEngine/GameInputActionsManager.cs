using System;
using System.Collections.Generic;
using Board;
using Board.Characters;
using Inputs;
using LevelEditor;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEngine
{
    public enum GameInputActionState
    {
        Neutral = 0,
        SelectedCharacter = 1,
        SelectedCard = 2,
    }
    
    public class GameInputActionsManager : MonoBehaviour
    {
        private RaycastHit[] _mouseClickRaycastHits = new RaycastHit[6];

        private GameInputActionState _currentInputActionState;
        private BoardCharacterController _selectedCharacter;

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnLeftClick += HandleClick;
        }

        private void Update()
        {
            SlotLocation location = GetCurrentHoveredSlotLocation();
            if (location == null || location != GameManager.Instance.Board.CurrentHoveredLocation)
            {
                GameManager.Instance.Board.CurrentHoveredLocation = location;
            }
        }
        
        /// <summary>
        /// Get the current slot hovered by the mouse
        /// </summary>
        /// <returns>The current slot hovered by the mouse</returns>
        private SlotLocation GetCurrentHoveredSlotLocation()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return null;
            }
            
            Ray ray = GameManager.Instance.Camera.Camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _mouseClickRaycastHits);
            for (int i = 0; i < hits; i++)
            {
                if (_mouseClickRaycastHits[i].collider.TryGetComponent(out SlotLocation location) == false
                    || location.IsUsable == false)
                {
                    continue;
                }
                return location;
            }

            return null;
        }

        /// <summary>
        /// Reset the selection and state of the input action manager
        /// </summary>
        public void ResetSelection()
        {
            _selectedCharacter?.OnCharacterAction.Invoke(CharacterAction.IsUnselected, null);
            _selectedCharacter = null;
            
            _currentInputActionState = GameInputActionState.Neutral;
        }
        
        /// <summary>
        /// Handle the click action to do when the player click depending on the current state of the input action manager
        /// </summary>
        private void HandleClick()
        {
            if (GameManager.Instance.Board.CurrentHoveredLocation == null)
            {
                ResetSelection();
                return;
            }

            switch (_currentInputActionState)
            {
                case GameInputActionState.Neutral:
                    HandleClickInNeutralState();
                    break;
                case GameInputActionState.SelectedCharacter:
                    HandleClickInCharacterSelectionState();
                    break;
                case GameInputActionState.SelectedCard:
                    HandleClickInSelectedCardState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Handle the click action to do when the player click in the neutral state
        /// </summary>
        private void HandleClickInNeutralState()
        {
            BoardCharacterController characterOnSlot = GameManager.Instance.Board.CurrentHoveredLocation.SlotView.Controller.Data.Character;
            
            // If the slot has a character and the character can be played
            if (characterOnSlot != null && characterOnSlot.CanGetPlayed())
            {
                _selectedCharacter = characterOnSlot;
                _currentInputActionState = GameInputActionState.SelectedCharacter;
                _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.IsSelected, null);
            }
        }

        /// <summary>
        /// Handle the click action to do when the player click in the character selection state
        /// </summary>
        private void HandleClickInCharacterSelectionState()
        {
            _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.IsUnselected, null);

            BoardController board = GameManager.Instance.Board;
            Vector3Int targetCoordinates = board.CurrentHoveredLocation.Coordinates;
            SlotController targetSlot = board.Data.SlotLocations[targetCoordinates.x, targetCoordinates.y, targetCoordinates.z].SlotView.Controller;
            
            if (targetSlot.Data.Obstacle.Has || targetSlot.Data.Character != null || targetSlot.Data.Type != SlotType.Base 
                || GameManager.Instance.Board.GetAccessibleSlotsByCharacter(_selectedCharacter).Contains(targetSlot) == false)
            {
                ResetSelection();
                return;
            }

            List<SlotController> path = board.GetPathFromSlotToSlot(_selectedCharacter.CurrentSlot, targetSlot);
            _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[]{ path });

            ResetSelection();
        }

        /// <summary>
        /// Handle the click action to do when the player click in the selected card state
        /// </summary>
        private void HandleClickInSelectedCardState()
        {
            //TODO implement card selection
        }
    }
}