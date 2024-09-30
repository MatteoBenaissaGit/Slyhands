using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Characters;
using GameEngine;
using LevelEditor.Entities;
using Players;
using Slots;
using UnityEngine;
using Random = UnityEngine.Random;

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
        MoveTo = 1, //parameters[0] = List<SlotController> path
        GetHit = 2,
        Die = 3,
        IsSelected = 5,
        IsUnselected = 6,
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
        
    }
    
    public class BoardCharacterController : BoardEntity
    {
        public CharacterType Type { get; private set; }
        public CharacterControllerData GameplayData { get; private set; }
        public CharacterData Data { get; private set; }
        
        public Action<CharacterAction, object[]> OnCharacterAction { get; set; }
        public List<SlotController> AccessibleSlots { get; set; }
        
        public SlotController CurrentSlot => Board.Data.SlotLocations[Coordinates.x, Coordinates.y, Coordinates.z].SlotView.Controller;

        public BoardCharacterController(BoardController board, Vector3Int coordinates, Team team, CharacterType type) : base(board, coordinates)
        {
            SuperType = BoardEntitySuperType.Character;
            Type = type;

            Data = GameManager.Instance.CharactersData.GetCharacterData(Type);

            GameplayData = new CharacterControllerData(Data, team)
            {
                Road = GameManager.Instance.Board.GetSlotFromCoordinates(coordinates).Data.LevelEditorCharacter.Road
            };

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
                    if (parameters == null || parameters.Length == 0 || parameters[0] is not List<SlotController> path || path.Count == 0)
                    {
                        return;
                    }
                    GameplayData.CurrentMovementPoints -= path.Count + 1;
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
                   && GameplayData.Team.Player.Type == PlayerType.Local;
        }

        public void SetForNewTurn()
        {
            GameplayData.CurrentMovementPoints = Data.MovementPoints;
        }

        public void UpdateAccessibleSlots(int movementPoints)
        {
            AccessibleSlots = Board.GetAccessibleSlotsBySlot(CurrentSlot, movementPoints);
        }
        
        public Task PlayTurn()
        {
            OnCharacterAction.Invoke(Characters.CharacterAction.IsSelected, null);

            //TODO if state idle/base/attack/search etc...
            
            if (GameplayData.HasPredefinedRoad)
            {
                MoveOnRoad();
            }
            else
            { 
                MoveRandomly();
            }
                
            OnCharacterAction.Invoke(Characters.CharacterAction.IsUnselected, null);
            
            return Task.CompletedTask;
        }

        #region Road Movement

        private bool _characterFollowPingPongRoadClockwise = true;

        public void MoveOnRoad()
        {
            Vector3Int[] road = GameplayData.Road;

            BoardController board = GameManager.Instance.Board;
            SlotController slotToGo = null;
            List<SlotController> path = new List<SlotController>();
            SlotController currentCharacterSlot = CurrentSlot;

            int movementPoints = GameplayData.CurrentMovementPoints;
            SlotController baseCharacterSlot = CurrentSlot;
            baseCharacterSlot.Data.Character = null;

            int maxIterations = 100;
            while (movementPoints > 0) 
            {
                if (--maxIterations < 0)
                {
                    Debug.LogError("max iterations reached, breaking loop");
                    break;
                }
                
                //get the target slot
                Vector3Int targetSlotCoordinates = road[GameplayData.RoadIndex];
                SlotController targetSlot = board.GetSlotFromCoordinates(targetSlotCoordinates);
                SlotController targetSlotClosest = null;
                if (targetSlot.IsAccessible == false && board.GetClosestToSlotFromSlot(targetSlot, currentCharacterSlot, out targetSlotClosest) == false)
                {
                    slotToGo = currentCharacterSlot;
                    break;
                }

#if UNITY_EDITOR
                Debug.DrawLine(currentCharacterSlot.Location.transform.position, targetSlot.Location.transform.position, Color.red, 2f);                
#endif

                //get the path to target within accessible slots
                List<SlotController> currentPath = board.GetPath(currentCharacterSlot, targetSlotClosest ?? targetSlot);
                SlotController fromSlot = path.Count > 0 ? path[^1] : currentCharacterSlot;
                List<SlotController> accessibleSlots = GameManager.Instance.Board.GetAccessibleSlotsBySlot(fromSlot, movementPoints);
                currentPath.RemoveAll(x => accessibleSlots.Contains(x) == false);
                
                path.AddRange(currentPath);
                
                if (path.Count == 0)
                {
                    break;
                }
                
                movementPoints -= currentPath.Count;
                slotToGo = path[^1];
                currentCharacterSlot = slotToGo;
                
                if (slotToGo == targetSlot)
                {
                    switch (GameplayData.RoadFollowMode)
                    {
                        case RoadFollowMode.PingPong:
                            GameplayData.RoadIndex += _characterFollowPingPongRoadClockwise ? 1 : -1;
                            if (_characterFollowPingPongRoadClockwise && GameplayData.RoadIndex >= road.Length)
                            {
                                _characterFollowPingPongRoadClockwise = false;
                                GameplayData.RoadIndex = road.Length - 2;
                            }
                            else if (_characterFollowPingPongRoadClockwise == false && GameplayData.RoadIndex < 0)
                            {
                                _characterFollowPingPongRoadClockwise = true;
                                GameplayData.RoadIndex = 1;
                            }
                            break;
                        case RoadFollowMode.Loop:
                            GameplayData.RoadIndex++;
                            if (GameplayData.RoadIndex >= road.Length - 1)
                            {
                                GameplayData.RoadIndex = 0;
                            }
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            baseCharacterSlot.Data.Character = this;

            if (slotToGo == null)
            {
                MoveRandomly();
                return;
            }
            
            OnCharacterAction.Invoke(Characters.CharacterAction.MoveTo, new object[]{path});
        }

        public void MoveRandomly()
        {
            List<SlotController> accessibleSlots = AccessibleSlots;
            SlotController targetSlot = accessibleSlots[Random.Range(0, accessibleSlots.Count)];
            List<SlotController> path = GameManager.Instance.Board.GetPath(CurrentSlot, targetSlot);
            OnCharacterAction.Invoke(Characters.CharacterAction.MoveTo, new object[]{path});
        }
        
        #endregion
    }
}