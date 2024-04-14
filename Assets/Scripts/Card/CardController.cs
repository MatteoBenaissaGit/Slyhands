using System;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public int RepositoryPosition;
    public bool IsSelected;

    private Card _card;

    [Header("Drag the card")] [SerializeField]
    private int _positionSmoothness;

    [SerializeField] private int _rotationSmoothness;

    [SerializeField] private float _distance = 1.0f;
    [SerializeField] private bool _useInitalCameraDistance = false;

    private float actualDistance;

    private CardManager _cardManager;

    void Start()
    {
        _cardManager = CardManager.Instance;
        _card = GetComponentInChildren<Card>();

        if (_useInitalCameraDistance)
        {
            Vector3 toObjectVector = transform.position - UnityEngine.Camera.main.transform.position;
            Vector3 linearDistanceVector = Vector3.Project(toObjectVector, UnityEngine.Camera.main.transform.forward);
            actualDistance = linearDistanceVector.magnitude;
        }
        else
        {
            actualDistance = _distance;
        }
    }

    private void Update()
    {
        if (!IsSelected) MoveToPosistion();
        if (Input.GetMouseButtonUp(0))
        {
            IsSelected = false;
        }
    }

    private void MoveToPosistion()
    {
        Vector3 toPosition = new Vector3(0, 0, 0);
        Quaternion toRotation = Quaternion.Euler(0, 0, 0);
        Vector3 toScale = new Vector3(1, 1, 1);

        if (_card.cardStatus == CardAction.None)
        {
            toPosition = new Vector3(_cardManager.Deck.transform.position.x, _cardManager.Deck.transform.position.y,
                _cardManager.Deck.transform.position.z + 0.2f);
            toRotation = Quaternion.Euler(0, 180, 0);
            toScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
        else if (_card.cardStatus == CardAction.Drawed)
        {
            toPosition = _cardManager.Hand.transform.position;
            toScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else if (_card.cardStatus == CardAction.Played)
        {
            toPosition = new Vector3(0f, 0f, 0);
            toScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

        if (transform.position != toPosition)
        {
            transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime * 3.0f);
        }

        if (transform.rotation != toRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10.0f);
        }

        if (transform.localScale != toScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, toScale, Time.deltaTime * 3.0f);
        }
    }

    private void OnMouseDrag()
    {
        if (_card.cardStatus == CardAction.Drawed)
        {
            IsSelected = true;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = actualDistance;
            mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            Quaternion toRotation = Quaternion.Euler((mousePosition.y - transform.position.y) * 90,
                -(mousePosition.x - transform.position.x) * 90, 0);

            transform.position = Vector3.Lerp(transform.position, mousePosition, Time.deltaTime * _positionSmoothness);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * _rotationSmoothness);
        }
    }
}