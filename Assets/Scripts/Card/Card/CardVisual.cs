using System.Collections;
using System.Collections.Generic;
using Data.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro _cardName;
    [SerializeField] private TextMeshPro _cardDescription;
    [SerializeField] private Material _typeMaterial;
    [SerializeField] private SpriteRenderer _cardSpriteRenderer;
    
    
    public void Initialize(CardData data)
    {
        _cardName.text = data.CardName;
        _cardDescription.text = data.CardDescription;
        _cardSpriteRenderer.sprite = data.CardSprite;

        switch (data.Type)
        {
            case CardEffectType.Movement:
                _typeMaterial = null;
                break;
            
            case CardEffectType.Equipment:
                _typeMaterial = null;
                break;
            
            case CardEffectType.Enemy:
                _typeMaterial = null;
                break;
            
            case CardEffectType.Utility:
                _typeMaterial = null;
                break;
        }
    }
}
