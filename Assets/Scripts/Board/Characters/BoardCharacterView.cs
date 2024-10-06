using System;
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
        [TitleGroup("Effects"), SerializeField] private ParticleSystem _stunParticleSystem;
        [TitleGroup("Effects"), SerializeField] private GameObject _attackIcon;
        
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");
        
        private Queue<SpriteRenderer> _footPrints;
        private float _footPrintFade;
        private static Vector3[] _orientationToFootPrintRotation = { new(90, 0, 0), new(90, 0, 270), new(90, 0, 180), new(90, 0, 90)};

        private UnityEngine.Camera _camera;
        
        public void Initialize(BoardCharacterController controller)
        {
            Controller = controller;
            Controller.OnCharacterAction += CharacterActionView;
            
            transform.rotation = Quaternion.Euler(0, ((int)Controller.GameplayData.Orientation) * 90, 0);
            
            InitializeFootPrints();
            
            _attackIcon.SetActive(false);
            GameManager.Instance.Camera.OnCameraRotated += MakeEffectsFaceCamera;
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.position, 0f);
        }

        private void Start()
        {
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.position, 0f);
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

        private void MakeEffectsFaceCamera(Vector3 cameraPosition, float moveDuration)
        {
            _attackIcon.transform.DOKill();
            _attackIcon.transform.DOLookAt(cameraPosition, moveDuration);
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
                case CharacterAction.Stun:
                    GameManager.Instance.TaskManager.EnqueueTask(SetStun);
                    break;
                case CharacterAction.UpdateStun:
                    GameManager.Instance.TaskManager.EnqueueTask(UpdateStun);
                    break;
                case CharacterAction.EndStun:
                    GameManager.Instance.TaskManager.EnqueueTask(EndStun);
                    break;
                case CharacterAction.EnemyDetected:
                    GameManager.Instance.TaskManager.EnqueueTask(EnemyDetectedFeedback);
                    break;
                case CharacterAction.EnemyLost:
                    GameManager.Instance.TaskManager.EnqueueTask(EnemyLostFeedback);
                    break;
                case CharacterAction.IsHovered:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not Color teamColor)
                    {
                        return;
                    }
                    Controller.GetSlotsInDetectionView().ForEach(x => x.Location.ShowDetection(true, teamColor));
                    break;
                case CharacterAction.IsLeaved:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not Color color)
                    {
                        return;
                    }
                    Controller.GetSlotsInDetectionView().ForEach(x => x.Location.ShowDetection(false, new Color(color.r,color.g,color.b,0f)));
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

                    float fade = _footPrintFade / _footPrints.Count; 
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

        /// <summary>
        /// Rotate the player view to the target orientation
        /// </summary>
        /// <param name="orientation">the orientation</param>
        private async Task Rotate(WorldOrientation.Orientation orientation)
        {
            float rotateTime = 0.5f;
            
            Vector2Int direction = WorldOrientation.GetDirection(orientation);
            transform.DOLookAt(transform.position + new Vector3(direction.x,0,direction.y), rotateTime);
            
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.position, rotateTime);
            
            _animator.SetBool(IsWalking, true);
            await Task.Delay((int)(rotateTime * 1000));
            _animator.SetBool(IsWalking, false);
        }

        private async Task SetStun()
        {
            float animationTime = 1f;

            _stunParticleSystem.Play();

            _animator.SetTrigger(Stun);
            _animator.SetBool(IsStunned, true);
            
            await Task.Delay((int)(animationTime * 1000));
        }
        
        private async Task UpdateStun()
        {
            float animationTime = 1f;

            //TODO animate the top indicator
            
            await Task.Delay((int)(animationTime * 1000));
        }
        
        private async Task EndStun()
        {
            float animationTime = 1f;

            _stunParticleSystem.Stop();

            _animator.SetBool(IsStunned, false);
            
            await Task.Delay((int)(animationTime * 1000));
        }
        
        private async Task EnemyDetectedFeedback()
        {
            float animationTime = 0.5f;

            _attackIcon.SetActive(true);
            _attackIcon.transform.DOComplete();
            _attackIcon.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            
            await Task.Delay((int)(animationTime * 1000));
        }
        
        private async Task EnemyLostFeedback()
        {
            float animationTime = 0.5f;

            _attackIcon.SetActive(false);
            
            await Task.Delay((int)(animationTime * 1000));
        }
    }
}