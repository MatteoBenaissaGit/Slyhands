using System.Collections;
using System.Collections.Generic;
using Data.Cards;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    public List<CardData> CardsInDrawer;

    [SerializeField] private float _timeBetweenCardSpawn;


    //Debug
    private int nbCardsCreate = 1;

    public void DrawCards(int nbCardsToDraw)
    {
        StartCoroutine(SpawnCards(nbCardsToDraw));
    }

    private IEnumerator SpawnCards(int nbCardsToDraw)
    {
        for (int i = 0; i < nbCardsToDraw; i++)
        {
            CardHand cardHand = CardManager.Instance.DeckManager.CardHand;
            
            if (cardHand.cardsInHand.Count < cardHand.NumberMaxCardsInHand)
            {
                if (CardsInDrawer.Count == 0)
                {
                    CardManager.Instance.DeckManager.ResetDrawerFromDiscardPile();
                }

                GameObject newCard = Instantiate(CardManager.Instance.DeckManager.CardPrefab, transform.position,
                    Quaternion.identity);

                newCard.GetComponent<CardController>().Data = CardsInDrawer[0];

                cardHand.cardsInHand.Add(newCard.transform);

                CardsInDrawer.RemoveAt(0);

                newCard.name = "Card " + nbCardsCreate;

                nbCardsCreate++;

                yield return new WaitForSeconds(_timeBetweenCardSpawn);
            }
        }
    }
}