using System;
using System.Linq;
using Board.Characters;
using DG.Tweening;
using Players;
using Slots;
using UnityEngine;

namespace LevelEditor.Entities
{
    /// <summary>
    /// This class handle a level editor character
    /// </summary>
    public class LevelEditorCharacter : MonoBehaviour
    {
        [field:SerializeField] public int TeamNumber { get; private set; }
        [field:SerializeField] public CharacterType Type { get; private set; }
        
        public Vector3Int Coordinates => Slot == null ? Vector3Int.zero : Slot.Coordinates;
        public SlotController Slot { get; private set; }
        public bool IsActive { get; private set; }

        [SerializeField] private SpriteRenderer _teamFeedbackSprite;

        private Renderer[] _renderers;

        /// <summary>
        /// Initialize the character with the slot he's on
        /// </summary>
        /// <param name="slot">the slot on which the character is</param>
        public void Initialize(SlotController slot)
        {
            _renderers = GetComponentsInChildren<Renderer>();
            
            Slot = slot;
            SetCharacterOrientation(Slot.Data.LevelEditorCharacter.Orientation);

            SetTeam(TeamNumber);
            
            transform.DOKill();
            Vector3 scale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
        }
        
        /// <summary>
        /// Set the character's orientation and change its rotation to match it
        /// </summary>
        /// <param name="orientation">The orientation to put the character in</param>
        /// <returns>The character</returns>
        public LevelEditorCharacter SetCharacterOrientation(Orientation orientation)
        {
            Slot.Data.LevelEditorCharacter.Orientation = orientation;
            transform.rotation = Quaternion.Euler(0, ((int)orientation) * 90, 0);
            return this;
        }

        /// <summary>
        /// Set the team of the level editor character
        /// </summary>
        /// <param name="teamNumber">the number of the team to set</param>
        public void SetTeam(int teamNumber)
        {
            if (LevelEditorManager.Instance == null)
            {
                return;
            }
            
            TeamNumber = teamNumber;
            Team team = GetTeam();
            if (team == null)
            {
                throw new Exception("No team found for the given team number");
            }

            Slot.Data.LevelEditorCharacter.Team = team;
            _teamFeedbackSprite.color = team.TeamColor;
        }

        /// <summary>
        /// Get the team of this level editor character
        /// </summary>
        /// <returns>The team of the character</returns>
        public Team GetTeam()
        {
            return LevelEditorManager.Instance.TeamsData.Teams.Find(x => x.TeamNumber == TeamNumber);
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            
            foreach (Renderer mesh in _renderers)
            {
                Color color = IsActive ? Color.white : new Color(0.26f, 0.26f, 0.26f);
                mesh.material.color = color;
            }
        }
    }
}
