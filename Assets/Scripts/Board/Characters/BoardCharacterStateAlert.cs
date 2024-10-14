using System.Collections.Generic;
using GameEngine;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterStateAlert : BoardCharacterState
    {
        public Vector3Int LastSeenEnemyPosition { get; set; }
        
        private int _rotatingTurnsLeft = 3;
        
        public BoardCharacterStateAlert(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            _rotatingTurnsLeft = 3;
        }

        public override void Play()
        {
            if (Controller.Coordinates != LastSeenEnemyPosition)
            {
                MoveTowardPosition(LastSeenEnemyPosition); 
                
                SlotController lastSeenEnemySlot = GameManager.Instance.Board.GetSlot(LastSeenEnemyPosition);
                if (lastSeenEnemySlot != null && lastSeenEnemySlot.HasCharacter(out var character) && character != Controller && character.GameplayData.Team == Controller.GameplayData.Team)
                {
                    _rotatingTurnsLeft--;
                    Rotate();
                }
            }
            else
            {
                _rotatingTurnsLeft--;
                Rotate();
            }
            
            Controller.DetectEnemies();
            Controller.UnsubscribeToDetectionView();
            Controller.SubscribeToDetectionView();
            
            if (_rotatingTurnsLeft <= 0)
            {
                Controller.OnCharacterAction.Invoke(CharacterAction.StopSearchingEnemy, null);
            }
        }

        public override void Quit()
        {
        }
    }
}