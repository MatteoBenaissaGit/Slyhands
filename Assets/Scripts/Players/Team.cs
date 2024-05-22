using System;
using System.Collections.Generic;
using Board.Characters;
using UnityEngine;

namespace Players
{
    [Serializable]
    public class Team
    {
        [field:SerializeField] public int TeamNumber { get; private set; }
        [field:SerializeField] public Color TeamColor { get; private set; }
        
        public Player Player { get; private set; }
        public List<BoardCharacterController> Characters { get; set; }
        
        [SerializeField] private PlayerType _teamType;

        public void Initialize()
        {
            Characters = new List<BoardCharacterController>();
            Player = new Player(_teamType, this);
        }

        public void MakeTurn()
        {
            Characters.ForEach(x => x.SetForNewTurn());
            Player.PlayBehavior?.StartTurn();
        }
    }
}