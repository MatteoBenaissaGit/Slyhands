using Data.Cards;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro _cardName;
    [SerializeField] private TextMeshPro _cardDescription;
    [SerializeField] private SpriteRenderer _cardTypeSpriteRenderer;
    [SerializeField] private SpriteRenderer _cardSubjectSpriteRenderer;
    [SerializeField] private CardEffectType _cardEffectType;
    
    public void Initialize(CardData data)
    {
        _cardName.text = data.CardName;
        _cardDescription.text = data.CardDescription;
        _cardTypeSpriteRenderer.sprite = data.CardTypeSprite;
        _cardSubjectSpriteRenderer.sprite = data.CardSubjectSprite;
        _cardEffectType = data.EffectType;
    }
}