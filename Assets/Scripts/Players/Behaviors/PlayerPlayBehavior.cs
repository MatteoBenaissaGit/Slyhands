namespace Players.Behaviors
{
    public abstract class PlayerPlayBehavior
    {
        protected Player Player { get; set; }
        
        public PlayerPlayBehavior(Player player)
        {
            Player = player;
        }
        
        public abstract void StartTurn();
        public abstract void EndTurn();
    }
}