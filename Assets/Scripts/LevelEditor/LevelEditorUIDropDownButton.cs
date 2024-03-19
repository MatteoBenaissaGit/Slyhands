using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class manage the control of a dropdown button
    /// </summary>
    public class LevelEditorUIDropDownButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        private Action _buttonAction;

        public void Initialize(Action actionToDo, string actionName, bool usable = true)
        {
            _text.text = actionName;

            if (usable == false)
            {
                _text.color = new Color(0.47f, 0.47f, 0.47f);
                _button.enabled = false;
                return;
            }
            
            _buttonAction = actionToDo;
            _button.onClick.AddListener(_buttonAction.Invoke);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
