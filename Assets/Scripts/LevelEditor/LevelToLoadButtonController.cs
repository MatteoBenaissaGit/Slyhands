using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the button used to select a level to load in the load menu
    /// </summary>
    public class LevelToLoadButtonController : MonoBehaviour
    {
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private TMP_Text _nameText;
    }
}
