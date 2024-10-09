using System.Collections;
using System.Collections.Generic;
using Data.Cards;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Card/DeckData", order = 1)]
public class DeckData : ScriptableObject
{
    public List<CardData> CardsInDeck;
    public string Name;
}
