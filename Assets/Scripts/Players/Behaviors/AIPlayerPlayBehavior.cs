using System.Collections.Generic;
using System.Threading.Tasks;
using Board.Characters;
using GameEngine;
using Slots;
using UnityEngine;

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
            character.OnCharacterAction.Invoke(CharacterAction.IsSelected);

            //if state idle/base
            if (HasPredefinedRoad(character))
            {
                MoveCharacterAlongSideRoad(character);
            }
            else
            {
                MoveCharacterRandomly(character);
            }
                
            character.OnCharacterAction.Invoke(CharacterAction.IsUnselected);
        }

        private void MoveCharacterAlongSideRoad(BoardCharacterController character)
        {
            int movementPoints = character.GameplayData.CurrentMovementPoints;
            Vector3Int[] road = character.GameplayData.Road;

            SlotController slotToGo = null;
            SlotController currentCharacterSlot = character.CurrentSlot;
            while (movementPoints > 0)
            {
                //get the target slot
                Vector3Int targetSlotCoordinates = road[character.GameplayData.RoadIndex + 1];
                SlotController targetSlot = GameManager.Instance.Board.GetSlotFromCoordinates(targetSlotCoordinates);

                //get the path to target within accessible slots
                List<SlotController> pathToTarget = GameManager.Instance.Board.GetPathFromSlotToSlot(currentCharacterSlot, targetSlot);
                List<SlotController> accessibleSlots = character.AccessibleSlots;
                pathToTarget.RemoveAll(x => accessibleSlots.Contains(x) == false);
                
                if (pathToTarget.Count == 0)
                {
                    break;
                }
                
                movementPoints -= pathToTarget.Count + 1;
                slotToGo = pathToTarget[^1];
                currentCharacterSlot = slotToGo;
                
                if (slotToGo == targetSlot)
                {
                    character.GameplayData.RoadIndex++;
                    if (character.GameplayData.RoadIndex >= road.Length - 1)
                    {
                        //TODO only for loop  
                        character.GameplayData.RoadIndex = 0;
                    }
                }
            }

            if (slotToGo == null)
            {
                MoveCharacterRandomly(character);
                return;
            }
            
            character.OnCharacterAction.Invoke(CharacterAction.MoveTo, slotToGo.Coordinates);
        }

        private void MoveCharacterRandomly(BoardCharacterController character)
        {
            List<SlotController> accessibleSlots = character.AccessibleSlots;
            SlotController targetSlot = accessibleSlots[Random.Range(0, accessibleSlots.Count)];
            character.OnCharacterAction.Invoke(CharacterAction.MoveTo, targetSlot.Coordinates);
        }
        
        private bool HasPredefinedRoad(BoardCharacterController character)
        {
            return character.GameplayData.Road != null && character.GameplayData.Road.Length > 1;
        }
    }
}