using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class manage the load menu of the level editor
    /// </summary>
    public class LevelEditorUILoadMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private Button LoadButton;
        [SerializeField, Required] private LevelToLoadButtonController LevelToLoadButtonPrefab;
        [SerializeField, Required] private LayoutGroup LevelToLoadLayout;
        [SerializeField, Required] private TMP_InputField SearchInputField;
    }
}