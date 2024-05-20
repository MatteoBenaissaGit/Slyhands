using UnityEngine;
using Inputs;
using Slots;
using UnityEngine.Serialization;

public enum CardPlacement
{
    Idle = 0,
    Overed = 1,
    Minor = 2,
    Selected = 3,
    Discard = 4
}

public class CardManager : MonoBehaviour
{
    public GameObject DiscardPile;

    public static CardManager Instance;

    public UnityEngine.Camera BoardCamera;
    private RaycastHit[] _slotHits = new RaycastHit[16];

    public UnityEngine.Camera CardCamera;
    private RaycastHit[] _cardHits = new RaycastHit[10];

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
    private float _offsetZCardCamera;

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

        cardControllerInput.OnLeftClickDown += GetSelectedCard;
        cardControllerInput.OnLeftClickUp += CheckSlotHovered;
    }

    private void Update()
    {
        if (CardSelected != null)
        {
            MoveSelectedCard();
        }

        Ray ray = CardCamera.ScreenPointToRay(Input.mousePosition);

        int hits = Physics.RaycastNonAlloc(ray, _cardHits);

        for (int i = 0; i < hits; i++)
        {
            if (CardSelected == null && _cardHits[i].collider.TryGetComponent(out CardController card) &&
                card != CardHovered)
            {
                if (CardHovered != null)
                {
                    CardHovered.IsHovered = false;
                }
            
                CardHovered = card;
                card.IsHovered = true;
                break;
            }
        }
    }

    private void GetSelectedCard()
    {
        if (CardHovered != null)
        {
            CardSelected = CardHovered;

            CardSelected.CardPlacement = CardPlacement.Selected;

            _offsetZCardCamera = CardSelected.transform.position.z - CardCamera.transform.position.z;

            _cardSelectedIndex = CardHand.cards.IndexOf(CardHovered.transform);

            CardHand.cards.Remove(CardHovered.transform);
        }
    }

    private void MoveSelectedCard()
    {
        Vector3 newPosition = GetMouseWorldPosition();
        Quaternion toRotation = Quaternion.Euler(-90 + (newPosition.y - CardSelected.transform.position.y) * 90,
            -(newPosition.x - CardSelected.transform.position.x) * 90, 0);


        CardSelected.transform.position = Vector3.Lerp(CardSelected.transform.position, newPosition,
            Time.deltaTime * SmoothMovementSpeed);
        CardSelected.transform.rotation = Quaternion.Lerp(CardSelected.transform.rotation, toRotation,
            Time.deltaTime * SmoothRotationSpeed);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _offsetZCardCamera + OffsetZCardSelected;
        return CardCamera.ScreenToWorldPoint(mousePoint);
    }

    private void CheckSlotHovered()
    {
        Ray ray = BoardCamera.ScreenPointToRay(Input.mousePosition);

        int hits = Physics.RaycastNonAlloc(ray, _slotHits);

        for (int i = 0; i < hits; i++)
        {
            if (!_slotHits[i].collider.TryGetComponent(out SlotLocation slot))
            {
                Debug.Log("Not a slot");
                continue;
            }

            Debug.Log("Slot Detected !");

            CardHovered = null;

            DeckManager.Instance.PlayCardOnLocation(CardSelected, slot);

            CardSelected = null;

            break;
        }

        if (CardSelected != null)
        {
            Debug.Log("Slot NOT Detected !");
            AddCardFromMouseToHand();
        }
    }

    private void AddCardFromMouseToHand()
    {
        if (CardSelected != null)
        {
            CardHand.cards.Insert(_cardSelectedIndex, CardSelected.transform);
            CardSelected.CardPlacement = CardPlacement.Idle;
            CardSelected = null;
        }
    }
}