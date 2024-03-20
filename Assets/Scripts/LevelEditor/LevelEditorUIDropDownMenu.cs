using System;
using System.Collections.Generic;
using Inputs;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the drop down menu of the level editor selection
    /// </summary>
    public class LevelEditorUIDropDownMenu : MonoBehaviour
    {
        [SerializeField] private LevelEditorUIDropDownButton _dropDownButtonPrefab;
        [SerializeField] private RectTransform _menuRectTransform;
        [SerializeField] private Transform _layout;

        private const float BackgroundSizePerButton = 109f;

        private bool _isActive;
        private List<LevelEditorUIDropDownButton> _currentButtons = new List<LevelEditorUIDropDownButton>();
        private SlotData _copiedSlotData;
        private SlotLocation _currentLocationSelected;

        private void Awake()
        {
            SetMenuSize();
        }

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnRightClick += ClearDropDownMenu;
            InputManager.Instance.LevelEditorInput.OnClickTap += ClearDropDownMenu;
            InputManager.Instance.LevelEditorInput.OnCameraZoomed += context =>  ClearDropDownMenu();
            InputManager.Instance.LevelEditorInput.OnCameraMoveButtonPressed += context =>  ClearDropDownMenu();
        }

        public void CreateDropDownMenu(SlotView slotView)
        {
            _isActive = true;
            _currentLocationSelected = GetViewLocation(slotView);
            _currentLocationSelected.SetSelected(true);
            
            LevelEditorUIDropDownButton copyButton = Instantiate(_dropDownButtonPrefab, _layout, true);
            copyButton.Initialize(() => Copy(slotView), "Copy", slotView != null);

            LevelEditorUIDropDownButton cutButton = Instantiate(_dropDownButtonPrefab, _layout, true);
            cutButton.Initialize(() => Cut(slotView), "Cut", slotView != null);

            LevelEditorUIDropDownButton pastButton = Instantiate(_dropDownButtonPrefab, _layout, true);
            pastButton.Initialize(() => Paste(slotView), "Paste", _copiedSlotData != null);

            _currentButtons.Add(copyButton);
            _currentButtons.Add(cutButton);
            _currentButtons.Add(pastButton);

            _menuRectTransform.position = Input.mousePosition + new Vector3(50, 0, 0);
            
            SetMenuSize();
        }

        public void CreateDropDownMenu(SlotLocation slotLocation)
        {
            CreateDropDownMenu(slotLocation.SlotView);
        }

        private void ClearDropDownMenu()
        {
            if (_isActive == false)
            {
                return;
            }
            _isActive = false;
            
            foreach (LevelEditorUIDropDownButton button in _currentButtons)
            {
                Destroy(button.gameObject);
            }
            _currentButtons.Clear();
            
            SetMenuSize();
            
            _currentLocationSelected.SetSelected(false);
            _currentLocationSelected = null;
        }

        private void SetMenuSize()
        {
            float size = _currentButtons.Count * BackgroundSizePerButton;
            _menuRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
        
        private SlotLocation GetViewLocation(SlotView view)
        {
            Vector3Int coordinates = view.Controller.Coordinates;
            return LevelEditorManager.Instance.Board.Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
        }

        #region Copy / Cut / Paste slot view actions

        private void Copy(SlotView view)
        {
            if (view.Controller == null)
            {
                return;
            }
            _copiedSlotData = view.Controller.Data;
            ClearDropDownMenu();
        }

        private void Cut(SlotView view)
        {
            Copy(view);
            GetViewLocation(view).DestroySlotViewOnLocation();
            ClearDropDownMenu();
        }

        private void Paste(SlotLocation location)
        {
            if (_copiedSlotData == null)
            {
                return;
            }

            if (location.SlotView != null)
            {
                location.DestroySlotViewOnLocation();
            }
            
            LevelEditorManager.Instance.Board.CreateSlotAt(location.Coordinates, _copiedSlotData);
            ClearDropDownMenu();
        }

        private void Paste(SlotView view)
        {
            Paste(GetViewLocation(view));
        }

        #endregion
    }
}