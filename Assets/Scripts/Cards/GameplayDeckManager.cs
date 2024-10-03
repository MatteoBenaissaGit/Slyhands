using System;
using System.Collections;
using System.Collections.Generic;
using Slots;
using UnityEngine;

public class GameplayDeckManager : MonoBehaviour
{
    [field: SerializeField] public GameObject CardPrefab { get; private set; }
    [field: SerializeField] public DeckData DeckData { get; private set; }
    [field: SerializeField] public CardHand CardHand { get; private set; }
    [field: SerializeField] public CardDrawer CardDrawer { get; private set; }
    [field: SerializeField] public DiscardPile DiscardPile { get; private set; }
    
    private void Start()
    {
        DrawerInitialisation();
    }

    private void DrawerInitialisation()
    {
        foreach (var cardData in DeckData.CardsInDeck)
        {
            CardDrawer.CardsInDrawer.Add(cardData);
        }
    }

    public void ResetDrawerFromDiscardPile()
    {
        DiscardPile.RandomizeDiscardPile();

        foreach (var cardData in DiscardPile.CardsInDiscardPile)
        {
            CardDrawer.CardsInDrawer.Add(cardData);
        }
        
        DiscardPile.CardsInDiscardPile.Clear();
    }
    
    public void PlayCardOnLocation(CardController card, SlotLocation slot)
    {
        Debug.Log($"{card.name} has been applied on {slot.name}");
        card.DiscardPosition = DiscardPile.transform.position;
        card.DiscardScale = card.IdleScale;
        card.DiscardRotation = new Vector3(-90, 180, 0);

        card.cardStatus = CardStatus.Discarded;
        
        DiscardPile.CardsInDiscardPile.Add(card.Data);
        
        StartCoroutine(DestroyCard(card.gameObject));
    }
    
    private IEnumerator DestroyCard(GameObject card)
    {
        yield return new WaitForSeconds(1);
        Destroy(card);
    }
}