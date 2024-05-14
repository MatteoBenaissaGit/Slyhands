using System.Collections.Generic;
using LevelEditor.Entities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.LoadAndSave
{
    public class LevelEditorUISetTeamMenu : LevelEditorUIMenu
    {
        [SerializeField, Required] private Button _saveButton;
        [SerializeField, Required] private TMP_Dropdown _dropDown;

        private LevelEditorCharacter _character;
        private List<int> _teams = new List<int>();

        public void OpenMenu(LevelEditorCharacter levelEditorCharacter)
        {
            _character = levelEditorCharacter;

            _teams.Clear();
            foreach (var team in LevelEditorManager.Instance.TeamsData.Teams)
            {
                _teams.Add(team.TeamNumber);
            }
            _dropDown.options.Clear();
            _teams.ForEach(x => _dropDown.options.Add(new TMP_Dropdown.OptionData(x.ToString())));
            _dropDown.value = _teams.IndexOf(_character.TeamNumber);
            _dropDown.RefreshShownValue();
            
            base.OpenMenu();
        }

        private void OnEnable()
        {
            base.Awake();

            _saveButton.onClick.AddListener(SetTeam);
        }

        private void OnDisable()
        {
            _saveButton.onClick.RemoveListener(SetTeam);
        }

        private void SetTeam()
        {
            _character.SetTeam(_teams[_dropDown.value]);
            
            LevelEditorManager.Instance.UI.HideMenu();
        }
    }
}