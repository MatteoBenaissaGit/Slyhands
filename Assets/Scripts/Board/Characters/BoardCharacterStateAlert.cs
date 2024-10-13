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
            Debug.Log("PLAY ALERT");
            if (Controller.Coordinates != LastSeenEnemyPosition)
            {
                MoveTowardPosition(LastSeenEnemyPosition); 
                
                //TODO if an ally is blocking the position, start rotating
                //TODO check for enemy on the road like in patrol
            }
            else
            {
                _rotatingTurnsLeft--;
                Rotate();
                
                //TODO check if enemy is in sight
                
                if (_rotatingTurnsLeft <= 0)
                {
                    Controller.OnCharacterAction.Invoke(CharacterAction.StopSearchingEnemy, null);
                }
            }
        }

        public override void Quit()
        {
        }
    }
}