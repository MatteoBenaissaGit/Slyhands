using System;
using System.Collections.Generic;
using DG.Tweening;
using LevelEditor.ActionButtons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class handle all the ui elements of the level editing
    /// </summary>
    public class LevelEditorUI : MonoBehaviour
    {
        [field:SerializeField] [field:BoxGroup("Action Buttons")] [field:Required]
        public LevelEditorUIActionButtons ActionButtons { get; private set; }
        [field:SerializeField] [field:BoxGroup("Action Buttons")] [field:Required]
        public LevelEditorDropDownMenuController DropDownMenuController { get; private set; }
        
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
        
        [SerializeField, BoxGroup("Other"), Required]
        private LevelEditorUIHeightSlider _heightSlider;

        private List<LevelEditorUIMenu> _menus;
        private LevelEditorUIMenu _currentMenu;

        private void Start()
        {
            _menus = new List<LevelEditorUIMenu>()
            {
                _loadMenu,
                _saveMenu,
                _createNewBoardMenu
            };
            
            _menus.ForEach(x => x.CanvasGroup.alpha = 0);
            _menus.ForEach(x => x.CanvasGroup.interactable = false);
            _menus.ForEach(x => x.CanvasGroup.blocksRaycasts = false);
            
            _saveButton.onClick.AddListener(() => ShowMenu(_saveMenu));
            _loadButton.onClick.AddListener(() => ShowMenu(_loadMenu));
            _createNewBoardButton.onClick.AddListener(() => ShowMenu(_createNewBoardMenu));

            _heightSlider.SetSlider(false);
        }

        private void OnDestroy()
        {
            _saveButton.onClick.RemoveListener(() => ShowMenu(_saveMenu));
            _loadButton.onClick.RemoveListener(() => ShowMenu(_loadMenu));
            _createNewBoardButton.onClick.RemoveListener(() => ShowMenu(_createNewBoardMenu));
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
            
            _currentMenu.transform.DOKill();
            _currentMenu.transform.DOScale(new Vector3(0, 1, 1), 0.2f)
                .OnComplete(() => _currentMenu.CanvasGroup.alpha = 1f)
                .OnComplete(() => _currentMenu.CanvasGroup.interactable = true)
                .OnComplete(() => _currentMenu.CanvasGroup.blocksRaycasts = true);
        }

        public void CreateNewBoard(int width, int height, int length)
        {
            LevelEditorManager.Instance.Board.CreateBlankBoard(width,length,height);
            _heightSlider.SetSlider(height > 1);
        }

        #endregion
    }
}
