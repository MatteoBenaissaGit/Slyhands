using System;
using System.Collections.Generic;
using DG.Tweening;
using LevelEditor.ActionButtons;
using LevelEditor.LoadAndSave;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    [Flags]
    public enum EditorMode
    {
        None = 0,
        
        Inactive = 1,
        BasicEditor = 2,
        SetRoadForCharacter = 4,
        
        All = Inactive | BasicEditor | SetRoadForCharacter
    }

    /// <summary>
    /// This class handle all the ui elements of the level editing
    /// </summary>
    public class LevelEditorUI : MonoBehaviour
    {
        [field:SerializeField] [field:BoxGroup("Action Buttons")] [field:Required]
        public LevelEditorInputActionsManager InputActionsManager { get; private set; }
        [field:SerializeField] [field:BoxGroup("Action Buttons")] [field:Required]
        public LevelEditorDropDownMenuController DropDownMenuController { get; private set; }
        [field:SerializeField] [field:BoxGroup("Other")] [field:Required]
        public LevelEditorUIShortcuts Shortcuts { get; private set; }
        [field:SerializeField] [field:BoxGroup("Menus")] [field:Required]
        public LevelEditorUISetTeamMenu SetTeamMenu { get; private set; }
        
        [SerializeField, BoxGroup("Menus"), Required]
        private LevelEditorUILoadMenu _loadMenu;
        [SerializeField, BoxGroup("Menus"), Required]
        private LevelEditorUISaveMenu _saveMenu;
        [SerializeField, BoxGroup("Menus"), Required]
        private LevelEditorUICreateBoardMenu _createNewBoardMenu;

        [SerializeField, BoxGroup("Buttons"), Required]
        private Button _saveButton;
        [SerializeField, BoxGroup("Buttons"), Required]
        private Button _loadButton;
        [SerializeField, BoxGroup("Buttons"), Required]
        private Button _createNewBoardButton;
        [SerializeField, BoxGroup("Buttons"), Required]
        private Button _exitSetRoadModeButton;
        [SerializeField, BoxGroup("Buttons"), Required]
        private Button _changeRoadModeButton;
        
        [SerializeField, BoxGroup("Other"), Required]
        private LevelEditorUIHeightSlider _heightSlider;

        public EditorMode CurrentMode { get; private set; }
        
        private List<LevelEditorUIMenu> _menus;
        private LevelEditorUIMenu _currentMenu;

        private void Start()
        {
            _menus = new List<LevelEditorUIMenu>()
            {
                _loadMenu,
                _saveMenu,
                _createNewBoardMenu,
                SetTeamMenu
            };
            
            _menus.ForEach(x => x.CanvasGroup.alpha = 0);
            _menus.ForEach(x => x.CanvasGroup.interactable = false);
            _menus.ForEach(x => x.CanvasGroup.blocksRaycasts = false);
            
            _saveButton.onClick.AddListener(() => ShowMenu(_saveMenu));
            _loadButton.onClick.AddListener(() => ShowMenu(_loadMenu));
            _createNewBoardButton.onClick.AddListener(() => ShowMenu(_createNewBoardMenu));
            _exitSetRoadModeButton.onClick.AddListener(() => SetMode(EditorMode.BasicEditor));
            _changeRoadModeButton.onClick.AddListener(() => LevelEditorManager.Instance.RoadModeManager.ChangeRoadMode(_changeRoadModeButton));

            _heightSlider.SetSlider(false);
            
            SetMode(EditorMode.BasicEditor);
        }

        private void OnDestroy()
        {
            _saveButton.onClick.RemoveListener(() => ShowMenu(_saveMenu));
            _loadButton.onClick.RemoveListener(() => ShowMenu(_loadMenu));
            _createNewBoardButton.onClick.RemoveListener(() => ShowMenu(_createNewBoardMenu));
            _exitSetRoadModeButton.onClick.RemoveListener(() => SetMode(EditorMode.BasicEditor));
            _changeRoadModeButton.onClick.RemoveListener(() => LevelEditorManager.Instance.RoadModeManager.ChangeRoadMode(_changeRoadModeButton));
        }

        public void SetMode(EditorMode mode)
        {
            if (mode == CurrentMode)
            {
                return;
            }

            ExitMode(CurrentMode);
            CurrentMode = mode;
            StartMode(CurrentMode);
        }

        private void StartMode(EditorMode mode)
        {
            switch (mode)
            {
                case EditorMode.Inactive:
                    break;
                case EditorMode.BasicEditor:
                    _loadButton.gameObject.SetActive(true);
                    _saveButton.gameObject.SetActive(true);
                    _createNewBoardButton.gameObject.SetActive(true);
                    break;
                case EditorMode.SetRoadForCharacter:
                    _exitSetRoadModeButton.gameObject.SetActive(true);
                    _changeRoadModeButton.gameObject.SetActive(true);
                    break;
            }
            
            InputActionsManager.SetButtonsForEditorMode(mode);
        }
        
        private void ExitMode(EditorMode mode)
        {
            switch (mode)
            {
                case EditorMode.Inactive:
                    break;
                case EditorMode.BasicEditor:
                    _loadButton.gameObject.SetActive(false);
                    _saveButton.gameObject.SetActive(false);
                    _createNewBoardButton.gameObject.SetActive(false);
                    break;
                case EditorMode.SetRoadForCharacter:
                    _exitSetRoadModeButton.gameObject.SetActive(false);
                    _changeRoadModeButton.gameObject.SetActive(false);
                    LevelEditorManager.Instance.RoadModeManager.ExitMode();
                    break;
            }
        }
        
        #region Menus

        /// <summary>
        /// Show a menu deriving from LevelEditorUIMenu
        /// </summary>
        /// <param name="menu">The menu to show</param>
        public void ShowMenu(LevelEditorUIMenu menu)
        {
            HideMenu();
            
            menu.transform.DOKill();
            menu.transform.localScale = new Vector3(0, 1, 1);
            menu.transform.DOScale(new Vector3(1, 1, 1), 0.1f);

            menu.CanvasGroup.alpha = 1f;
            menu.CanvasGroup.interactable = true;
            menu.CanvasGroup.blocksRaycasts = true;

            _currentMenu = menu;
            _currentMenu.OpenMenu();
        }
        
        /// <summary>
        /// Hide the current displayed menu
        /// </summary>
        public void HideMenu()
        {
            if (_currentMenu == null)
            {
                return;
            }
            
            _currentMenu.CloseMenu();
            _currentMenu.transform.DOKill();
            _currentMenu.transform.DOScale(new Vector3(0, 1, 1), 0.2f)
                .OnComplete(() => _currentMenu.CanvasGroup.alpha = 1f)
                .OnComplete(() => _currentMenu.CanvasGroup.interactable = true)
                .OnComplete(() => _currentMenu.CanvasGroup.blocksRaycasts = true);
        }

        public void SetHeightSlider(int height)
        {
            _heightSlider.SetSlider(height > 1);
        }

        #endregion
    }
}
