using UnityEngine;
using System.Collections.Generic;
using Inputs;
using Slots;

public class CardHand : MonoBehaviour
{
    [field:Header("References")]
    [field: SerializeField] public UnityEngine.Camera BoardCamera { get; set; }
    [field: SerializeField] public UnityEngine.Camera CardCamera { get; set; }
    
    [Space(10)]
    public List<Transform> cardsInHand = new List<Transform>();
    private CardController _cardHovered { get; set; }
    private CardController _cardSelected { get; set; }

    [Header("Hand Properties")] public int NumberMaxCardsInHand;
    
    [SerializeField] private Vector3 _cardSpacingInHand;
    [field: SerializeField] public float HandCardsMovementSpeed { get; private set; }
    
    [field: Header("On Card Hovered")]

    [SerializeField] private Vector3 _hoveredOffsetPosition;
    [SerializeField] private float _hoveredOffsetScale;
    
    [Space(10)]
    [SerializeField] private Vector3 _minorOffsetPosition;
    
    [field: Header("Card Selected Movement")]
    [field: SerializeField] public float OffsetZCardSelected { get; private set; }
    [field: SerializeField] public float SmoothMovementSpeed { get; private set; }
    [field: SerializeField] public float SmoothRotationSpeed { get; private set; }
    
    [field: Header("Discard")] public GameObject DiscardPile;
    
    private float _offsetZCardCamera;
    private int _cardSelectedIndex;
    private RaycastHit[] _slotHits = new RaycastHit[16];
    private RaycastHit[] _cardHits = new RaycastHit[8];
    
    private void Start()
    {
        InputCardController cardControllerInput = InputManager.Instance.CardControllerInput;
        cardControllerInput.OnLeftClickDown += SetSelectedCard;
        cardControllerInput.OnLeftClickUp += CheckSlotHovered;
        cardControllerInput.OnMouseMoved += UpdateCardDetection;
    }

    private void Update()
    {
        foreach (var card in cardsInHand)
        {
            CardController cardController = card.GetComponent<CardController>();

            if (_cardHovered != null)
            {
                if (cardController != _cardHovered)
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
        float totalWidth = (cardsInHand.Count - 1) * _cardSpacingInHand.x;
        float startX = transform.position.x + (-totalWidth / 2);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float xPosition = startX + i * _cardSpacingInHand.x;
            float yPosition = transform.position.y + i * _cardSpacingInHand.y;
            float zPosition = transform.position.z + i * _cardSpacingInHand.z;

            CardController cardController = cardsInHand[i].GetComponent<CardController>();

            //POSITION

            //Idle position
            cardController.IdlePosition = new Vector3(xPosition, yPosition, zPosition);

            //Overed position
            cardController.HoveredPosition = cardController.IdlePosition + _hoveredOffsetPosition;

            //Minor position
            if (_cardHovered != null && cardsInHand[i].gameObject != _cardHovered.gameObject)
            {
                //Basic spacing + spacing by card index difference
                Vector3 cardMinorPosition;
                if (i < cardsInHand.IndexOf(_cardHovered.transform))
                {
                    cardMinorPosition = cardController.IdlePosition - _minorOffsetPosition;
                }
                else
                {
                    cardMinorPosition = cardController.IdlePosition + _minorOffsetPosition;
                }

                cardMinorPosition.y = cardController.IdlePosition.y + _minorOffsetPosition.y;

                cardController.MinorPosition = cardMinorPosition;
            }


            //SCALE

            //Idle Scale

            //Overed Scale
            cardController.HoveredScale = cardController.IdleScale * _hoveredOffsetScale;


            //ROTATION
            cardsInHand[i].localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }
    
    /// <summary>
    /// Make raycasts to find a card to hovered
    /// </summary>
    private void DetectCardToHovered()
    {
        //Debug.Log("There is no card selected. Try to hovered a card");

        //Raycast
        Ray ray = CardCamera.ScreenPointToRay(Input.mousePosition);
        int hits = Physics.RaycastNonAlloc(ray, _cardHits);

        if (hits != 0) //Check if there is something in raycast result
        {
            // Debug.Log("There is something on the ray way");

            //Check if ray hits a different card than the current hovered card
            for (int i = 0; i < hits; i++)
            {
                if (_cardHits[i].collider.TryGetComponent(out CardController card) && card.cardStatus != CardStatus.Discarded)
                {
                    // Debug.Log($"{card.name} has found on the ray way");

                    if (_cardHovered != null) //If there is already a card hovered
                    {
                        if (_cardHits[i].collider.gameObject ==
                            _cardHovered.gameObject) //Check if the first card on the ray way is CardHovered
                        {
                            return;
                        }
                        else
                        {
                            _cardHovered = card;
                            return;
                        }
                    }
                    else
                    {
                        _cardHovered = card;
                        return;
                    }
                }
            }

            ResetCardsStatus();
        }
        else
        {
            if (_cardHovered != null)
            {
                ResetCardsStatus();
            }
        }
    }

    private void ResetCardsStatus()
    {
        _cardHovered = null;

        foreach (var card in cardsInHand)
        {
            card.GetComponent<CardController>().cardStatus = CardStatus.InHand;
        }
    }

    private void UpdateCardDetection()
    {
        if (_cardSelected == null) //Check if there isn't card selected
        {
            DetectCardToHovered(); //Check if there is a card to hovered under the cursor
        }
        else
        {
            SetSelectedCardTransform();
            _cardSelected.cardStatus = CardStatus.Dragged; //Change card status from InHandHovered to Dragged
        }

        if (_cardHovered != null)
        {
            _cardHovered.cardStatus = CardStatus.InHandHovered; //Change card status from InHand to InHandHovered
        }
    }

    /// <summary>
    /// Called when player makes left mouse button click
    /// </summary>
    private void SetSelectedCard()
    {
        if (_cardHovered != null) //Check if there is a card hovered. In this case, the card hovered become the selected card.
        {
            _cardSelected = _cardHovered; //Set CardSelected with CardHovered

            _offsetZCardCamera =
                _cardSelected.transform.position.z -
                CardCamera.transform.position.z; //Get distance between card selected and card camera

            _cardSelectedIndex =
                cardsInHand.IndexOf(_cardHovered.transform); //Get index of card selected in the hand if it go back in

            cardsInHand.Remove(_cardHovered.transform); //Remove card selected from the cards in hand list

            _cardHovered = null; //Reset CardHovered
        }
    }

    private void SetSelectedCardTransform() //Called when there is a selected card
    {
        Vector3 newPosition = GetMouseWorldPosition(); //Get the mouse position on the screen

        float clampValue = 0.1f;
        float yDifference = Mathf.Clamp(newPosition.y - _cardSelected.transform.position.y, -clampValue, clampValue);
        float xDifference = Mathf.Clamp(newPosition.x - _cardSelected.transform.position.x, -clampValue, clampValue);
        
        Quaternion toRotation = Quaternion.Euler(-90 + (yDifference) * 90, -(xDifference) * 90, 0);

        //Set target position and rotation to selected card's controller
        _cardSelected.TargetPosition = newPosition;
        _cardSelected.TargetRotation = toRotation;
    }

    private Vector3 GetMouseWorldPosition() //Called in SetSelectedCardTransform
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _offsetZCardCamera + OffsetZCardSelected; //Apply Z offset on mouse position
        return CardCamera.ScreenToWorldPoint(mousePoint);
    }

    private void CheckSlotHovered() //Check if there is a slot under the card selected
    {
        if (_cardSelected != null)
        {
            Ray ray = BoardCamera.ScreenPointToRay(Input.mousePosition);

            int hits = Physics.RaycastNonAlloc(ray, _slotHits);

            if (hits != 0)
            {
                for (int i = 0; i < hits;)
                {
                    if (_slotHits[i].collider.TryGetComponent(out SlotLocation slot) == false)
                    {
                        Debug.Log("Not a slot");
                        AddCardFromMouseToHand();
                        return;
                    }

                    Debug.Log("Slot Detected !");

                    _cardSelected.cardStatus = CardStatus.Discarded;
                    
                    CardManager.Instance.DeckManager.PlayCardOnLocation(_cardSelected, slot);

                    _cardSelected = null;

                    return;
                }
            }
            else
            {
                Debug.Log("No objects detected on drop");
                AddCardFromMouseToHand();
            }
        }
    }

    //Called when left mouse click up and there is no slot location under the card selected
    private void AddCardFromMouseToHand()
    {
        cardsInHand.Insert(_cardSelectedIndex, _cardSelected.transform);
        _cardSelected.cardStatus = CardStatus.InHand;
        _cardSelected = null;
    }
}