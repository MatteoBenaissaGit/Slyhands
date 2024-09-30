namespace Board.Characters
{
    public abstract class BoardCharacterState
    {
        protected BoardCharacterController Controller { get; private set; }
        
        public BoardCharacterState(BoardCharacterController controller)
        {
            Controller = controller;
        }
        
        public abstract void Start();
        public abstract void Play();
        public abstract void Quit();
    }
}