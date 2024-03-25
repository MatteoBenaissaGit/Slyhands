using System;
using System.Collections.Generic;
using Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class is an action button controller where you have multiples choices option when you click it
    /// </summary>
    public class LevelEditorActionButtonControllerExtended : LevelEditorActionButtonController
    {
        public GameObject CurrentChoice { get; private set; }
        
        [SerializeField] private GameObject _choicesTab;
        [SerializeField] private List<GameObject> _choicesList = new List<GameObject>();

        private bool _areChoicesDisplayed;

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnClickTap += CheckForClickTapClearChoiceTab;
            InputManager.Instance.LevelEditorInput.OnRightClick += CheckForClickTapClearChoiceTab;
            
            DisplayChoices(false);
        }

        public override void SetSelected(bool isSelected, bool doInstant = false)
        {
            DisplayChoices(isSelected);
            if (isSelected == false)
            {
                base.SetSelected(false, doInstant);
            }
        }

        private void CheckForClickTapClearChoiceTab()
        {
            if (EventSystem.current.IsPointerOverGameObject() || _areChoicesDisplayed == false)
            {
                return;
            }
            LevelEditorManager.Instance.UI.ActionButtons.SetCurrentButton();
            DisplayChoices(false);
        }

        private void DisplayChoices(bool doShow)
        {
            SelectedEffect(doShow);
            _areChoicesDisplayed = doShow;
            _choicesTab.SetActive(doShow);
        }

        public void SetButtonChoice(int choiceIndex)
        {
            if (_areChoicesDisplayed == false)
            {
                return;
            }
            CurrentChoice = _choicesList[choiceIndex];
            DisplayChoices(false);
            base.SetSelected(true);
        }
    }
}