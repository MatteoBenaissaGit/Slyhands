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
        public Team Team { get; private set; }
        public PlayerPlayBehavior PlayBehavior { get; private set; }

        public Player(PlayerType type, Team team)
        {
            Type = type;
            Team = team;
            ID = Guid.NewGuid().ToString();
            
            //Debug.Log($"{Type} player {ID} created for team {Team}");

            PlayBehavior = type switch
            {
                PlayerType.Local => new LocalPlayerPlayBehavior(this),
                PlayerType.AI => new AIPlayerPlayBehavior(this),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}