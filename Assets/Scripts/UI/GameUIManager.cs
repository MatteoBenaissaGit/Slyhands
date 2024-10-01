using System;
using GameEngine;
using Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] [Required] [FoldoutGroup("UI References")] private TMP_Text _turnTeamNumberText;
        [SerializeField] [Required] [FoldoutGroup("UI References")] private Button _nextTurnButton;
        
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _nextTurnButton.onClick.AddListener(() => GameManager.Instance.TaskManager.EnqueueTask(_gameManager.SetNextTurn));
        }

        private void OnDestroy()
        {
            _nextTurnButton.onClick.RemoveListener(() => GameManager.Instance.TaskManager.EnqueueTask(_gameManager.SetNextTurn));
        }

        public void SetTurnForTeam(Team team)
        {
            _turnTeamNumberText.text = team.Number.ToString();
            _turnTeamNumberText.color = team.Color;
            
            _nextTurnButton.gameObject.SetActive(team.Player.Type == PlayerType.Local);
        }
    }
}
