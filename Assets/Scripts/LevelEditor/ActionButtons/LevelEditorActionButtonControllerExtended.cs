using System;
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
        public GameObject CurrentChoice { get; private set; }
        
        [SerializeField] private GameObject _choicesTab;
        [SerializeField] private List<LevelEditorUIActionChoiceButton> _choiceButtons = new List<LevelEditorUIActionChoiceButton>();
        [SerializeField] private GameObject _previsualisation;
        [SerializeField] private Image _previsualisationIcon;

        private bool _areChoicesDisplayed;

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnClickTap += CheckForClickTapClearChoiceTab;
            InputManager.Instance.LevelEditorInput.OnRightClick += CheckForClickTapClearChoiceTab;
            
            DisplayChoices(false);
            SetPrevisualisation(null);

            for (int i = 0; i < _choiceButtons.Count; i++)
            {
                _choiceButtons[i].Initialize(this);
            }
        }

        public override void SetSelected(bool isSelected, bool doInstant = false)
        {
            DisplayChoices(isSelected);
            if (isSelected == false)
            {
                SetPrevisualisation(null);
                base.SetSelected(false, doInstant);
            }
        }

        private void CheckForClickTapClearChoiceTab()
        {
            if (EventSystem.current.IsPointerOverGameObject() || _areChoicesDisplayed == false)
            {
                return;
            }
            LevelEditorManager.Instance.UI.InputActionsManager.SetCurrentButton();
            DisplayChoices(false);
        }

        private void DisplayChoices(bool doShow)
        {
            SelectedEffect(doShow);
            _areChoicesDisplayed = doShow;
            _choicesTab.SetActive(doShow);
        }

        public void SetButtonChoice(GameObject choiceGameObject, Sprite choiceObjectSprite = null)
        {
            if (_areChoicesDisplayed == false)
            {
                return;
            }
            
            CurrentChoice = choiceGameObject;
            DisplayChoices(false);
            SetPrevisualisation(choiceObjectSprite);
            
            base.SetSelected(true);
        }

        private void SetPrevisualisation(Sprite sprite)
        {
            _previsualisation.SetActive(sprite != null);
            _previsualisationIcon.sprite = sprite;
        }
    }
}