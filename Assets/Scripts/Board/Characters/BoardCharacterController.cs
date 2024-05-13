using System;
using System.Collections.Generic;
using Data.Characters;
using GameEngine;
using Players;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public enum CharacterType
    {
        PlayerMainCharacter = 0,
        BaseEnemy = 1,
    }

    public enum CharacterAction
    {
        Idle = 0,
        MoveTo = 1,
        GetHit = 2,
        Die = 3,
        IsSelected = 5,
        IsUnselected = 6,
    }

    public class CharacterControllerData
    {
        public CharacterControllerData(int maxLife, int teamNumber)
        {
            MaxLife = maxLife;
            CurrentLife = MaxLife;
            
            Team = GameManager.Instance.Teams.Find(x => x.TeamNumber == teamNumber);
        }

        public Team Team { get; set; }
        public bool IsSelectable { get; set; } = true;
        public Orientation Orientation {get; set;}
        public int MaxLife { get; private set; }
        public int CurrentLife { get; set; }
    }
    
    public class BoardCharacterController : BoardEntity
    {
        public CharacterType Type { get; private set; }
        public CharacterControllerData GameplayData { get; private set; }
        public CharacterData Data { get; private set; }
        
        public CharacterActionDelegate OnCharacterAction { get; set; }
        public List<SlotController> AccessibleSlots { get; set; }
        
        public SlotController CurrentSlot
        {
            get { return Board.Data.SlotLocations[Coordinates.x, Coordinates.y, Coordinates.z].SlotView.Controller; }
        }

        public BoardCharacterController(BoardController board, Vector3Int coordinates) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Character;
            Type = CharacterType.PlayerMainCharacter;

            Data = GameManager.Instance.CharactersData.GetCharacterData(Type);
            GameplayData = new CharacterControllerData(Data.Life, (int)Type); //TODO set team from level editor character parameter
            
            OnCharacterAction += CharacterAction;
        }
        
        public delegate void CharacterActionDelegate(CharacterAction action, Vector3Int targetCoordinates = new Vector3Int());
        
        private void CharacterAction(CharacterAction action, Vector3Int targetCoordinates = new Vector3Int())
        {
            switch (action)
            {
                case Characters.CharacterAction.Idle:
                    break;
                case Characters.CharacterAction.MoveTo:
                    break;
                case Characters.CharacterAction.GetHit:
                    break;
                case Characters.CharacterAction.Die:
                    break;
                case Characters.CharacterAction.IsSelected:
                    break;
                case Characters.CharacterAction.IsUnselected:
                    break;
            }
        }

        public void MoveTo(Vector3Int targetCoordinates)
        {
            CurrentSlot.Data.Character = null;
            Coordinates = targetCoordinates;
            CurrentSlot.Data.Character = this;
        }
        
        /// <summary>
        /// Tell if this character can be selected and played by a local player
        /// </summary>
        /// <returns>Can the character be played by the current local player ?</returns>
        public bool CanGetPlayed()
        {
            return GameplayData.IsSelectable 
                   && GameManager.Instance.Data.CurrentTurnTeam == GameplayData.Team 
                   && GameplayData.Team.Player.Type == PlayerType.Local;
        }
    }
}