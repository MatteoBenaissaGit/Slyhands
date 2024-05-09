using System;
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
    }
    
    public class GameInputActionsManager : MonoBehaviour
    {
        private RaycastHit[] _mouseClickRaycastHits = new RaycastHit[6];

        private GameInputActionState _currentInputActionState;
        private BoardCharacterController _selectedCharacter;

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnClickTap += HandleClick;
        }

        private void Update()
        {
            SlotLocation location = GetCurrentHoveredSlotLocation();
            if (location == null || location != GameManager.Instance.Board.CurrentHoveredLocation)
            {
                GameManager.Instance.Board.CurrentHoveredLocation = location;
            }
        }
        
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

        public void ResetSelection()
        {
            _selectedCharacter = null;
            _currentInputActionState = GameInputActionState.Neutral;
        }
        
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleClickInNeutralState()
        {
            BoardCharacterController characterOnSlot =
                GameManager.Instance.Board.CurrentHoveredLocation.SlotView.Controller.Data.Character;
            if (characterOnSlot != null)
            {
                _selectedCharacter = characterOnSlot;
                _currentInputActionState = GameInputActionState.SelectedCharacter;
                _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.IsSelected);
            }
        }

        private void HandleClickInCharacterSelectionState()
        {
            _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.IsUnselected);

            Vector3Int coordinates = _selectedCharacter.Coordinates;
            SlotController targetSlot = _selectedCharacter.Board.Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z].SlotView.Controller;
            if (targetSlot.Data.Obstacle.Has || targetSlot.Data.Character != null || targetSlot.Data.Type != SlotType.Base)
            {
                return;
            }

            _selectedCharacter.OnCharacterAction.Invoke(CharacterAction.MoveTo, GameManager.Instance.Board.CurrentHoveredLocation.Coordinates);

            ResetSelection();
        }
    }
}