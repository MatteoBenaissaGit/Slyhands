using System;
using System.Collections.Generic;
using Inputs;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class manage all the action buttons in the level editor
    /// </summary>
    public class LevelEditorActionButtonsManager : MonoBehaviour
    {
        [SerializeField] private List<LevelEditorActionButtonController> _buttons = new List<LevelEditorActionButtonController>();

        private SlotLocation _currentHoveredLocation => LevelEditorManager.Instance.CurrentHoveredLocation;
        
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
            InputManager.Instance.LevelEditorInput.OnClickTap += ClickTapAction;
            InputManager.Instance.LevelEditorInput.OnRightClick += RightClickAction;
            InputManager.Instance.LevelEditorInput.OnClickHold += (bool doHold) => _isHoldingClick = doHold;
            InputManager.Instance.LevelEditorInput.OnRightClickHold += (bool doHold) => _isHoldingRightClick = doHold;
        }

        private void Update()
        {
            SlotLocation location = GetClickedSlotLocation();
            if (location != _currentHoveredLocation)
            {
                LevelEditorManager.Instance.CurrentHoveredLocation = location;
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
        }

        /// <summary>
        /// Set the default action button as current
        /// </summary>
        public void SetCurrentButton()
        {
            SetCurrentButton(_buttons[0]);
        }

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
                    LevelEditorManager.Instance.CurrentSelectedLocation = _currentHoveredLocation;
                    break;
                case LevelEditorActionButtonType.Paint:
                    LevelEditorManager.Instance.Board.CreateSlotAt(_currentHoveredLocation.Coordinates);
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    LevelEditorActionButtonControllerExtended extendedActionButton = _currentButton as LevelEditorActionButtonControllerExtended;
                    _currentHoveredLocation.SlotView?.SetObstacle(extendedActionButton?.CurrentChoice);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle the action made when the user right click 
        /// </summary>
        private void RightClickAction()
        {
            if (_currentButton == null || _currentHoveredLocation == null)
            {
                CheckForDeselectionOfCurrentSlot();
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    LevelEditorManager.Instance.UI.DropDownMenuController.CreateDropDownMenu(_currentHoveredLocation);
                    break;
                case LevelEditorActionButtonType.Paint:
                    _currentHoveredLocation.DestroySlotViewOnLocation();
                    break;
                case LevelEditorActionButtonType.AddObstacle:
                    _currentHoveredLocation.SlotView?.SetObstacle(null);
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
                    LevelEditorManager.Instance.Board.CreateSlotAt(_currentHoveredLocation.Coordinates);
                    break;
                case LevelEditorActionButtonType.AddObstacle:
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
                    || location.IsEditable == false)
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
            if (LevelEditorManager.Instance.CurrentSelectedLocation == null)
            {
                return;
            }
            
            if (_currentHoveredLocation == null && EventSystem.current.IsPointerOverGameObject() == false)
            {
                LevelEditorManager.Instance.CurrentSelectedLocation = null;
            }
        }
    }
}