namespace Board.Characters
{
    public class BoardCharacterStateStunned : BoardCharacterState
    {
        public int Duration = 0;
        
        public BoardCharacterStateStunned(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            CanPlay = false;
            Controller.UnsubscribeToDetection();
        }

        public override void Play()
        {
            Duration--;
            if (Duration <= 0)
            {
                Controller.OnCharacterAction?.Invoke(CharacterAction.EndStun, null);
                Controller.SetState(Controller.PatrolState);
                return;
            }
            
            Controller.OnCharacterAction?.Invoke(CharacterAction.UpdateStun, null);
        }

        public override void Quit()
        {
            CanPlay = true;
            Controller.SubscribeToDetection();
        }
    }
}