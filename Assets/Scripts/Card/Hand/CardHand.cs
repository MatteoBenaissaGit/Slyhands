using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    public List<Transform> cards = new List<Transform>();
    public float cardSpacing = 1.0f;
    public GameObject CardPrefab;

    public void AddCard()
    {
        GameObject newCard = Instantiate(CardPrefab, transform.position, Quaternion.identity);
        cards.Add(newCard.transform);


        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        float totalWidth = (cards.Count - 1) * cardSpacing;
        float startX = transform.position.x + (-totalWidth / 2);

        for (int i = 0; i < cards.Count; i++)
        {
            float xPosition = startX + i * cardSpacing;
            cards[i].GetComponent<CardController>().IdlePosition =
                new Vector3(xPosition, transform.position.y, transform.position.z);
            cards[i].transform.localPosition = cards[i].GetComponent<CardController>().IdlePosition;

            cards[i].localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
}