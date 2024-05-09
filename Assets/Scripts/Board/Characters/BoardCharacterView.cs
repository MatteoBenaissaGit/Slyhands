using System;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterView : MonoBehaviour
    {
        public BoardCharacterController Controller { get; private set; }

        [SerializeField] private float _yOffset;
        
        public void Initialize(BoardCharacterController controller)
        {
            Controller = controller;
            Controller.OnCharacterAction += CharacterActionView;
            
            transform.rotation = Quaternion.Euler(0, ((int)Controller.GameplayData.Orientation) * 90, 0);
        }

        private void OnDestroy()
        {
            Controller.OnCharacterAction -= CharacterActionView;
        }

        private void CharacterActionView(CharacterAction action, Vector3Int targetCoordinates = new Vector3Int())
        {
            switch (action)
            {
                case CharacterAction.Idle:
                    break;
                case CharacterAction.MoveTo:
                    MoveTo(targetCoordinates);
                    break;
                case CharacterAction.GetHit:
                    break;
                case CharacterAction.Die:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void MoveTo(Vector3Int targetCoordinates)
        {
            transform.position = Controller.Board.GetCoordinatesToWorldPosition(targetCoordinates) + new Vector3(0,_yOffset,0);
        }
    }
}