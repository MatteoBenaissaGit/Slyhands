using System;
using Data.Cards;
using UnityEngine;

/// <summary>
/// The different action the card can execute
/// </summary>
public enum CardAction
{
    None = 0,
    Drawed = 1,
    Played = 2,
    Discarded = 3,
}

/// <summary>
/// Define what a card is and be, will connect all data and behaviours
/// </summary>
[RequireComponent(typeof(CardView))]
public class Card : MonoBehaviour
{
    #region Field and Properties

    [field: SerializeField] public CardDataScriptable CardDataScriptable { get; private set; }
    
    /// <summary>
    /// This action get invoked on any card actions
    /// </summary>
    public Action<CardAction, bool> OnCardAction { get; set; }

    #endregion
}