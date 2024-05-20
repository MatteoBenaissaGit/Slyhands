using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class CardHand : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;

    [SerializeField] private Vector3 _cardSpacingInHand;
    [SerializeField] private Vector3 _overedOffsetPosition;
    [SerializeField] private float _overedOffsetScale;

    public List<Transform> cards = new List<Transform>();

    private CardController _currentCardControllerOvered;

    public void AddCard()
    {
        GameObject newCard = Instantiate(CardPrefab, transform.position, Quaternion.identity);
        cards.Add(newCard.transform);

        newCard.name = "Card " + cards.Count;
    }

    public void RemoveCard()
    {
        cards.Remove(CardManager.Instance.CardHovered.transform);
    }

    private void Update()
    {
        _currentCardControllerOvered = CardManager.Instance.CardHovered;

        foreach (var card in cards)
        {
            if (card.GetComponent<CardController>() != _currentCardControllerOvered)
            {
                card.GetComponent<CardController>().IsHovered = false;
            }
        }

        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        float totalWidth = (cards.Count - 1) * _cardSpacingInHand.x;
        float startX = transform.position.x + (-totalWidth / 2);

        for (int i = 0; i < cards.Count; i++)
        {
            float xPosition = startX + i * _cardSpacingInHand.x;
            float yPosition = transform.position.y + i * _cardSpacingInHand.y;
            float zPosition = transform.position.z + i * _cardSpacingInHand.z;

            CardController cardController = cards[i].GetComponent<CardController>();

            //POSITION

            //Idle position
            cardController.IdlePosition =
                new Vector3(xPosition, yPosition, zPosition);

            //Overed position
            cardController.HoveredPosition = cardController.IdlePosition + _overedOffsetPosition;

            //Minor position
            if (_currentCardControllerOvered != null)
            {
                if (cardController.IsHovered)
                {
                    cardController.MinorPosition = cardController.IdlePosition;

                    cardController.CardPlacement = CardPlacement.Overed;
                }
                else
                {
                    //Basic spacing + spacing by card index difference
                    float cardMinorPositionX;
                    if (i < cards.IndexOf(CardManager.Instance.CardHovered.transform))
                    {
                        cardMinorPositionX = cardController.IdlePosition.x - _cardSpacingInHand.x / 2;
                    }
                    else
                    {
                        cardMinorPositionX = cardController.IdlePosition.x + _cardSpacingInHand.x / 2;
                    }

                    cardController.MinorPosition = new Vector3(cardMinorPositionX, cardController.IdlePosition.y,
                        cardController.IdlePosition.z);


                    cardController.CardPlacement = CardPlacement.Minor;
                }
            }


            //SCALE

            //Idle Scale

            //Overed Scale
            cardController.HoveredScale = cardController.IdleScale * _overedOffsetScale;


            //ROTATION
            cards[i].localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
}