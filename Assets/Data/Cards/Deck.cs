using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Cards
{
    [Serializable]
    public class Deck
    {
        public List<byte> IDCardInDeck;
        public string Name = "";
    }
}