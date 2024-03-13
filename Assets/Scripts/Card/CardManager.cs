using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardsSlots;
}
