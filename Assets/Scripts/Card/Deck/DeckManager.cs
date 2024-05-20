using Slots;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another Deck Manager in this scene !");
        }
    }

    public void PlayCardOnLocation(CardController card, SlotLocation slot)
    {
        Debug.Log($"{card.name} has been applied on {slot.name}");
        card.DiscardPosition = CardManager.Instance.DiscardPile.transform.position;
        card.DiscardScale = card.IdleScale;
        card.DiscardRotation = new Vector3(-90, 180, 0);

        card.CardPlacement = CardPlacement.Discard;
    }
}