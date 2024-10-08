using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterStateAlert : BoardCharacterState
    {
        public Vector3Int LastSeenEnemyPosition { get; set; }
        
        public BoardCharacterStateAlert(BoardCharacterController controller) : base(controller)
        {
        }

        public override void Start()
        {
            
        }

        public override void Play()
        {
            Debug.Log("PLAY ALERT");
        }

        public override void Quit()
        {
        }
    }
}