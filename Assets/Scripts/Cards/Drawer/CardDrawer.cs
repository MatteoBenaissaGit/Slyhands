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
        print("bla");
        StartCoroutine(SpawnCards(nbCardsToDraw));
    }

    private IEnumerator SpawnCards(int nbCardsToDraw)
    {
        print($"ble : {nbCardsToDraw}");

        for (int i = 0; i < nbCardsToDraw; i++)
        {
            CardHand cardHand = CardManager.Instance.GameplayDeckManager.CardHand;
            
            if (cardHand.cardsInHand.Count < cardHand.MaxCardsInHand)
            {
                if (CardsInDrawer.Count == 0)
                {
                    CardManager.Instance.GameplayDeckManager.ResetDrawerFromDiscardPile();
                }

                print("bli");

                GameObject newCard = Instantiate(CardManager.Instance.GameplayDeckManager.CardPrefab, transform.position,
                    Quaternion.identity);

                newCard.GetComponent<CardController>().Data = CardsInDrawer[0];

                cardHand.cardsInHand.Add(newCard.transform);

                CardsInDrawer.RemoveAt(0);

                newCard.name = "Card " + nbCardsCreate;

                nbCardsCreate++;
                
                print("blo");


                yield return new WaitForSeconds(_timeBetweenCardSpawn);
                
                print("blu");

            }
        }
    }
}