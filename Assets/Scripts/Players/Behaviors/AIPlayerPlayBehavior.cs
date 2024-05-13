using System.Threading.Tasks;
using GameEngine;
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
            GameManager.Instance.Task.EnqueueTask(PlayTurn);
        }

        public override void EndTurn()
        {
        }

        private async Task PlayTurn()
        {
            await Task.Delay(1000);
            GameManager.Instance.SetNextTurn();
        }
    }
}