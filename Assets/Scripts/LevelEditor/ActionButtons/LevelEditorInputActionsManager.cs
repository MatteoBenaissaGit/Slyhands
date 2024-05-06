using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Inputs;
using LevelEditor.Entities;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class manage all the action buttons in the level editor
    /// </summary>
    public class LevelEditorInputActionsManager : MonoBehaviour
    {
        [field:Required] [field:SerializeField] public LevelEditorActionPreview Preview { get; private set; }
        
        public Orientation CurrentPlacingOrientation { get; private set; }
        
        [SerializeField] private List<LevelEditorActionButtonController> _buttons = new List<LevelEditorActionButtonController>();

        private SlotLocation _currentHoveredLocation => LevelEditorManager.Instance.Board.CurrentHoveredLocation;
        
        private LevelEditorActionButtonController _currentButton;
        private bool _isHoldingClick;
        private bool _isHoldingRightClick;
        private RaycastHit[] _mouseClickRaycastHits;

        private void Awake()
        {
            _mouseClickRaycastHits = new RaycastHit[32];

            _buttons.ForEach(x => x.Button.onClick.AddListener(() => SetCurrentButton(x)));

            _currentButton = _buttons[0];
            _currentButton.SetSelected(true, true);
        }

        private void OnDestroy()
        {
            _buttons.ForEach(x => x.Button.onClick.RemoveListener(() => SetCurrentButton(x)));
        }

        private void Start()
        {
            LevelEditorManager.Instance.UI.Shortcuts.SetShortcuts(_currentButton.Type);

            InputManager.Instance.LevelEditorInput.OnClickTap += ClickTapAction;
            InputManager.Instance.LevelEditorInput.OnRightClick += RightClickAction;
            
            InputManager.Instance.LevelEditorInput.OnClickHold += (bool doHold) => _isHoldingClick = doHold;
            InputManager.Instance.LevelEditorInput.OnRightClickHold += (bool doHold) => _isHoldingRightClick = doHold;
            
            InputManager.Instance.LevelEditorInput.OnRotation += ChangeRotation;
        }

        private void Update()
        {
            SlotLocation location = GetClickedSlotLocation();
            if (location != _currentHoveredLocation)
            {
                LevelEditorManager.Instance.Board.CurrentHoveredLocation = location;
                Preview.UpdatePreview(location);
            }

            if (_isHoldingClick)
            {
                ClickHoldAction();
            }
            else if (_isHoldingRightClick)
            {
                RightClickHoldAction();
            }
        }

        /// <summary>
        /// Set the current used button
        /// </summary>
        /// <param name="buttonToSet">The button to set as current</param>
        public void SetCurrentButton(LevelEditorActionButtonController buttonToSet)
        {
            _currentButton.SetSelected(false);
            _currentButton = buttonToSet;
            _currentButton.SetSelected(true);
            LevelEditorManager.Instance.UI.Shortcuts.SetShortcuts(_currentButton.Type);

            LevelEditorManager.Instance.Board.CurrentSelectedLocation = null;
        }

        /// <summary>
        /// Set the default action button as current
        /// </summary>
        public void SetCurrentButton()
        {
            SetCurrentButton(_buttons[0]);
        }

        #region Click actions

        /// <summary>
        /// Handle the action made when the user tap click 
        /// </summary>
        private void ClickTapAction()
        {
            if (_currentButton == null || _currentHoveredLocation == null)
            {
                CheckForDeselectionOfCurrentSlot();
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    LevelEditorManager.Instance.Board.CurrentSelectedLocation = _currentHoveredLocation;
                    break;
                case LevelEditorActionButtonType.Paint:
                    PaintCurrentHoveredSlot();
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    AddObstacleOnCurrentHoveredSlot();
                    break;
                case LevelEditorActionButtonType.AddCharacter:
                    AddEntityOnCurrentHoveredSlot();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Handle the action made when the user right click 
        /// </summary>
        private async void RightClickAction()
        {
            if (_currentButton == null || _currentHoveredLocation == null)
            {
                CheckForDeselectionOfCurrentSlot();
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    await Task.Delay(1);
                    LevelEditorManager.Instance.UI.DropDownMenuController.CreateDropDownMenu(_currentHoveredLocation);
                    break;
                case LevelEditorActionButtonType.Paint:
                    _currentHoveredLocation.DestroySlotViewOnLocation();
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    _currentHoveredLocation.SlotView?.CreateObstacle(null);
                    break;
                case LevelEditorActionButtonType.AddCharacter:
                    _currentHoveredLocation.SlotView?.CreateCharacterOnSlot(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle the action made when the user hold click
        /// </summary>
        private void ClickHoldAction()
        {
            if (_currentButton == null || _currentHoveredLocation == null)
            {
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    break;
                case LevelEditorActionButtonType.Paint:
                    PaintCurrentHoveredSlot();
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    AddObstacleOnCurrentHoveredSlot();
                    break;
                case LevelEditorActionButtonType.AddCharacter:
                    AddEntityOnCurrentHoveredSlot();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle the action made when the user hold right click
        /// </summary>
        private void RightClickHoldAction()
        {
            if (_currentButton == null || _currentHoveredLocation == null)
            {
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    break;
                case LevelEditorActionButtonType.Paint:
                    _currentHoveredLocation.DestroySlotViewOnLocation();
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    _currentHoveredLocation.SlotView?.CreateObstacle(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion

        /// <summary>
        /// Get the slot location under the current mouse when the user clicked
        /// </summary>
        /// <returns>The slot location clicked on</returns>
        private SlotLocation GetClickedSlotLocation()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return null;
            }
            
            Ray ray = LevelEditorManager.Instance.Camera.Camera.ScreenPointToRay(Input.mousePosition);
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
        /// Check if it is needed to deselect the current selected location
        /// </summary>
        private void CheckForDeselectionOfCurrentSlot()
        {
            if (LevelEditorManager.Instance.Board.CurrentSelectedLocation == null)
            {
                return;
            }
            
            if (_currentHoveredLocation == null && EventSystem.current.IsPointerOverGameObject() == false)
            {
                LevelEditorManager.Instance.Board.CurrentSelectedLocation = null;
            }
        }

        #region Actions

        /// <summary>
        /// Add the selected entity of the entity add button on the current slot
        /// </summary>
        private async void AddEntityOnCurrentHoveredSlot()
        {
            if (_currentHoveredLocation.SlotView == null)
            {
                return;
            }
            
            LevelEditorActionButtonControllerExtended entityButton = _currentButton as LevelEditorActionButtonControllerExtended;
            GameObject entityToAdd = entityButton?.CurrentChoice;
            LevelEditorCharacter characterInstantiated = _currentHoveredLocation.SlotView.CreateCharacterOnSlot(entityToAdd);

            if (characterInstantiated != null)
            {
                characterInstantiated.SetCharacterOrientation(CurrentPlacingOrientation);
            }
        }

        /// <summary>
        /// Add the selected slot of the paint button on the current slot
        /// </summary>
        private void PaintCurrentHoveredSlot()
        {
            LevelEditorActionButtonControllerExtended paintButton = _currentButton as LevelEditorActionButtonControllerExtended;
            string slotTypeId = paintButton?.CurrentChoiceID;
            
            Vector3Int coordinates = _currentHoveredLocation.Coordinates;
            BoardController board = LevelEditorManager.Instance.Board;
            board.CreateSlotAt(coordinates).view
                .SetSlotTypeReference(slotTypeId)
                .SetSlotOrientation(CurrentPlacingOrientation);
            
            //if there is a character or an obstacle on the slot under the painted slot, destroy it
            if (coordinates.y - 1 >= 0 && board.Data.SlotLocations[coordinates.x, coordinates.y - 1, coordinates.z].SlotView != null)
            {
                board.Data.SlotLocations[coordinates.x, coordinates.y - 1, coordinates.z].SlotView.DestroyObstacleOnSlot();
                board.Data.SlotLocations[coordinates.x, coordinates.y - 1, coordinates.z].SlotView.DestroyCharacterOnSlot();
            }
        }
        
        /// <summary>
        /// Add the selected obstacle of the obstacle add button on the current slot
        /// </summary>
        private void AddObstacleOnCurrentHoveredSlot()
        {
            LevelEditorActionButtonControllerExtended obstacleButton = _currentButton as LevelEditorActionButtonControllerExtended;
            _currentHoveredLocation.SlotView?.CreateObstacle(obstacleButton?.CurrentChoice);
            _currentHoveredLocation.SlotView?.SetObstacleOrientation(CurrentPlacingOrientation);
        }
        
        #endregion

        /// <summary>
        /// Change the rotation variable to the next orientation
        /// </summary>
        private void ChangeRotation()
        {
            Orientation newOrientation = CurrentPlacingOrientation switch
            {
                Orientation.North => Orientation.East,
                Orientation.East => Orientation.South,
                Orientation.South => Orientation.West,
                Orientation.West => Orientation.North,
                _ => throw new ArgumentOutOfRangeException()
            };

            CurrentPlacingOrientation = newOrientation;
            Preview.SetOrientation(CurrentPlacingOrientation);
        }
    }
}