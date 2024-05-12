using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameEngine;
using LevelEditor;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterView : MonoBehaviour
    {
        public BoardCharacterController Controller { get; private set; }

        [SerializeField] private Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        public SlotLocation CurrentSlotLocation
        {
            get
            {
                BoardController board = GameManager.Instance == null ? LevelEditorManager.Instance.Board : GameManager.Instance.Board;
                SlotLocation slotLocation = board.Data.SlotLocations[Controller.Coordinates.x, Controller.Coordinates.y, Controller.Coordinates.z];
                if (slotLocation == null)
                {
                    throw new Exception("no slot location found for the current slot coordinates");
                }
                return slotLocation;
            }
        }

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
                    GameManager.Instance.Task.EnqueueTask(() => MoveTo(targetCoordinates));
                    break;
                case CharacterAction.GetHit:
                    break;
                case CharacterAction.Die:
                    break;
                case CharacterAction.IsSelected:
                    Controller.AccessibleSlots = Controller.Board.GetAccessibleSlotsByCharacter(Controller);
                    foreach (SlotController slot in Controller.AccessibleSlots)
                    {
                        slot.Location.SetSelected(true);
                        slot.Location.SetAccessible(true);
                    }
                    break;
                case CharacterAction.IsUnselected:
                    Controller.AccessibleSlots.ForEach(x => x.Location.SetSelected(false));
                    break;
            }
        }

        /// <summary>
        /// Move the player's view into the target coordinates finding the path to reach it
        /// </summary>
        /// <param name="targetCoordinates">the coordinates to reach</param>
        private async Task MoveTo(Vector3Int targetCoordinates)
        {
            Controller.GameplayData.CanGetSelected = false;
            
            SlotController targetSlot = Controller.Board.Data.SlotLocations[targetCoordinates.x, targetCoordinates.y, targetCoordinates.z].SlotView.Controller;
            List<SlotController> pathToTarget = Controller.Board.GetPathFromCharacterToSlot(Controller, targetSlot);

            float totalTime = 0;
            float moveTime = 0.2f;
            Sequence sequence = DOTween.Sequence();
            Vector3Int previousCoordinates = Controller.Coordinates;
            Vector3 previousPosition = transform.position;
            foreach (SlotController slot in pathToTarget)
            {
                //position
                Vector3 targetPosition = Controller.Board.GetCoordinatesToWorldPosition(slot.Coordinates);
                sequence.Append(transform.DOMove(targetPosition, moveTime).SetEase(Ease.Linear));
                
                //rotation
                Vector3 direction = slot.Coordinates - previousCoordinates;
                direction.y = 0;
                previousCoordinates = slot.Coordinates;
                previousCoordinates.y = 0;
                sequence.Join(transform.DOLookAt(previousPosition + direction, moveTime)).SetEase(Ease.Linear);
                
                previousPosition = targetPosition;

                totalTime += moveTime;
            }
            
            transform.DOKill();
            sequence.Play();
            _animator.SetBool(IsWalking, true);
            
            await Task.Delay((int)(totalTime * 1000));
            
            Controller.GameplayData.CanGetSelected = true;
            Controller.MoveTo(targetCoordinates);
            
            _animator.SetBool(IsWalking, false);
        }
    }
}