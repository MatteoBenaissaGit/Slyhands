﻿using System;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class is an action button controller where you have multiples choices option when you click it
    /// </summary>
    public class LevelEditorActionButtonControllerExtended : LevelEditorActionButtonController
    {
        [field:SerializeField] public bool UseIdInsteadOfObject { get; private set; }
        
        public GameObject CurrentChoice { get; private set; }
        public string CurrentChoiceID { get; private set; }
        
        [SerializeField] private GameObject _choicesTab;
        [SerializeField] private List<LevelEditorUIActionChoiceButton> _choiceButtons = new List<LevelEditorUIActionChoiceButton>();
        [SerializeField] private GameObject _preview;
        [SerializeField] private Image _previewImage;

        private bool _areChoicesDisplayed;

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnClickTap += CheckForClickTapClearChoiceTab;
            InputManager.Instance.LevelEditorInput.OnRightClick += CheckForClickTapClearChoiceTab;
            
            DisplayChoices(false);
            SetPreview(null);

            for (int i = 0; i < _choiceButtons.Count; i++)
            {
                _choiceButtons[i].Initialize(this);
            }
        }

        /// <summary>
        /// Set the action button selected
        /// </summary>
        /// <param name="isSelected">is the button selected ?</param>
        /// <param name="doInstant">do it instantly ?</param>
        public override void SetSelected(bool isSelected, bool doInstant = false)
        {
            DisplayChoices(isSelected);
            if (isSelected == false)
            {
                SetPreview(null);
                base.SetSelected(false, doInstant);
            }
        }

        /// <summary>
        /// Check when the player click if it's on the button ui or not
        /// </summary>
        private void CheckForClickTapClearChoiceTab()
        {
            if (EventSystem.current.IsPointerOverGameObject() || _areChoicesDisplayed == false)
            {
                return;
            }
            LevelEditorManager.Instance.UI.InputActionsManager.SetCurrentButton();
            DisplayChoices(false);
        }

        /// <summary>
        /// Display the choices of the button
        /// </summary>
        /// <param name="doShow">show the choices tab ?</param>
        private void DisplayChoices(bool doShow)
        {
            SelectedEffect(doShow);
            _areChoicesDisplayed = doShow;
            _choicesTab.SetActive(doShow);
        }

        /// <summary>
        /// Set the button choice
        /// </summary>
        /// <param name="choiceGameObject">the game object chosen</param>
        /// <param name="choiceObjectSprite">the icon of the chosen object</param>
        public void SetButtonChoice(GameObject choiceGameObject, Sprite choiceObjectSprite = null)
        {
            SetButtonChoiceAction(() => CurrentChoice = choiceGameObject, choiceObjectSprite);
        }
        
        /// <summary>
        /// Set the button choice
        /// </summary>
        /// <param name="choiceID">the id chosen</param>
        /// <param name="choiceObjectSprite">the icon of the chosen object</param>
        public void SetButtonChoice(string choiceID, Sprite choiceObjectSprite = null)
        {
            SetButtonChoiceAction(() => CurrentChoiceID = choiceID, choiceObjectSprite);
        }

        /// <summary>
        /// Set the action to do when the choice is selected
        /// </summary>
        /// <param name="action">the action to do</param>
        /// <param name="choiceObjectSprite">the icon of the chosen object</param>
        private void SetButtonChoiceAction(Action action, Sprite choiceObjectSprite = null)
        {
            if (_areChoicesDisplayed == false)
            {
                return;
            }
            
            action.Invoke();
            
            DisplayChoices(false);
            SetPreview(choiceObjectSprite);
            base.SetSelected(true);
        }

        /// <summary>
        /// Set the preview of the chosen object
        /// </summary>
        /// <param name="sprite">the icon to show, if null deactivate the preview</param>
        private void SetPreview(Sprite sprite)
        {
            _preview.SetActive(sprite != null);
            _previewImage.sprite = sprite;
        }
    }
}