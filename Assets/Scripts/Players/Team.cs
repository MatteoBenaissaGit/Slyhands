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
        
        public List<Player> Players { get; private set; }
        public List<BoardCharacterController> Characters { get; set; } = new List<BoardCharacterController>();
        
        [SerializeField] private int _numberOfPlayers;
        [SerializeField] private PlayerType _teamType;

        public void Initialize()
        {
            Players = new List<Player>();
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                Players.Add(new Player(_teamType, TeamNumber));
            }
        }
    }
}