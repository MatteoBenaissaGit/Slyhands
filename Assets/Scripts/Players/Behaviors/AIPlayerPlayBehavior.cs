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
                character.OnCharacterAction.Invoke(CharacterAction.IsSelected);
                
                List<SlotController> accessibleSlots = character.AccessibleSlots;
                SlotController targetSlot = accessibleSlots[Random.Range(0, accessibleSlots.Count)];
                character.OnCharacterAction.Invoke(CharacterAction.MoveTo, targetSlot.Coordinates);
                
                character.OnCharacterAction.Invoke(CharacterAction.IsUnselected);
            }

            await Task.Delay(1000);
            
            GameManager.Instance.TaskManager.EnqueueTask(GameManager.Instance.SetNextTurn);
        }
    }
}