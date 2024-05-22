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
        _cardSubjectSpriteRenderer.sprite = data.CardSubjectSprite;
        _cardEffectType = data.EffectType;

        switch (data.EffectType)
        {
            case CardEffectType.Movement:
                _cardTypeSpriteRenderer.sprite = CardManager.Instance.DeckManager.CardTypeSprite[0];
                break;
            
            case CardEffectType.Equipment:
                _cardTypeSpriteRenderer.sprite = CardManager.Instance.DeckManager.CardTypeSprite[1];
                break;
            
            case CardEffectType.Enemy:
                _cardTypeSpriteRenderer.sprite = CardManager.Instance.DeckManager.CardTypeSprite[2];
                break;
            
            case CardEffectType.Utility:
                _cardTypeSpriteRenderer.sprite = CardManager.Instance.DeckManager.CardTypeSprite[3];
                break;
        }
    }
}