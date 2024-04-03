using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    #region Field and Properties

    private Card _card;

    [Header("Prefab Elements")]
    [SerializeField] private TMP_Text _cardNameText;
    [SerializeField] private TMP_Text _cardDescriptionText;
    [SerializeField] private TMP_Text _playerCostText;

    [Header("Card Visual")] 
    [SerializeField] private SpriteRenderer _subjectSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardOrnamentRenderer;

    #endregion

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetCard();
    }

    private void OnValidate()
    {
        Awake();
    }

    private void SetCard()
    {
        SetCardProperties();
        SetCardVisual();
    }

    private void SetCardProperties()
    {
        _cardNameText.text = _card.CardDataScriptable.CardName;
        _cardDescriptionText.text = _card.CardDataScriptable.CardDescription;
        _playerCostText.text = _card.CardDataScriptable.PlayerCost.ToString();
    }

    private void SetCardVisual()
    {
        _subjectSpriteRenderer.sprite = _card.CardDataScriptable.SubjectSprite;
        _cardOrnamentRenderer.sprite = _card.CardDataScriptable.CardOrnament;
    }
}