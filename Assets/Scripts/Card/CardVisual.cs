using System;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    #region Field and Properties

    private Card _card;

    [Header("Prefab Elements")]
    [SerializeField] private string _cardName;
    [SerializeField] private string _cardDescription;
    [SerializeField] private int _playerCost;
    [SerializeField] private Sprite _cardSprite;
    [SerializeField] private Sprite _cardOrnament;
    [SerializeField] private CardEffectType _effectType;

    [Header("Card Visual")] 
    [SerializeField] private SpriteRenderer _cardSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardOrnamentRenderer;

    #endregion

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetCardVisual();
    }

    private void OnValidate()
    {
        Awake();
    }

    private void SetCardVisual()
    {
        _cardName = _card.CardData.CardName;
        _cardDescription = _card.CardData.CardDescription;
        _playerCost = _card.CardData.PlayerCost;
        _cardSprite = _card.CardData.CardSprite;
        _cardOrnament = _card.CardData.CardOrnament;
        _effectType = _card.CardData.EffectType;

        _cardSpriteRenderer.sprite = _cardSprite;
        _cardOrnamentRenderer.sprite = _cardOrnament;
    }
}