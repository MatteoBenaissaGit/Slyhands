using System;
using System.Collections.Generic;
using GameEngine;
using LevelEditor.Entities;
using Slots;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board.Characters
{
    public class BoardCharacterStatePatrol : BoardCharacterState
    {
        public BoardCharacterStatePatrol(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            
        }

        public override void Play()
        {
            if (Controller.GameplayData.HasPredefinedRoad)
            {
                MoveOnRoad();
            }
            else
            {
                Rotate();
            }

            List<BoardEntity> enemiesDetected = Controller.GetEnemiesInDetectionView();
            if (enemiesDetected.Count > 0)
            {
                enemiesDetected
                    .Sort((x, y) => Vector3Int.Distance(x.Coordinates, Controller.CurrentSlot.Coordinates)
                    .CompareTo(Vector3Int.Distance(y.Coordinates, Controller.CurrentSlot.Coordinates)));
                BoardEntity enemy = enemiesDetected[0];
                
                Controller.OnCharacterAction?.Invoke(CharacterAction.EnemyDetected, new object[]{enemy});
            }
        }

        public override void Quit()
        {
            
        }
        
        #region Road Movement

        private bool _characterFollowPingPongRoadClockwise = true;

        public void MoveOnRoad()
        {
            Vector3Int[] road = Controller.GameplayData.Road;

            BoardController board = GameManager.Instance.Board;
            SlotController slotToGo = null;
            List<SlotController> path = new List<SlotController>();
            SlotController currentCharacterSlot = Controller.CurrentSlot;

            int movementPoints = Controller.GameplayData.CurrentMovementPoints;
            SlotController baseCharacterSlot = Controller.CurrentSlot;
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
                Vector3Int targetSlotCoordinates = road[Controller.GameplayData.RoadIndex];
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

                CharacterControllerData gameplayData = Controller.GameplayData;
                if (slotToGo == targetSlot)
                {
                    switch (gameplayData.RoadFollowMode)
                    {
                        case RoadFollowMode.PingPong:
                            gameplayData.RoadIndex += _characterFollowPingPongRoadClockwise ? 1 : -1;
                            if (_characterFollowPingPongRoadClockwise && gameplayData.RoadIndex >= road.Length)
                            {
                                _characterFollowPingPongRoadClockwise = false;
                                gameplayData.RoadIndex = road.Length - 2;
                            }
                            else if (_characterFollowPingPongRoadClockwise == false && gameplayData.RoadIndex < 0)
                            {
                                _characterFollowPingPongRoadClockwise = true;
                                gameplayData.RoadIndex = 1;
                            }
                            break;
                        case RoadFollowMode.Loop:
                            gameplayData.RoadIndex++;
                            if (gameplayData.RoadIndex >= road.Length - 1)
                            {
                                gameplayData.RoadIndex = 0;
                            }
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            baseCharacterSlot.Data.Character = Controller;

            if (slotToGo == null)
            {
                return;
            }
            
            Controller.GameplayData.Orientation = WorldOrientation.GetDirection(path[^2].Coordinates, path[^1].Coordinates);
            
            Controller.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[]{path});
        }

        public void MoveRandomly()
        {
            List<SlotController> accessibleSlots = Controller.AccessibleSlots;
            SlotController targetSlot = accessibleSlots[Random.Range(0, accessibleSlots.Count)];
            List<SlotController> path = GameManager.Instance.Board.GetPath(Controller.CurrentSlot, targetSlot);
            Controller.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[]{path});
        }
        
        #endregion
        
        #region Static Movement

        private void Rotate()
        {
            WorldOrientation.Orientation orientation = Controller.GameplayData.Orientation;
            orientation++;
            if ((int)orientation > 3)
            {
                orientation = 0;
            }
            Controller.GameplayData.Orientation = orientation;
            
            Controller.OnCharacterAction.Invoke(CharacterAction.Rotate, new object[]{orientation});
        }
        
        #endregion
    }
}