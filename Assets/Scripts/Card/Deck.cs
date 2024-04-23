using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> CardsInDeck;

    [SerializeField] private GameObject _cardPrefab;
    
    private void OnMouseUp()
    {
        var currentCard = CardsInDeck[0];
        currentCard.cardStatus = CardAction.Drawed;
        
        CardManager.Instance.Hand.CardsInHand.Add(currentCard);
        CardsInDeck.Remove(currentCard);
    }
}
