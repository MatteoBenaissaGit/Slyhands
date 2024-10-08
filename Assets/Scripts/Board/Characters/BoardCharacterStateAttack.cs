namespace Board.Characters
{
    public class BoardCharacterStateAttack : BoardCharacterState
    {
        public BoardCharacterController EnemyAttacked { get; set; }
        
        public BoardCharacterStateAttack(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            
        }

        public override void Play()
        {
            
        }

        public override void Quit()
        {
            Controller.OnCharacterAction.Invoke(CharacterAction.EnemyLost, new object[]{EnemyAttacked.Coordinates});
        }
    }
}