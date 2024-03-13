using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorUIActionButtons : MonoBehaviour
    {
        [SerializeField, Required] private Button _selectionButton;
        [SerializeField, Required] private Button _paintButton;
    }
}