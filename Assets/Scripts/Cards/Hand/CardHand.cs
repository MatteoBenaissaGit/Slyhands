using UnityEngine;
using System.Collections.Generic;
using Board.Characters;
using Inputs;
using Sirenix.OdinInspector;
using Slots;

public class CardHand : MonoBehaviour
{
    [BoxGroup("References")] public UnityEngine.Camera BoardCamera;
    [BoxGroup("References")] public UnityEngine.Camera CardCamera;

    [PropertySpace(SpaceBefore = 30, SpaceAfter = 30), BoxGroup("References")] public List<Transform> cardsInHand = new();

    [BoxGroup("Hand Properties")] public int MaxCardsInHand;
    [Space, BoxGroup("Hand Properties")] public float HandCardsMovementSpeed;
    
    [BoxGroup("Card Selected Movement"), SerializeField] public Vector3 SelectedOffsetPosition;
    [BoxGroup("Card Selected Movement"), SerializeField] public float SelectedOffsetScale; 
    [Space, BoxGroup("Card Selected Movement"), SerializeField] public float SmoothMovementSpeed;
    [BoxGroup("Card Selected Movement"), SerializeField] public float SmoothRotationSpeed;
    [BoxGroup("Card Selected Movement"), SerializeField] public float SmoothScalingSpeed;

    
    [Space, BoxGroup("Card Selected Movement"), SerializeField] private float _clampValueRotation;
    [Space, BoxGroup("Hand Properties"), SerializeField] private Vector3 _cardSpacingInHand;
    [BoxGroup("On Card Hovered")] [SerializeField] private Vector3 _hoveredOffsetPosition;
    [Space, BoxGroup("On Card Hovered"), SerializeField] private float _hoveredOffsetScale;
    [Space(20), BoxGroup("On Card Hovered")] [SerializeField] private Vector3 _minorOffsetPosition;
    
    private CardController _cardHovered;
    private CardController _cardSelected;
    
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
        cardControllerInput.OnMouseStopMoved += ResetCardMoved;
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

            cardController.IdlePosition = new Vector3(xPosition, yPosition, zPosition);
            cardController.HoveredPosition = cardController.IdlePosition + _hoveredOffsetPosition;

            if (_cardHovered != null && cardsInHand[i].gameObject != _cardHovered.gameObject)
            {
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

            cardController.HoveredScale = cardController.IdleScale * _hoveredOffsetScale;
            cardsInHand[i].localRotation = Quaternion.Euler(-90, 0, 0);
        }
    }

    /// <summary>
    /// Make raycasts to find a card to hovered
    /// </summary>
    private void DetectCardToHovered()
    {
        if (CardCamera == null) return;

        Ray ray = CardCamera.ScreenPointToRay(Input.mousePosition);
        int hits = Physics.RaycastNonAlloc(ray, _cardHits);

        if (hits != 0)
        {
            for (int i = 0; i < hits; i++)
            {
                if (!_cardHits[i].collider.TryGetComponent(out CardController card) || card.cardStatus == CardStatus.Discarded)
                {
                    continue;
                }
                if (_cardHovered != null)
                {
                    if (_cardHits[i].collider.gameObject == _cardHovered.gameObject) 
                    {
                        return;
                    }
                    _cardHovered = card;
                    return;
                }
                _cardHovered = card;
                return;
            }

            ResetCardsStatus();
        }
        else if (_cardHovered != null)
        {
            ResetCardsStatus();
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
        if (_cardSelected == null) 
        {
            DetectCardToHovered(); 
        }
        else
        {
            SetSelectedCardTransform();
            _cardSelected.cardStatus = CardStatus.Dragged;
        }

        if (_cardHovered != null)
        {
            _cardHovered.cardStatus = CardStatus.InHandHovered; 
        }
    }

    /// <summary>
    /// Called when player makes left mouse button click
    /// </summary>
    private void SetSelectedCard()
    {
        if (_cardHovered != null)
        {
            _cardSelected = _cardHovered; 
            _offsetZCardCamera = _cardSelected.transform.position.z - CardCamera.transform.position.z; 
            _cardSelectedIndex = cardsInHand.IndexOf(_cardHovered.transform);
            cardsInHand.Remove(_cardHovered.transform);
            _cardHovered = null; 
        }
    }

    private void ResetCardMoved()
    {
        if (_cardSelected != null)
        {
            Vector3 newPosition = GetMouseWorldPosition();

            Quaternion toRotation = Quaternion.Euler(-90, 0, 0);

            _cardSelected.TargetPosition = newPosition;
            _cardSelected.TargetRotation = toRotation;
        }
    }

    /// <summary>
    /// Called when there is a selected card
    /// </summary>
    private void SetSelectedCardTransform()
    {
        Vector3 newPosition = GetMouseWorldPosition(); 
        
        float yDifference = Mathf.Clamp((newPosition.y - _cardSelected.transform.position.y), -_clampValueRotation / 10, _clampValueRotation / 10);
        float xDifference = Mathf.Clamp((newPosition.x - _cardSelected.transform.position.x), -_clampValueRotation / 10, _clampValueRotation / 10);

        Quaternion toRotation = Quaternion.Euler(-90 + (yDifference) * 90, -(xDifference) * 90, 0);

        _cardSelected.TargetPosition = newPosition;
        _cardSelected.TargetRotation = toRotation;
    }

    /// <summary>
    /// Called in SetSelectedCardTransform
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _offsetZCardCamera + SelectedOffsetPosition.z; 
        return CardCamera.ScreenToWorldPoint(mousePoint) + SelectedOffsetPosition;
    }

    /// <summary>
    /// Check if there is a slot under the card selected
    /// </summary>
    private void CheckSlotHovered()
    {
        if (_cardSelected == null)
        {
            return;
        }
        
        Ray ray = BoardCamera.ScreenPointToRay(Input.mousePosition);
        int hits = Physics.RaycastNonAlloc(ray, _slotHits);
        if (hits != 0)
        {
            for (int i = 0; i < hits;)
            {
                if (_slotHits[i].collider.TryGetComponent(out SlotLocation slot) == false)
                {
                    AddCardFromMouseToHand();
                    return;
                }

                slot = _slotHits[i].collider.GetComponent<SlotLocation>();

                if (slot.SlotView.Controller.HasCharacter(out var character) == false)
                {
                    AddCardFromMouseToHand();
                    return;
                }

                _cardSelected.cardStatus = CardStatus.Discarded;
                CardManager.Instance.GameplayDeckManager.PlayCardOnLocation(_cardSelected, slot);
                _cardSelected = null;
                character.OnCharacterAction.Invoke(CharacterAction.Stun, new object []{3});
                return;
            }
        }
        else
        {
            AddCardFromMouseToHand();
        }
    }

    /// <summary>
    /// Called when left mouse click up and there is no slot location under the card selected
    /// </summary>
    private void AddCardFromMouseToHand()
    {
        cardsInHand.Insert(_cardSelectedIndex, _cardSelected.transform);
        _cardSelected.cardStatus = CardStatus.InHand;
        _cardSelected = null;
    }
}