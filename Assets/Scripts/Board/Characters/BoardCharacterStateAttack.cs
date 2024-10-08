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
            Debug.Log("PLAY ATTACK");
            
            if (Controller.GetEnemiesInDetectionView(Controller.Coordinates, Controller.GameplayData.Orientation).Contains(EnemyAttacked) == false)
            {
                Debug.Log("WHY SO FEINIOUS ?");
                Controller.OnCharacterAction.Invoke(CharacterAction.EnemyLost, new object[]{EnemyAttackedLastSeenCoordinates});
                // Controller.AlertState.Play();
                return;
            }

            EnemyAttackedLastSeenCoordinates = EnemyAttacked.Coordinates;

            Debug.Log("MOVE TO ENEMY");
            MoveTowardEnemy();

            TryAttackingEnemy();
        }

        private void TryAttackingEnemy()
        {
            //TODO attack
        }

        public override void Quit()
        {
            
        }
        
        private void MoveTowardEnemy()
        {
            List<SlotController> pathToEnemy = GameManager.Instance.Board.GetPath(Controller.Coordinates, EnemyAttacked.Coordinates, PathFindingOption.IgnoreCharacters);
            List<SlotController> accessibleSlots = GameManager.Instance.Board.GetAccessibleSlotsBySlot(Controller.CurrentSlot, Controller.GameplayData.CurrentMovementPoints);
            pathToEnemy.RemoveAll(x => accessibleSlots.Contains(x) == false);
            
            pathToEnemy.Insert(0, Controller.CurrentSlot);
            WorldOrientation.Orientation controllerOrientation = WorldOrientation.GetDirection(pathToEnemy[^1].Coordinates, EnemyAttacked.Coordinates);
            pathToEnemy.RemoveAt(0);
            
            Controller.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[] { pathToEnemy, controllerOrientation});
        }
    }
}