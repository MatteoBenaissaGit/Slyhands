    using Data.Cards;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro _cardPowerText;
    [SerializeField] private SpriteRenderer _cardIllustration;
    
    public void Initialize(Data.Cards.Card data)
    {
        _cardPowerText.text = data.CardPower.ToString();
        _cardIllustration.sprite = data.CardIllustrationSprite;
    }
}