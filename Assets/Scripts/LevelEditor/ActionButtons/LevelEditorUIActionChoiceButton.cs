using Sirenix.OdinInspector;
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
        [SerializeField, HideIf("_idInsteadOfObject"), ShowIf("_referenceAnObject")] private GameObject _choiceObject;
        [SerializeField] private bool _idInsteadOfObject;
        [SerializeField, ShowIf("_idInsteadOfObject")] private string _choiceId;
        [SerializeField, ShowIf("_idInsteadOfObject")] private bool _referenceAnObject;
        
        /// <summary>
        /// Initialize the button for the desired action button extended
        /// </summary>
        /// <param name="actionButtonExtended">the action button extended to initialize with</param>
        public void Initialize(LevelEditorActionButtonControllerExtended actionButtonExtended)
        {
            if (_idInsteadOfObject == false || _referenceAnObject)
            {
                _button.onClick.AddListener(() => actionButtonExtended.SetButtonChoice(_choiceObject, _icon.sprite));
            }
            
            if (_idInsteadOfObject)
            {
                _button.onClick.AddListener(() => actionButtonExtended.SetButtonChoice(_choiceId, _icon.sprite));
            }
        }
    }
}
