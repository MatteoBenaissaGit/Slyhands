using System;
using Inputs;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorUICreateBoardMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private TMP_InputField _widthInputField;
        [SerializeField, Required] private TMP_InputField _lengthInputField;
        [SerializeField, Required] private TMP_InputField _heightInputField;
        
        [SerializeField, Required] private Button _createNewBoardButton;

        private void Start()
        {
            _createNewBoardButton.onClick.AddListener(CreateNewBoardButtonActon);

            InputManager.Instance.LevelEditorInput.OnTabPressed += TabPressed;
            InputManager.Instance.LevelEditorInput.OnEnterPressed += EnterPressed;
        }

        public override void OnMenuOpened()
        {
            _widthInputField.Select();
        }

        protected override void OnDestroy()
        {
            _createNewBoardButton.onClick.RemoveListener(CreateNewBoardButtonActon);
        }

        private void TabPressed()
        {
            GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (selectedGameObject == _widthInputField.gameObject)
            {
                _lengthInputField.Select();
            }
            else if (selectedGameObject == _lengthInputField.gameObject)
            {
                _heightInputField.Select();
            }
            else if (selectedGameObject == _heightInputField.gameObject)
            {
                _widthInputField.Select();
            }
        }
        
        private void EnterPressed()
        {
            CreateNewBoardButtonActon();
        }

        private void CreateNewBoardButtonActon()
        {
            LevelEditorManager.Instance.UI.HideMenu();
            
            if (int.TryParse(_widthInputField.text, out int width) == false ||
                int.TryParse(_heightInputField.text, out int height) == false ||
                int.TryParse(_lengthInputField.text, out int length) == false)
            {
                return;
            }

            if (width == 0 || height == 0 || length == 0)
            {
                return;
            }
            
            LevelEditorManager.Instance.Board.CreateBlankBoard(width, height, length);
        }
    }
}
