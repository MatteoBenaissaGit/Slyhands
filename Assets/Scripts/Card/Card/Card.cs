using Data.Cards;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData Data;
    public CardVisual CardVisual;

    private void Start()
    {
        CardVisual.Initialize(Data);
    }
}