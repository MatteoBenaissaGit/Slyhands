using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.LoadAndSave
{
    /// <summary>
    /// This class handle the button used to select a level to load in the load menu
    /// </summary>
    public class LevelToLoadButtonController : MonoBehaviour
    {
        [field:SerializeField] [field:Required] public RectTransform Rect { get; private set; }
        
        public LevelData Data { get; private set; }
        
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private TMP_Text _nameText;
        [SerializeField, Required] private Button _deleteButton;

        /// <summary>
        /// Initialize the button by setting up its name & Data
        /// </summary>
        /// <param name="level">the level data to associate to this button</param>
        /// <returns></returns>
        public Button Initialize(LevelData level)
        {
            _nameText.text = level.Name;
            Data = level;

            _deleteButton.onClick.AddListener(DeleteSave);
            SetSelected(false);

            return _button;
        }

        /// <summary>
        /// Set the button selected or not
        /// </summary>
        /// <param name="isSelected">is the button selected ?</param>
        public void SetSelected(bool isSelected)
        {
            if (_button == null || _button.image == null || _button.image.gameObject == null)
            {
                return;
            }
            _button.image.color = isSelected ? Color.black : new Color(0.38f, 0.38f, 0.38f);
            _nameText.color = Color.white;
        }

        /// <summary>
        /// Get the level load button reference
        /// </summary>
        /// <returns>the level load button reference</returns>
        public Button GetButton()
        {
            return _button; 
        }

        /// <summary>
        /// Delete the save associated to the button in the game data and destroy the button
        /// </summary>
        private void DeleteSave()
        {
            _deleteButton.onClick.RemoveListener(DeleteSave);
            LevelEditorManager.Instance.SaveLoadManager.RemoveLevelData(Data);
            Destroy(gameObject);
        }
    }
}
