using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Board.Characters;
using GameEngine;
using LevelEditor.Entities;
using Slots;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Players.Behaviors
{
    public class AIPlayerPlayBehavior : PlayerPlayBehavior
    {
        public AIPlayerPlayBehavior(Player player) : base(player)
        {
        }
        
        public override void StartTurn()
        {
            GameManager.Instance.TaskManager.EnqueueTask(PlayTurn);
        }

        public override void EndTurn()
        {
        }

        private async Task PlayTurn()
        {
            foreach (BoardCharacterController character in Player.Team.Characters)
            {
                await PlayCharacter(character);
            }

            GameManager.Instance.TaskManager.EnqueueTask(GameManager.Instance.SetNextTurn);
        }

        private async Task PlayCharacter(BoardCharacterController character)
        {
            character.OnCharacterAction.Invoke(CharacterAction.IsSelected, null);

            //TODO if state idle/base/attack/search etc...
            
            if (HasPredefinedRoad(character))
            {
                MoveCharacterAlongSideRoad(character);
            }
            else
            { 
                MoveCharacterRandomly(character);
            }
                
            character.OnCharacterAction.Invoke(CharacterAction.IsUnselected, null);
        }

        private bool _characterFollowPingPongRoadClockwise = true;
        
        private void MoveCharacterAlongSideRoad(BoardCharacterController character)
        {
            Vector3Int[] road = character.GameplayData.Road;

            BoardController board = GameManager.Instance.Board;
            SlotController slotToGo = null;
            List<SlotController> path = new List<SlotController>();
            SlotController currentCharacterSlot = character.CurrentSlot;

            int iterations = 0;
            while (character.GameplayData.CurrentMovementPoints > 0)
            {
                //loop security
                iterations++;
                if (iterations > 100)
                {
                    Debug.Log("Max iterations reached");
                    break;
                }
                
                //get the target slot
                Vector3Int targetSlotCoordinates = road[character.GameplayData.RoadIndex];
                SlotController targetSlot = board.GetSlotFromCoordinates(targetSlotCoordinates);
                if (targetSlot.IsAccessible == false && board.GetClosestToSlotFromSlot(targetSlot, currentCharacterSlot, out slotToGo) == false)
                {
                    slotToGo = currentCharacterSlot;
                    break;
                }

#if UNITY_EDITOR
                Debug.DrawLine(currentCharacterSlot.Location.transform.position, targetSlot.Location.transform.position, Color.red, 2f);                
#endif

                //get the path to target within accessible slots
                List<SlotController> currentPath = board.GetPathFromSlotToSlot(currentCharacterSlot, slotToGo ?? targetSlot);
                character.UpdateAccessibleSlots();
                List<SlotController> accessibleSlots = character.AccessibleSlots;
                currentPath.RemoveAll(x => accessibleSlots.Contains(x) == false);
                path.AddRange(currentPath);
                
                if (path.Count == 0)
                {
                    break;
                }
                
                character.GameplayData.CurrentMovementPoints -= currentPath.Count + 1;
                slotToGo = path[^1];
                currentCharacterSlot = slotToGo;
                
                if (slotToGo == targetSlot)
                {
                    switch (character.GameplayData.RoadFollowMode)
                    {
                        case RoadFollowMode.PingPong:
                            character.GameplayData.RoadIndex += _characterFollowPingPongRoadClockwise ? 1 : -1;
                            if (_characterFollowPingPongRoadClockwise && character.GameplayData.RoadIndex >= road.Length)
                            {
                                _characterFollowPingPongRoadClockwise = false;
                                character.GameplayData.RoadIndex = road.Length - 2;
                            }
                            else if (_characterFollowPingPongRoadClockwise == false && character.GameplayData.RoadIndex < 0)
                            {
                                _characterFollowPingPongRoadClockwise = true;
                                character.GameplayData.RoadIndex = 1;
                            }
                            break;
                        case RoadFollowMode.Loop:
                            character.GameplayData.RoadIndex++;
                            if (character.GameplayData.RoadIndex >= road.Length - 1)
                            {
                                character.GameplayData.RoadIndex = 0;
                            }
                            break;
                    }
                }
            }

            if (slotToGo == null)
            {
                MoveCharacterRandomly(character);
                return;
            }
            
            character.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[]{path});
        }

        private void MoveCharacterRandomly(BoardCharacterController character)
        {
            List<SlotController> accessibleSlots = character.AccessibleSlots;
            SlotController targetSlot = accessibleSlots[Random.Range(0, accessibleSlots.Count)];
            List<SlotController> path = GameManager.Instance.Board.GetPathFromSlotToSlot(character.CurrentSlot, targetSlot);
            character.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[]{path});
        }
        
        private bool HasPredefinedRoad(BoardCharacterController character)
        {
            return character.GameplayData.Road != null && character.GameplayData.Road.Length > 1;
        }
    }
}