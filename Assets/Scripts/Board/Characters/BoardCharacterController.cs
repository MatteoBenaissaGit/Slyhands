using System;
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
        Die = 3
    }

    public class CharacterControllerData
    {
        public Orientation Orientation {get; set;}
    }
    
    public class BoardCharacterController : BoardEntity
    {
        public CharacterActionDelegate OnCharacterAction { get; set; }
        public CharacterType Type { get; private set; }
        public CharacterControllerData GameplayData { get; private set; }
        
        public SlotController CurrentSlot
        {
            get { return Board.Data.SlotLocations[Coordinates.x, Coordinates.y, Coordinates.z].SlotView.Controller; }
        }

        public BoardCharacterController(BoardController board, Vector3Int coordinates) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Character;
            Type = CharacterType.PlayerMainCharacter;

            GameplayData = new CharacterControllerData();
            
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
                    CurrentSlot.Data.Character = null;
                    Coordinates = targetCoordinates;
                    CurrentSlot.Data.Character = this;
                    break;
                case Characters.CharacterAction.GetHit:
                    break;
                case Characters.CharacterAction.Die:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}