using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorUIActionButtons : MonoBehaviour
    {
        [SerializeField, Required] private LevelEditorActionButtonController _selectionButton;
        [SerializeField, Required] private LevelEditorActionButtonController _paintButton;

        private List<LevelEditorActionButtonController> _buttons;
        private LevelEditorActionButtonController _currentButton;

        private void Awake()
        {
            _buttons = new List<LevelEditorActionButtonController>()
            {
                _selectionButton,
                _paintButton
            };
            
            _selectionButton.Button.onClick.AddListener(() => SetCurrentButton(_selectionButton));
            _paintButton.Button.onClick.AddListener(() => SetCurrentButton(_paintButton));

            _currentButton = _selectionButton;
            _currentButton.SetSelected(true, true);
        }

        private void OnDestroy()
        {
            _selectionButton.Button.onClick.RemoveListener(() => SetCurrentButton(_selectionButton));
            _paintButton.Button.onClick.RemoveListener(() => SetCurrentButton(_paintButton));
        }

        /// <summary>
        /// Set the current used button
        /// </summary>
        /// <param name="buttonToSet">The button to set as current</param>
        public void SetCurrentButton(LevelEditorActionButtonController buttonToSet)
        {
            _currentButton.SetSelected(false);
            _currentButton = buttonToSet;
            _currentButton.SetSelected(true);
        }
    }
}