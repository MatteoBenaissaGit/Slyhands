using System;
using System.Collections.Generic;
using Board.Characters;
using UnityEngine;

namespace Players
{
    [Serializable]
    public class Team
    {
        [field:SerializeField] public int Number { get; private set; }
        [field:SerializeField] public Color Color { get; private set; }
        [field:SerializeField] public PlayerType Type { get; private set; }
        
        public Player Player { get; private set; }
        public List<BoardCharacterController> Characters { get; set; }
        

        public Team(int number, Color color, PlayerType type)
        {
            Number = number;
            Color = color;
            Type = type;
        }
        
        public void Initialize()
        {
            Characters = new List<BoardCharacterController>();
            Player = new Player(Type, this);
        }

        public void MakeTurn()
        {
            Characters.ForEach(x => x.SetForNewTurn());
            Player.PlayBehavior?.StartTurn();
        }
    }
}