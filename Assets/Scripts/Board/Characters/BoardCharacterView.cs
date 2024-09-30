﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameEngine;
using Players;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterView : MonoBehaviour
    {
        public BoardCharacterController Controller { get; private set; }

        [TitleGroup("Animation"), SerializeField] private Animator _animator;
        [TitleGroup("Footprint"), SerializeField] private Sprite _footPrint;
        [TitleGroup("Footprint"), SerializeField] private Color _footPrintColor = Color.black;
        [TitleGroup("Footprint"), SerializeField] private float _footPrintSize = 0.25f;
        
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        
        private Queue<SpriteRenderer> _footPrints;
        private float _footPrintFade;
        private static Vector3[] _orientationToFootPrintRotation = { new(90, 0, 0), new(90, 0, 270), new(90, 0, 180), new(90, 0, 90)};

        public void Initialize(BoardCharacterController controller)
        {
            Controller = controller;
            Controller.OnCharacterAction += CharacterActionView;
            
            transform.rotation = Quaternion.Euler(0, ((int)Controller.GameplayData.Orientation) * 90, 0);
            
            InitializeFootPrints();
        }

        private void InitializeFootPrints()
        {
            if (_footPrint == null || Controller.Data.FootPrintLength == 0)
            {
                return;
            }

            _footPrintFade = _footPrintColor.a;
            
            int footPrints = Controller.Data.FootPrintLength;
            _footPrints = new Queue<SpriteRenderer>();
            GameObject footPrintParent = new GameObject("FootPrints Parent");
            for (int i = 0; i < footPrints; i++)
            {
                SpriteRenderer footPrint = new GameObject($"FootPrint_{i}").AddComponent<SpriteRenderer>();
                footPrint.sprite = _footPrint;
                footPrint.transform.parent = footPrintParent.transform;
                footPrint.gameObject.SetActive(false);
                _footPrints.Enqueue(footPrint);
            }
            footPrintParent.transform.localScale *= _footPrintSize;
        }

        private void OnDestroy()
        {
            Controller.OnCharacterAction -= CharacterActionView;
        }

        private void CharacterActionView(CharacterAction action, params object[] parameters)
        {
            switch (action)
            {
                case CharacterAction.Idle:
                    break;
                case CharacterAction.MoveTo:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not List<SlotController> path || path.Count == 0)
                    {
                        return;
                    }
                    GameManager.Instance.TaskManager.EnqueueTask(() => MoveTo(path));
                    break;
                case CharacterAction.GetHit:
                    break;
                case CharacterAction.Die:
                    break;
                case CharacterAction.IsSelected:
                    Controller.UpdateAccessibleSlots(Controller.GameplayData.CurrentMovementPoints);
                    if (Controller.GameplayData.Team.Player.Type != PlayerType.Local)
                    {
                        break;
                    }
                    foreach (SlotController slot in Controller.AccessibleSlots)
                    {
                        slot.Location.SetSelected(true);
                        slot.Location.SetAccessible(true);
                    }
                    break;
                case CharacterAction.IsUnselected:
                    if (Controller.GameplayData.Team.Player.Type != PlayerType.Local)
                    {
                        break;
                    }
                    Controller.AccessibleSlots.ForEach(x => x.Location.SetSelected(false));
                    break;
                case CharacterAction.Rotate:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not WorldOrientation.Orientation orientation)
                    {
                        return;
                    }
                    GameManager.Instance.TaskManager.EnqueueTask(() => Rotate(orientation));
                    break;
            }
        }

        /// <summary>
        /// Move the player's view into the target coordinates finding the path to reach it
        /// </summary>
        /// <param name="targetCoordinates">the coordinates to reach</param>
        private async Task MoveTo(List<SlotController> path)
        {
            float totalTime = 0;
            float moveTime = 0.2f;
            
            Vector3 previousPosition = transform.position;
            
            foreach (SlotController slot in path)
            {
                Sequence sequence = DOTween.Sequence();
                
                int multiplier = 1;
                
                //position
                Vector3 targetPosition = Controller.Board.GetCoordinatesToWorldPosition(slot.Coordinates);
                if (Math.Abs(targetPosition.y - previousPosition.y) > 0.1f)
                {
                    multiplier = 2;
                }
                sequence.Append(transform.DOMove(targetPosition, moveTime * multiplier).SetEase(Ease.Linear));
                
                //rotation
                Vector3 direction = GameManager.Instance.Board.GetCoordinatesToWorldPosition(slot.Coordinates) - previousPosition;
                direction.y = 0;
                sequence.Join(transform.DOLookAt(previousPosition + direction, moveTime * multiplier)).SetEase(Ease.Linear);
                
                //footprints
                if (_footPrints != null) //no footprints on stairs
                {
                    SpriteRenderer newFootPrint = _footPrints.Dequeue();
                    _footPrints.Enqueue(newFootPrint);
                    newFootPrint.DOComplete();
                    newFootPrint.transform.position = previousPosition;
                    WorldOrientation.Orientation orientation = WorldOrientation.GetOrientation(targetPosition - previousPosition);
                    newFootPrint.transform.rotation = Quaternion.Euler(_orientationToFootPrintRotation[(int)orientation]);
                    newFootPrint.gameObject.SetActive(true);
                    newFootPrint.color = new Color(_footPrintColor.r, _footPrintColor.g, _footPrintColor.b, 0f);

                    float fade = 1f / _footPrints.Count; 
                    foreach (SpriteRenderer footPrint in _footPrints)
                    {
                        footPrint.DOComplete();
                        sequence.Join(footPrint.DOFade(fade, moveTime * multiplier));
                        fade += 1f / _footPrints.Count;
                    }
                }
                
                //play sequence
                previousPosition = targetPosition;
                transform.DOKill();
                sequence.Play();
                _animator.SetBool(IsWalking, true);
                
                await Task.Delay((int)((moveTime * multiplier) * 1000));
            }
            
            _animator.SetBool(IsWalking, false);
        }

        private async Task Rotate(WorldOrientation.Orientation orientation)
        {
            float rotateTime = 0.5f;
            
            Vector2Int direction = WorldOrientation.GetDirection(orientation);
            transform.DOLookAt(transform.position + new Vector3(direction.x,0,direction.y), rotateTime);
            
            _animator.SetBool(IsWalking, true);
            await Task.Delay((int)(rotateTime * 1000));
            _animator.SetBool(IsWalking, false);
        }

    }
}