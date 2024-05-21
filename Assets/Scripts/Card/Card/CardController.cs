using UnityEngine;
using Data.Cards;

public class CardController : MonoBehaviour
{
    public CardData Data;
    [field: SerializeField] public CardVisual CardVisual { get; private set; }

    [Header("Hovered Behavior")] public CardStatus cardStatus;

    [Header("Debug Position")] public Vector3 IdlePosition;
    public Vector3 HoveredPosition;
    public Vector3 MinorPosition;
    public Vector3 DiscardPosition;

    [Header("Debug Scale")] public Vector3 IdleScale;
    public Vector3 HoveredScale;
    public Vector3 MinorScale;
    public Vector3 DiscardScale;

    [Header("Debug Rotation")] public Vector3 DiscardRotation;

    [Header("Debug Dragged Properties")]
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;

    private void Start()
    {
        CardVisual = GetComponent<CardVisual>();
        CardVisual.Initialize(Data);

        IdleScale = transform.localScale;
        MinorScale = transform.localScale;
    }

    private void Update()
    {
        switch (cardStatus)
        {
            case CardStatus.InHand:
                transform.position = Vector3.Lerp(transform.position, IdlePosition,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, IdleScale,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                break;
            case CardStatus.InHandHovered:
                transform.position = Vector3.Lerp(transform.position, HoveredPosition,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, HoveredScale,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                break;
            case CardStatus.InHandMinor:
                transform.position = Vector3.Lerp(transform.position, MinorPosition,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, MinorScale,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                break;

            case CardStatus.Discarded:
                //Position
                transform.position = Vector3.Lerp(transform.position, DiscardPosition,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, DiscardScale,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);

                //Rotation
                Quaternion toRotation = Quaternion.Euler(DiscardRotation.x, DiscardRotation.y, DiscardRotation.z);

                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation,
                    Time.deltaTime * CardManager.Instance.HandCardsMovementSpeed);
                break;
            
            case CardStatus.Dragged:
                transform.position = Vector3.Lerp(transform.position, TargetPosition,
                    Time.deltaTime * CardManager.Instance.SmoothMovementSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation,
                    Time.deltaTime * CardManager.Instance.SmoothRotationSpeed);
                break;
        }
    }
}