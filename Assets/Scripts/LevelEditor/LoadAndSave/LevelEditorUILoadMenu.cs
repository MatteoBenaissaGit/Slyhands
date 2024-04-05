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
        
        protected override void Awake()
        {
            base.Awake();
            _loadButton.onClick.AddListener(LoadLevel);
        }

        public override void OpenMenu()
        {
            ShowSavedLevels();
        }

        public override void CloseMenu()
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
            LevelEditorManager.Instance.UI.HideMenu(); //TODO doesnt hide ???
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
    }
}