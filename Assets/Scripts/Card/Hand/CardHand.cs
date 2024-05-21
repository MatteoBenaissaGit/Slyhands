using UnityEngine;
using System.Collections.Generic;

public class CardHand : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;

    [SerializeField] private Vector3 _cardSpacingInHand;
    [SerializeField] private Vector3 _hoveredOffsetPosition;
    [SerializeField] private float _overedOffsetScale;

    public List<Transform> cards = new List<Transform>();

    [SerializeField] private CardController _currentCardControllerHovered;

    //Debug
    private int nbCardsCreate = 1;

    public void AddCard()
    {
        GameObject newCard = Instantiate(CardPrefab, transform.position, Quaternion.identity);
        cards.Add(newCard.transform);

        newCard.name = "Card " + nbCardsCreate;

        nbCardsCreate++;
    }

    public void RemoveCard()
    {
        cards.Remove(CardManager.Instance.CardHovered.transform);
    }

    private void Update()
    {
        _currentCardControllerHovered = CardManager.Instance.CardHovered ? CardManager.Instance.CardHovered : null;

        foreach (var card in cards)
        {
            CardController cardController = card.GetComponent<CardController>();

            if (_currentCardControllerHovered != null)
            {
                if (cardController != _currentCardControllerHovered)
                {
                    cardController.cardStatus = CardStatus.InHandMinor;
                }
            }
            else
            {
                cardController.cardStatus = CardStatus.InHand;
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
            cardController.IdlePosition = new Vector3(xPosition, yPosition, zPosition);

            //Overed position
            cardController.HoveredPosition = cardController.IdlePosition + _hoveredOffsetPosition;

            //Minor position
            if (_currentCardControllerHovered != null && cards[i].gameObject != _currentCardControllerHovered.gameObject)
            {
                //Basic spacing + spacing by card index difference
                float cardMinorPositionX;
                if (i < cards.IndexOf(_currentCardControllerHovered.transform))
                {
                    cardMinorPositionX = cardController.IdlePosition.x - _cardSpacingInHand.x / 2;
                }
                else
                {
                    cardMinorPositionX = cardController.IdlePosition.x + _cardSpacingInHand.x / 2;
                }

                cardController.MinorPosition = new Vector3(cardMinorPositionX, cardController.IdlePosition.y,
                    cardController.IdlePosition.z);
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