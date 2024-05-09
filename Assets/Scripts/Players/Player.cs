using System;
using System.Collections.Generic;
using Players.Behaviors;
using UnityEngine;

namespace Players
{
    /// <summary>
    /// The type of the player (local, AI)
    /// </summary>
    public enum PlayerType
    {
        Local = 0,
        AI = 1
    }
    
    /// <summary>
    /// This class handle the global logic of a game player and contains different behaviors depending on its type
    /// </summary>
    public class Player
    {
        public PlayerType Type { get; private set; }
        public string ID { get; private set; }
        public int Team { get; private set; }
        public PlayerPlayBehavior PlayBehavior { get; private set; }

        public Player(PlayerType type, int team)
        {
            Type = type;
            Team = team;
            ID = Guid.NewGuid().ToString();
            
            Debug.Log($"{Type} player {ID} created for team {Team}");

            PlayBehavior = type switch
            {
                PlayerType.Local => new LocalPlayerPlayBehavior(),
                PlayerType.AI => new AIPlayerPlayBehavior(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}