﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inputs;
using LevelEditor.Entities;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the drop down menu of the level editor selection
    /// </summary>
    public class LevelEditorDropDownMenuController : MonoBehaviour
    {
        [SerializeField] private LevelEditorUIDropDownButton _dropDownButtonPrefab;
        [SerializeField] private RectTransform _menuRectTransform;
        [SerializeField] private Transform _layout;

        private const float BackgroundSizePerButton = 62f;

        private bool _isActive;
        private List<LevelEditorUIDropDownButton> _currentButtons = new List<LevelEditorUIDropDownButton>();
        private SlotData _copiedSlotData;

        private void Awake()
        {
            SetMenuSize();
        }

        private void Start()
        {
            _copiedSlotData = null;
            
            InputLevelEditor levelEditorInput = InputManager.Instance.LevelEditorInput;
            
            levelEditorInput.OnRightClick += RightClickTapCheckToClearDropDownMenu;
            levelEditorInput.OnLeftClick += ClickTapCheckToClearDropDownMenu;
            levelEditorInput.OnCameraZoomed += context =>  ClearDropDownMenu();
            levelEditorInput.OnCameraMoveButtonPressed += context =>  ClearDropDownMenu();

            levelEditorInput.OnControlShortcut += ShortcutAction;
        }

        /// <summary>
        /// Make a shortcut action (copy / paste / cut)
        /// </summary>
        /// <param name="shortcutAction">the shortcut action to execute</param>
        private void ShortcutAction(InputLevelEditor.ControlShortcutAction shortcutAction)
        {
            if (shortcutAction == InputLevelEditor.ControlShortcutAction.Save)
            {
                LevelEditorManager.Instance.UI.LoadMenu.SaveLastLoadedLevel();
                return;
            }
            
            SlotLocation currentLocation = LevelEditorManager.Instance.Board.CurrentSelectedLocation;
            switch (shortcutAction)
            {
                case InputLevelEditor.ControlShortcutAction.Copy:
                    Copy(currentLocation.SlotView);
                    break;
                case InputLevelEditor.ControlShortcutAction.Paste:
                    Paste(currentLocation);
                    break;
                case InputLevelEditor.ControlShortcutAction.Cut:
                    Cut(currentLocation.SlotView);
                    break;
            }
        }
        
        /// <summary>
        /// Create the dropdown menu on a defined slot location 
        /// </summary>
        /// <param name="slotCoordinate">the coordinates of the location</param>
        public void CreateDropDownMenu(Vector3Int slotCoordinate)
        {
            _isActive = true;
            LevelEditorManager.Instance.Board.CurrentSelectedLocation = 
                LevelEditorManager.Instance.Board.Data.SlotLocations[slotCoordinate.x, slotCoordinate.y, slotCoordinate.z];

            SlotLocation currentLocationSelected = LevelEditorManager.Instance.Board.CurrentSelectedLocation;
            SlotView slotView = currentLocationSelected.SlotView;
            
            if (LevelEditorManager.Instance.UI.CurrentMode == EditorMode.BasicEditor)
            {
                // copy / paste / cut buttons
                LevelEditorUIDropDownButton copyButton = Instantiate(_dropDownButtonPrefab, _layout, true);
                copyButton.Initialize(() => Copy(slotView), "Copy", slotView != null);
                _currentButtons.Add(copyButton);

                LevelEditorUIDropDownButton cutButton = Instantiate(_dropDownButtonPrefab, _layout, true);
                cutButton.Initialize(() => Cut(slotView), "Cut", slotView != null);
                _currentButtons.Add(cutButton);

                LevelEditorUIDropDownButton pastButton = Instantiate(_dropDownButtonPrefab, _layout, true);
                pastButton.Initialize(() => Paste(currentLocationSelected), "Paste", _copiedSlotData != null);
                _currentButtons.Add(pastButton);
                
                // character related buttons
                if (slotView != null && slotView.Controller.Data.LevelEditorCharacter.Has)
                {
                    // set team button
                    LevelEditorUIDropDownButton setCharacterTeamButton = Instantiate(_dropDownButtonPrefab, _layout, true);
                    setCharacterTeamButton.Initialize(() => OpenSetTeamMenuForCharacter(slotView.LevelEditorCharacterOnSlot), "Set team");
                    _currentButtons.Add(setCharacterTeamButton);
                
                    // set road mode button
                    LevelEditorUIDropDownButton setRoadModeButton = Instantiate(_dropDownButtonPrefab, _layout, true);
                    setRoadModeButton.Initialize(() => StartSetRoadModeForCharacter(slotView.LevelEditorCharacterOnSlot), "Set road");
                    _currentButtons.Add(setRoadModeButton);
                }
            }

            _menuRectTransform.position = Input.mousePosition + new Vector3(25, 0, 0);
            
            SetMenuSize();

            if (_currentButtons.Count <= 0)
            {
                ClearDropDownMenu();
            }
        }

        /// <summary>
        /// Create the dropdown menu on a defined slot location 
        /// </summary>
        /// <param name="slotLocation">the slot to create the menu on</param>
        public void CreateDropDownMenu(SlotLocation slotLocation)
        {
            CreateDropDownMenu(slotLocation.Coordinates);
        }

        /// <summary>
        /// Remove the dropdown menu
        /// </summary>
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
            
            LevelEditorManager.Instance.Board.CurrentSelectedLocation = null;
        }

        /// <summary>
        /// Check if when the user click it needs to remove the dropdown menu or not
        /// </summary>
        private void ClickTapCheckToClearDropDownMenu()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            ClearDropDownMenu();
        }
        
        /// <summary>
        /// Check if when the user right click it needs to remove the dropdown menu or not
        /// </summary>
        private void RightClickTapCheckToClearDropDownMenu()
        {
            if (EventSystem.current.IsPointerOverGameObject() || _isActive == false)
            {
                return;
            }
            ClearDropDownMenu();
        }
        
        /// <summary>
        /// Set the menu size depending on the number of actions in it
        /// </summary>
        private void SetMenuSize()
        {
            float size = _currentButtons.Count * BackgroundSizePerButton;
            _menuRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
        
        /// <summary>
        /// Get the slot location of a slot view 
        /// </summary>
        /// <param name="view">the view to get the location of</param>
        /// <returns>the slot location of the given slot view</returns>
        private SlotLocation GetViewLocation(SlotView view)
        {
            if (view == null)
            {
                return null;
            }
            Vector3Int coordinates = view.Controller.Coordinates;
            return LevelEditorManager.Instance.Board.Data.SlotLocations[coordinates.x, coordinates.y, coordinates.z];
        }
        
        #region Copy / Cut / Paste slot view actions

        /// <summary>
        /// Copy and temporary save the data of a slot view
        /// </summary>
        /// <param name="view">the view to copy</param>
        private void Copy(SlotView view)
        {
            ClearDropDownMenu();

            if (view == null || view.Controller == null)
            {
                return;
            }
            _copiedSlotData = view.Controller.Data;
        }

        /// <summary>
        /// Cut a defined view by copying its data and destroying it after
        /// </summary>
        /// <param name="view">the slot view to cut</param>
        private void Cut(SlotView view)
        {
            ClearDropDownMenu();
            Copy(view);
            GetViewLocation(view)?.DestroySlotViewOnLocation();
        }

        /// <summary>
        /// Paste the copied slot view on a specified slot location, destroying the current slot view on it if necessary
        /// </summary>
        /// <param name="location">The slot location to copy the view on</param>
        private void Paste(SlotLocation location)
        {
            if (_copiedSlotData == null)
            {
                return;
            }

            if (LevelEditorManager.Instance.Board.CurrentSelectedLocation.SlotView != null)
            {
                location.DestroySlotViewOnLocation();
            }
            
            LevelEditorManager.Instance.Board.CreateSlotAt(location.Coordinates, _copiedSlotData);
            ClearDropDownMenu();
        }

        #endregion

        private void OpenSetTeamMenuForCharacter(LevelEditorCharacter character)
        {
            ClearDropDownMenu();
            
            LevelEditorManager.Instance.UI.SetTeamMenu.OpenMenu(character);
            LevelEditorManager.Instance.UI.ShowMenu(LevelEditorManager.Instance.UI.SetTeamMenu);
        }

        private void StartSetRoadModeForCharacter(LevelEditorCharacter character)
        {
            ClearDropDownMenu();

            LevelEditorManager.Instance.UI.SetMode(EditorMode.SetRoadForCharacter);
            LevelEditorManager.Instance.RoadModeManager.StartMode(character);
        }
    }
}