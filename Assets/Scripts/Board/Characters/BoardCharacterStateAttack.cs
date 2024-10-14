using System.Collections.Generic;
using GameEngine;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterStateAttack : BoardCharacterState
    {
        public BoardCharacterController EnemyAttacked { get; set; }
        public Vector3Int EnemyAttackedLastSeenCoordinates { get; set; }
        
        public BoardCharacterStateAttack(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            
        }

        public override void Play()
        {
            if (Controller.GetEnemiesInDetectionView(Controller.Coordinates, Controller.GameplayData.Orientation).Contains(EnemyAttacked) == false)
            {
                Controller.OnCharacterAction.Invoke(CharacterAction.EnemyLost, new object[]{EnemyAttackedLastSeenCoordinates});
                Controller.AlertState.Play();
                return;
            }

            EnemyAttackedLastSeenCoordinates = EnemyAttacked.Coordinates;

            MoveTowardPosition(EnemyAttacked.Coordinates);

            TryAttackingEnemy();
        }

        private void TryAttackingEnemy()
        {
            if (BoardController.GetDistanceBetweenSlots(Controller.CurrentSlot, EnemyAttacked.CurrentSlot) > 1)
            {
                return;
            }
            Controller.Attack(EnemyAttacked);
        }

        public override void Quit()
        {
            
        }
    }
}