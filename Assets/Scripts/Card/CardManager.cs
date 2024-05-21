using UnityEngine;
using Inputs;
using Slots;

public enum CardStatus
{
    InHand = 0,
    InHandHovered = 1,
    InHandMinor = 2,
    Dragged = 3,
    Discarded = 4
}

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public UnityEngine.Camera BoardCamera;
    private RaycastHit[] _slotHits = new RaycastHit[16];

    public UnityEngine.Camera CardCamera;
    private RaycastHit[] _cardHits = new RaycastHit[8];

    [field: SerializeField] public CardHand CardHand { get; private set; }


    [field: Header("Card Hovered")]
    [field: SerializeField]
    public CardController CardHovered { get; set; }

    [field: SerializeField] public float HandCardsMovementSpeed { get; private set; }


    [field: Header("Card Selected")]
    [field: SerializeField]
    public CardController CardSelected { get; private set; }

    private int _cardSelectedIndex;


    [field: Header("Card Selected Movement")]
    [field: SerializeField]
    public float OffsetZCardSelected { get; private set; }

    [field: SerializeField] public float SmoothMovementSpeed { get; private set; }
    [field: SerializeField] public float SmoothRotationSpeed { get; private set; }
    public float _offsetZCardCamera;


    [field: Header("Discard")] public GameObject DiscardPile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another Card Manager in this scene !");
        }
    }

    private void Start()
    {
        InputCardController cardControllerInput = InputManager.Instance.CardControllerInput;

        cardControllerInput.OnLeftClickDown += SetSelectedCard;
        cardControllerInput.OnLeftClickUp += CheckSlotHovered;
    }

    private void DetectCardToHovered() //Make raycasts to find a card to hovered
    {
        Debug.Log("There is no card selected. Try to hovered a card");

        //Raycast
        Ray ray = CardCamera.ScreenPointToRay(Input.mousePosition);
        int hits = Physics.RaycastNonAlloc(ray, _cardHits);

        if (hits != 0) //Check if there is something in raycast result
        {
            Debug.Log("There is something on the ray way");

            //Check if ray hits a different card than the current hovered card
            for (int i = 0; i < hits; i++)
            {
                if (_cardHits[i].collider.TryGetComponent(out CardController card))
                {
                    Debug.Log($"{card.name} has found on the ray way");

                    if (CardHovered != null) //If there is already a card hovered
                    {
                        if (_cardHits[i].collider.gameObject ==
                            CardHovered.gameObject) //Check if the first card on the ray way is CardHovered
                        {
                            return;
                        }
                        else
                        {
                            CardHovered = card;
                            return;
                        }
                    }
                    else
                    {
                        CardHovered = card;
                        return;
                    }
                }
            }

            ResetCardsStatus();
        }
        else
        {
            if (CardHovered != null)
            {
                ResetCardsStatus();
            }
        }
    }

    private void ResetCardsStatus()
    {
        CardHovered = null;

        foreach (var card in CardHand.cards)
        {
            card.GetComponent<CardController>().cardStatus = CardStatus.InHand;
        }
    }

    private void Update()
    {
        if (CardSelected == null) //Check if there isn't card selected
        {
            DetectCardToHovered(); //Check if there is a card to hovered under the cursor
        }

        if (CardSelected != null)
        {
            SetSelectedCardTransform();
        }

        if (CardHovered != null)
            CardHovered.cardStatus = CardStatus.InHandHovered; //Change card status from InHand to InHandHovered

        if (CardSelected != null)
            CardSelected.cardStatus = CardStatus.Dragged; //Change card status from InHandHovered to Dragged
    }

    private void SetSelectedCard() //Called when player make left mouse button click
    {
        if (CardHovered !=
            null) //Check if there is a card hovered. In this case, the card hovered become the selected card.
        {
            CardSelected = CardHovered; //Set CardSelected with CardHovered
            
            _offsetZCardCamera =
                CardSelected.transform.position.z -
                CardCamera.transform.position.z; //Get distance between card selected and card camera

            _cardSelectedIndex = CardHand.cards.IndexOf(CardHovered.transform); //Get index of card selected in the hand if it go back in
            
            CardHand.cards.Remove(CardHovered.transform); //Remove card selected from the cards in hand list
            
            CardHovered = null; //Reset CardHovered
        }
    }

    private void SetSelectedCardTransform() //Called when there is a selected card
    {
        Vector3 newPosition = GetMouseWorldPosition(); //Get the mouse position on the screen
        Quaternion toRotation = Quaternion.Euler(-90 + (newPosition.y - CardSelected.transform.position.y) * 90,
            -(newPosition.x - CardSelected.transform.position.x) * 90, 0);

        //Set target position and rotation to selected card's controller
        CardSelected.TargetPosition = newPosition;
        CardSelected.TargetRotation = toRotation;
    }

    private Vector3 GetMouseWorldPosition() //Called in SetSelectedCardTransform
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _offsetZCardCamera + OffsetZCardSelected; //Apply Z offset on mouse position
        return CardCamera.ScreenToWorldPoint(mousePoint);
    }

    private void CheckSlotHovered() //Check if there is a slot under the card selected
    {
        if (CardSelected != null)
        {
            Ray ray = BoardCamera.ScreenPointToRay(Input.mousePosition);

            int hits = Physics.RaycastNonAlloc(ray, _slotHits);

            for (int i = 0; i < hits; i++)
            {
                if (_slotHits[i].collider.TryGetComponent(out SlotLocation slot) == false)
                {
                    Debug.Log("Not a slot");
                    AddCardFromMouseToHand();
                    return;
                }

                Debug.Log("Slot Detected !");
                
                DeckManager.Instance.PlayCardOnLocation(CardSelected, slot);

                CardSelected = null;

                return;
            }
        }
    }

    //Called when left mouse click up and there is no slot location under the card selected
    private void AddCardFromMouseToHand() 
    {
        CardHand.cards.Insert(_cardSelectedIndex, CardSelected.transform);
        CardSelected.cardStatus = CardStatus.InHand;
        CardSelected = null;
    }
}