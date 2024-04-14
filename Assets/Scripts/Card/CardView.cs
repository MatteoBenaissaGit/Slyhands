using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardView : MonoBehaviour
{
    #region Field and Properties

    private Card _card;

    [Header("Prefab Elements")] [SerializeField]
    private TMP_Text _cardNameText;

    [SerializeField] private TMP_Text _cardDescriptionText;
    [SerializeField] private SpriteRenderer _subjectSpriteRenderer;

    #endregion

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetCard();
    }

    private void SetCard()
    {
        SetCardProperties();
    }

    private void SetCardProperties()
    {
        _cardNameText.text = _card.CardDataScriptable.CardName;
        _cardDescriptionText.text = _card.CardDataScriptable.CardDescription;
        _subjectSpriteRenderer.sprite = _card.CardDataScriptable.SubjectSprite;
    }
}