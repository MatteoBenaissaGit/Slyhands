using System.Collections.Generic;
using Data.Cards;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    [field: SerializeField] public List<CardData> CardsInDiscardPile { get; set; }

    public void RandomizeDiscardPile()
    {
        Shuffle(CardsInDiscardPile);
    }
    
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}