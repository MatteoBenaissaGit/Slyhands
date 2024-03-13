using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorUISaveMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private Button _saveButton;
        [SerializeField, Required] private TMP_InputField _saveNameInputField;
    }
}