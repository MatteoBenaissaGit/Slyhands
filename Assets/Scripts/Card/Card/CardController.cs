using UnityEngine;
using Data.Cards;

public class CardController : MonoBehaviour
{
    public CardData Data;
    [field: SerializeField] public CardVisual CardVisual { get; private set; }

    [Header("Hovered Behavior")] public CardPlacement CardPlacement;
    public bool IsHovered;

    [Header("Debug Position")] public Vector3 IdlePosition;
    public Vector3 HoveredPosition;
    public Vector3 MinorPosition;
    public Vector3 DiscardPosition;

    [Header("Debug Scale")] public Vector3 IdleScale;
    public Vector3 HoveredScale;
    public Vector3 MinorScale;
    public Vector3 DiscardScale;

    [Header("Debug Rotation")] public Vector3 DiscardRotation;

    private void Start()
    {
        CardVisual = GetComponent<CardVisual>();
        CardVisual.Initialize(Data);

        IdleScale = transform.localScale;
        MinorScale = transform.localScale;
    }

    private void Update()
    {
        if (CardManager.Instance.CardHand.cards.Contains(transform))
        {
            switch (CardPlacement)
            {
                case CardPlacement.Idle:
                    transform.position = Vector3.Lerp(transform.position, IdlePosition,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    transform.localScale = Vector3.Lerp(transform.localScale, IdleScale,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    break;
                case CardPlacement.Overed:
                    transform.position = Vector3.Lerp(transform.position, HoveredPosition,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    transform.localScale = Vector3.Lerp(transform.localScale, HoveredScale,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    break;
                case CardPlacement.Minor:
                    transform.position = Vector3.Lerp(transform.position, MinorPosition,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    transform.localScale = Vector3.Lerp(transform.localScale, MinorScale,
                        Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                    break;
            }
        }

        if (CardPlacement == CardPlacement.Discard)
        {
            IsHovered = false;

            transform.position = Vector3.Lerp(transform.position, DiscardPosition,
                Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, DiscardScale,
                Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
            
            
            Quaternion toRotation = Quaternion.Euler(DiscardRotation.x, DiscardRotation.y, DiscardRotation.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation,
                Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
        }
    }
}