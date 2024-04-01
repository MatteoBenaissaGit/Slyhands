using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the level editor saving menu
    /// </summary>
    public class LevelEditorUISaveMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private Button _saveButton;
        [SerializeField, Required] private TMP_InputField _saveNameInputField;

        private void OnEnable()
        {
            base.Awake();

            _saveButton.onClick.AddListener(SaveLevel);
        }

        private void OnDisable()
        {
            _saveButton.onClick.RemoveListener(SaveLevel);
        }

        private void SaveLevel()
        {
            LevelEditorManager.Instance.SaveLoadManager.SaveLevelData(LevelEditorManager.Instance.Board.Data, _saveNameInputField.text);
            LevelEditorManager.Instance.UI.HideMenu();
        }
    }
}