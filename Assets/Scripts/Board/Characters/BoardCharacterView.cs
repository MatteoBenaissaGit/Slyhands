using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameEngine;
using Players;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;
using UnityEngine.Serialization;

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
        [TitleGroup("Effects"), SerializeField] private SpriteRenderer _stateIcon;
        
        //animator values
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");
        private static readonly int Detect = Animator.StringToHash("Detect");
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        private Queue<SpriteRenderer> _footPrints;
        private float _footPrintFade;
        private static Vector3[] _orientationToFootPrintRotation = { new(90, 0, 0), new(90, 0, 270), new(90, 0, 180), new(90, 0, 90)};

        private UnityEngine.Camera _camera;

        private Sprite _attackSprite;
        private Sprite _alertSprite;
        
        public void Initialize(BoardCharacterController controller)
        {
            Controller = controller;
            Controller.OnCharacterAction += CharacterActionView;
            
            transform.rotation = Quaternion.Euler(0, ((int)Controller.GameplayData.Orientation) * 90, 0);
            
            InitializeFootPrints();

            _stateIcon.sprite = null;
            GameManager.Instance.Camera.OnCameraRotated += MakeEffectsFaceCamera;
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.forward, 0f);
            
            _attackSprite = GameManager.Instance.UIData.GetSprite("AttackIcon");
            _alertSprite = GameManager.Instance.UIData.GetSprite("AlertIcon");
        }

        private void Start()
        {
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.forward, 0f);
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

        private void MakeEffectsFaceCamera(Vector3 cameraForward, float moveDuration)
        {
            _stateIcon.transform.DOKill();
            _stateIcon.transform.DOLookAt(_stateIcon.transform.position - cameraForward, moveDuration);
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
                    if (parameters.Length < 3 || parameters[1] is not WorldOrientation.Orientation finalOrientation || parameters[2] is not bool)
                    {
                        return;
                    }
                    GameManager.Instance.TaskManager.EnqueueTask(() => Rotate(finalOrientation));
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
                    GameManager.Instance.TaskManager.EnqueueTask(() => SetStateIcon(null, false));
                    GameManager.Instance.TaskManager.EnqueueTask(SetStun);
                    break;
                case CharacterAction.UpdateStun:
                    GameManager.Instance.TaskManager.EnqueueTask(UpdateStun);
                    break;
                case CharacterAction.EndStun:
                    GameManager.Instance.TaskManager.EnqueueTask(EndStun);
                    break;
                case CharacterAction.EnemyDetected:
                    GameManager.Instance.TaskManager.EnqueueTask(() => SetStateIcon(_attackSprite));
                    break;
                case CharacterAction.EnemyLost:
                    GameManager.Instance.TaskManager.EnqueueTask(() => SetStateIcon(_alertSprite));
                    break;
                case CharacterAction.IsHovered:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not Color teamColor || Controller.CurrentState.CanPlay == false)
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
                case CharacterAction.StopSearchingEnemy:
                    GameManager.Instance.TaskManager.EnqueueTask(() => SetStateIcon(null, false));
                    break;
                case CharacterAction.Attack:
                    GameManager.Instance.TaskManager.EnqueueTask(AttackFeedback);
                    break;
                case CharacterAction.GetAttacked:
                    GameManager.Instance.TaskManager.EnqueueTask(DeathFeedback);
                    break;
                case CharacterAction.SoundDetected:
                    if (parameters == null || parameters.Length < 3 || parameters[0] is not Vector3Int coordinates)
                    {
                        return;
                    }
                    Debug.Log("sound detected");
                    GameManager.Instance.TaskManager.EnqueueTask(() => SetStateIcon(_alertSprite));
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
                
                //effects
                MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.forward, moveTime);
                
                //slot event
                slot.OnCharacterEnter?.Invoke(Controller, slot.Coordinates);
                
                //play sequence
                previousPosition = targetPosition;
                transform.DOKill();
                sequence.Play();
                _animator.SetBool(IsWalking, true);
                
                await Task.Delay((int)((moveTime * multiplier) * 1000));
                
                //sound
                if (Controller.Data.DoMakeSoundMoving)
                {
                    slot.MakeSound(Controller, Controller.Data.MoveSoundRange);
                }
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
            
            MakeEffectsFaceCamera(GameManager.Instance.Camera.transform.forward, rotateTime);
            
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
        
        private async Task SetStateIcon(Sprite icon, bool doShow = true)
        {
            float animationTime = 0.5f;

            if (doShow)
            {
                _animator.SetTrigger(Detect);
            }

            _stateIcon.sprite = doShow ? icon : null;
            _stateIcon.transform.DOComplete();
            _stateIcon.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            
            await Task.Delay((int)(animationTime * 1000));
        }

        private async Task AttackFeedback()
        {
            _animator.SetTrigger(Attack);
            await Task.Delay(1000);
        }

        private async Task DeathFeedback()
        {
            _animator.SetTrigger(Stun);
            _animator.SetBool(IsStunned, true);
            await Task.Delay(1000);
        }
    }
}