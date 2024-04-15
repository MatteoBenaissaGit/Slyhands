using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> CardsInHand;

    public float cardSpacing = 1.0f;

    void Start()
    {
        ArrangeHand();
    }

    private void Update()
    {
        ArrangeHand();
    }


    void ArrangeHand()
    {
        // Arrange the cards in the hand
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            if (CardsInHand[i].GetComponentInParent<CardController>().IsSelected == false)
            {
                Vector3 newPosition = transform.position + new Vector3(i * cardSpacing, 0f, 0f);
                CardsInHand[i].transform.parent.transform.position = newPosition;
            }
        }
    }
}