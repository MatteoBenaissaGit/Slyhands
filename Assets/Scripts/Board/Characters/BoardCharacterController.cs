using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board.Characters.AttackSystem;
using Data.Characters;
using GameEngine;
using LevelEditor.Entities;
using Players;
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
        
        MoveTo = 1, //parameters[0] = List<SlotController> path, parameters[1] = Orientation finalOrientation
        
        GetHit = 2,
        Die = 3,
        
        IsSelected = 5,
        IsUnselected = 6,
       
        Rotate = 7, //parameters[0] = WorldOrientation.Orientation orientation 
        
        Stun = 8,  //parameters[0] = int turn duration
        UpdateStun = 9,
        EndStun = 10,
        
        EnemyDetected = 11, //parameters[0] = BoardCharacterController enemy, parameters[1] = Vector3Int lastSeenCoordinates
        EnemyLost = 12, //parameters[0] = Vector3Int lastSeenCoordinates
        StopSearchingEnemy = 13,
        
        IsHovered = 14, //parameters[0] = Color teamColor
        IsLeaved = 15, //parameters[0] = Color teamColor
        
        Attack = 16, //parameters[0] = IAttackable attackable
        GetAttacked = 17 //parameters[0] = IAttacker attacker
    }

    public class CharacterControllerData
    {
        public CharacterControllerData(CharacterData data, Team team)
        {
            MaxLife = data.Life;
            CurrentLife = MaxLife;

            Team = team;
        }

        public Team Team { get; set; }
        public bool IsSelectable { get; set; } = true;
        public WorldOrientation.Orientation Orientation {get; set;}
        
        public int MaxLife { get; private set; }
        public int CurrentLife { get; set; }
        public int CurrentMovementPoints { get; set; }
        
        public Vector3Int[] Road { get; set; }
        public RoadFollowMode RoadFollowMode => Road[0] == Road[^1] ? RoadFollowMode.Loop : RoadFollowMode.PingPong;
        public int RoadIndex { get; set; } = 1;
        public bool HasPredefinedRoad => Road != null && Road.Length > 1;
        
        public Vector2Int[] DetectionView { get; set; }
    }
    
    public class BoardCharacterController : BoardEntity, IAttackable, IAttacker
    {
        public CharacterType Type { get; private set; }
        public CharacterControllerData GameplayData { get; private set; }
        public CharacterData Data { get; private set; }
        
        public BoardCharacterState CurrentState { get; set; }
        public BoardCharacterStatePatrol PatrolState;
        public BoardCharacterStateAttack AttackState;
        public BoardCharacterStateAlert AlertState;
        public BoardCharacterStateStunned StunnedState;
        
        public Action<CharacterAction, object[]> OnCharacterAction { get; set; }
        public List<SlotController> AccessibleSlots { get; set; }
        
        public SlotController CurrentSlot => Board.Data.SlotLocations[Coordinates.x, Coordinates.y, Coordinates.z].SlotView.Controller;

        public BoardCharacterController(BoardController board, Vector3Int coordinates, Team team, CharacterType type, WorldOrientation.Orientation orientation) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Character;
            Type = type;

            Data = GameManager.Instance.CharactersData.GetCharacterData(Type);

            List<Vector2Int> detectionView = new List<Vector2Int>();
            foreach (ViewDetectionSquare detectionSquare in Data.ViewDetectionList)
            {
                if (detectionSquare.IsDetected == false) continue;
                detectionView.Add(detectionSquare.Position);
            }
            
            GameplayData = new CharacterControllerData(Data, team)
            {
                Road = GameManager.Instance.Board.GetSlot(coordinates).Data.LevelEditorCharacter.Road,
                DetectionView = detectionView.ToArray(),
                Orientation = orientation
            };
            
            PatrolState = new BoardCharacterStatePatrol(this);
            AttackState = new BoardCharacterStateAttack(this);
            AlertState = new BoardCharacterStateAlert(this);
            StunnedState = new BoardCharacterStateStunned(this);
            SetState(PatrolState);

            OnCharacterAction += CharacterAction;
        }
        
        private void CharacterAction(CharacterAction action, params object[] parameters)
        {
            switch (action)
            {
                case Characters.CharacterAction.Idle:
                    break;
                case Characters.CharacterAction.MoveTo: 
                    GameplayData.IsSelectable = true;
                    if (parameters == null || parameters.Length < 2 || parameters[0] is not List<SlotController> path || path.Count == 0)
                    {
                        return;
                    }
                    if (parameters[1] is not WorldOrientation.Orientation orientation)
                    {
                        return;
                    }
                    GameplayData.Orientation = orientation;
                    GameplayData.CurrentMovementPoints -= path.Count;
                    MoveTo(path[^1].Coordinates);
                    break;
                case Characters.CharacterAction.GetHit:
                    break;
                case Characters.CharacterAction.Die:
                    break;
                case Characters.CharacterAction.IsSelected:
                    break;
                case Characters.CharacterAction.IsUnselected:
                    break; 
                case Characters.CharacterAction.Stun:
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not int duration)
                    {
                        return;
                    }
                    SetState(StunnedState);
                    StunnedState.Duration = duration;
                    break;
                case Characters.CharacterAction.EnemyDetected:
                    if (parameters == null || parameters.Length < 2 || parameters[0] is not BoardCharacterController enemy)
                    {
                        return;
                    }
                    if (parameters[1] is not Vector3Int seenCoordinates)
                    {
                        return;
                    }

                    AttackState.EnemyAttacked = enemy;
                    AttackState.EnemyAttackedLastSeenCoordinates = seenCoordinates;
                    SetState(AttackState);
                    break;
                case Characters.CharacterAction.EnemyLost:
                    if (parameters == null || parameters.Length <= 0 || parameters[0] is not Vector3Int lastSeenCoordinates)
                    {
                        return;
                    }
                    AlertState.LastSeenEnemyPosition = lastSeenCoordinates;
                    SetState(AlertState);
                    break;
                case Characters.CharacterAction.StopSearchingEnemy:
                    SetState(PatrolState);
                    break;
            }
        }

        public void MoveTo(Vector3Int targetCoordinates)
        {
            CurrentSlot.Data.Character = null;
            Coordinates = targetCoordinates;
            CurrentSlot.Data.Character = this;
        }
        
        /// <summary>
        /// Tell if this character can be selected and played by a local player
        /// </summary>
        /// <returns>Can the character be played by the current local player ?</returns>
        public bool CanGetPlayed()
        {
            return GameplayData.IsSelectable 
                   && GameManager.Instance.Data.CurrentTurnTeam == GameplayData.Team 
                   && GameplayData.Team.Player.Type == PlayerType.Local
                   && CurrentState.CanPlay;
        }

        public void SetForNewTurn()
        {
            GameplayData.CurrentMovementPoints = Data.MovementPoints;
        }

        public void UpdateAccessibleSlots(int movementPoints)
        {
            AccessibleSlots = Board.GetAccessibleSlotsBySlot(CurrentSlot, movementPoints);
        }

        #region State

        public void SetState(BoardCharacterState state)
        {
            if (state == CurrentState)
            {
                Debug.LogWarning("State already set");
                return;
            }
            
            CurrentState?.Quit();
            CurrentState = state;
            CurrentState.Start();
        }

        #endregion
        
        public Task PlayTurn()
        {
            OnCharacterAction.Invoke(Characters.CharacterAction.IsSelected, null);
            
            CurrentState.Play();
                
            OnCharacterAction.Invoke(Characters.CharacterAction.IsUnselected, null);
            
            return Task.CompletedTask;
        }

        public void SubscribeToDetection()
        {
            SubscribeToDetectionView();
            SubscribeToSoundDetection();
        }

        public void UnsubscribeToDetection()
        {
            UnsubscribeToDetectionView();
            UnsubscribeToDetectionView();
        }
        
        #region Detection

        public void DetectEnemies()
        {
            DetectEnemies(Coordinates, GameplayData.Orientation);
        }
        
        public void DetectEnemies(Vector3Int coordinates, WorldOrientation.Orientation orientation)
        {
            List<BoardEntity> enemiesDetected = GetEnemiesInDetectionView(coordinates, orientation);
            if (enemiesDetected.Count > 0)
            {
                enemiesDetected
                    .Sort((x, y) => Vector3Int.Distance(x.Coordinates, coordinates)
                        .CompareTo(Vector3Int.Distance(y.Coordinates, coordinates)));
                BoardEntity enemy = enemiesDetected[0];
                
                OnCharacterAction?.Invoke(Characters.CharacterAction.EnemyDetected, new object[]{enemy, enemy.Coordinates});
            }
        }
        
        public List<SlotController> GetSlotsInDetectionView()
        {
            return GetSlotsInDetectionView(Coordinates, GameplayData.Orientation);
        }
        
        public List<SlotController> GetSlotsInDetectionView(Vector3Int coordinates, WorldOrientation.Orientation orientation)
        {
            List<SlotController> slots = new();
            foreach (Vector2Int detectionSquare in GameplayData.DetectionView)
            {
                Vector3Int offset = WorldOrientation.TransposeVectorToOrientation(new Vector3Int(detectionSquare.x, 0, detectionSquare.y), orientation);
                Vector3Int coordinatesOffset = coordinates + offset;
                SlotController slot = Board.GetSlot(coordinatesOffset);
                while (slot == null && coordinatesOffset.y > 0)
                {
                    coordinatesOffset.y --;
                    slot = Board.GetSlot(coordinatesOffset);
                }
                if (slot == null) continue;
                slots.Add(slot);
            }
            return slots;
        }

        public List<BoardEntity> GetEntitiesInDetectionView(Vector3Int coordinates, WorldOrientation.Orientation orientation)
        {
            List<BoardEntity> entities = new List<BoardEntity>();
            List<SlotController> slots = GetSlotsInDetectionView(coordinates, orientation);
            foreach (SlotController slot in slots)
            {
                if (slot != null && slot.HasCharacter(out var character))
                {
                    entities.Add(character);
                }
            }

            return entities;
        }

        public List<BoardEntity> GetEnemiesInDetectionView(Vector3Int coordinates, WorldOrientation.Orientation orientation)
        {
            var entitiesInDetectionView = GetEntitiesInDetectionView(coordinates, orientation);
            List<BoardEntity> enemies = new List<BoardEntity>();
            foreach (var entity in entitiesInDetectionView)
            {
                if (entity is BoardCharacterController character && character.GameplayData.Team.Number != GameplayData.Team.Number)
                {
                    enemies.Add(character);
                }
            }

            return enemies;
        }

        private List<SlotController> _detectionViewSubbed;
        private void SubscribeToDetectionView()
        {
            _detectionViewSubbed = GetSlotsInDetectionView();
            foreach (SlotController slot in _detectionViewSubbed)
            {
                slot.OnCharacterEnter += OnCharacterEnteredDetectionView;
            }
        }

        private void OnCharacterEnteredDetectionView(BoardCharacterController character, Vector3Int slotCoordinates)
        {
            if (character.GameplayData.Team.Number != GameplayData.Team.Number)
            {
                if (CurrentState == AttackState && character == AttackState.EnemyAttacked)
                {
                    AttackState.EnemyAttackedLastSeenCoordinates = slotCoordinates;
                    return;
                }
                OnCharacterAction?.Invoke(Characters.CharacterAction.EnemyDetected, new object[]{character, slotCoordinates});
            }
        }

        private void UnsubscribeToDetectionView()
        {
            foreach (SlotController slot in _detectionViewSubbed)
            {
                slot.OnCharacterEnter -= OnCharacterEnteredDetectionView;
            }
            _detectionViewSubbed.Clear();
        }
        
        #endregion

        #region Sound

        private List<SlotController> _currentSoundDetectionSubbed;
        
        private void SubscribeToSoundDetection(int range = 1)
        {
            _currentSoundDetectionSubbed.Clear();
            _currentSoundDetectionSubbed = GameManager.Instance.Board.GetSlotsInRange(range, Coordinates);
            foreach (SlotController slot in _currentSoundDetectionSubbed)
            {
                slot.OnSoundDetected += OnSoundDetected;
            }
        }

        private void UnsubscribeToSoundDetection()
        {
            foreach (SlotController slot in _currentSoundDetectionSubbed)
            {
                slot.OnSoundDetected -= OnSoundDetected;
            }
        }

        private void OnSoundDetected(Vector3Int from, int range)
        {
            OnCharacterAction.Invoke(Characters.CharacterAction.EnemyLost, new object[]{from});
        }

        #endregion

        #region Attack
        
        public Vector3Int GetCoordinates => Coordinates; 

        public void GetAttacked(IAttacker attacker)
        {
            OnCharacterAction.Invoke(Characters.CharacterAction.GetAttacked, new object[]{attacker});
            if (GameplayData.Team.Number == 0)
            {
                GameManager.Instance.TaskManager.EnqueueTask(() => GameManager.Instance.EndGame(EndGameOptions.Loss));
            }
        }

        public void Attack(IAttackable attackable)
        {
            OnCharacterAction.Invoke(Characters.CharacterAction.Rotate, new object[]{WorldOrientation.GetDirection(Coordinates, attackable.GetCoordinates)});
            OnCharacterAction.Invoke(Characters.CharacterAction.Attack, new object[]{attackable});
            attackable.GetAttacked(this);
        }
        
        #endregion
    }
}