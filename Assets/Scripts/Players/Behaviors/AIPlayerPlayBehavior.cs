using System.Threading.Tasks;
using Board.Characters;
using GameEngine;

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
                await character.PlayTurn();
            }

            GameManager.Instance.TaskManager.EnqueueTask(GameManager.Instance.SetNextTurn);
        }
    }
}