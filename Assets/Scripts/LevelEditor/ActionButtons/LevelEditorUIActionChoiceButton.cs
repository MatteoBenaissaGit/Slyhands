using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class handle the choice button of an extended action button in the level editor
    /// </summary>
    public class LevelEditorUIActionChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _choiceObject;
        
        /// <summary>
        /// Initialize the button for the desired action button extended
        /// </summary>
        /// <param name="actionButtonExtended">the action button extended to initialize with</param>
        public void Initialize(LevelEditorActionButtonControllerExtended actionButtonExtended)
        {
            _button.onClick.AddListener(() => actionButtonExtended.SetButtonChoice(_choiceObject, _icon.sprite));
        }
    }
}
