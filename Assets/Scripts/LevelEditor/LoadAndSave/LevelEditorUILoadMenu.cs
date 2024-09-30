using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.LoadAndSave
{
    /// <summary>
    /// This class manage the load menu of the level editor
    /// </summary>
    public class LevelEditorUILoadMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private Button _loadButton;
        [SerializeField, Required] private LevelToLoadButtonController _levelToLoadButtonPrefab;
        [SerializeField, Required] private TMP_InputField _searchInputField;
        [SerializeField, Required] private RectTransform _content;

        private LevelToLoadButtonController _selectedLevelButton;
        private string _lastLoadedLevel;
        
        protected override void Awake()
        {
            base.Awake();
            _loadButton.onClick.AddListener(LoadLevel);
            _searchInputField.onValueChanged.AddListener(input => FilterSaves(input));
        }

        public override void OnMenuOpened()
        {
            ShowSavedLevels();
        }

        public override void OnMenuClosed()
        {
            for (int i = 0; i < _content.childCount; i++)
            {
                GameObject child =  _content.GetChild(i).gameObject;
                if (child.TryGetComponent(out LevelToLoadButtonController loadButton))
                {
                    loadButton.GetButton().onClick.RemoveListener(() => SetSelectedLevel(loadButton));
                }
                Destroy(child);
            }
        }

        private void LoadLevel()
        {
            if (_selectedLevelButton == null)
            {
                return;
            }
            
            LevelEditorManager.Instance.Board.LoadBoardFromLevelData(_selectedLevelButton.Data);
            LevelEditorManager.Instance.UI.HideMenu(); //TODO dont hide ???
            
            _lastLoadedLevel = _selectedLevelButton.Data.Name;
        }

        /// <summary>
        /// Reset the last loaded level info, used when a new board is created to ensure it's save doesn't overwrite the last loaded level
        /// </summary>
        public void ResetLastLoadedLevel()
        {
            _lastLoadedLevel = string.Empty;
        }

        /// <summary>
        /// Update the save of the last loaded level
        /// </summary>
        public void SaveLastLoadedLevel()
        {
            if (string.IsNullOrEmpty(_lastLoadedLevel))
            {
                LevelEditorManager.Instance.UI.ShowMenu(LevelEditorManager.Instance.UI.SaveMenu);
                return;
            }
            
            var lastLoadedLevel = LevelEditorManager.Instance.SaveLoadManager.GetLevelsData().Datas.Find(x => x.Name == _lastLoadedLevel);
            if (lastLoadedLevel == null)
            {
                return;
            }
            LevelEditorManager.Instance.SaveLoadManager.SaveLevelData(LevelEditorManager.Instance.Board.Data, _lastLoadedLevel);
        }

        private void ShowSavedLevels()
        {
            List<LevelData> levelDataList = LevelEditorManager.Instance.SaveLoadManager.GetLevelsData().Datas;
            foreach (LevelData levelData in levelDataList)
            {
                LevelToLoadButtonController levelButton = Instantiate(_levelToLoadButtonPrefab, _content);
                levelButton.transform.localPosition = Vector3.zero;
                levelButton.Initialize(levelData).onClick.AddListener(() => SetSelectedLevel(levelButton));
            }

            float contentHeight = (_levelToLoadButtonPrefab.Rect.rect.height + 40) * levelDataList.Count;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        }

        private void SetSelectedLevel(LevelToLoadButtonController levelButtonSelected)
        {
            _selectedLevelButton?.SetSelected(false);
            _selectedLevelButton = levelButtonSelected;
            _selectedLevelButton?.SetSelected(true);
        }
        
        private void FilterSaves(string filter)
        {
            for (int i = 0; i < _content.childCount; i++)
            {
                GameObject child =  _content.GetChild(i).gameObject;
                if (child.TryGetComponent(out LevelToLoadButtonController loadButton))
                {
                    loadButton.gameObject.SetActive(loadButton.Data.Name.Contains(filter));
                }
            }
        }
    }
}