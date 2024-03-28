using UnityEngine;

/// <summary>
/// Define what a card is and be, will connect all data and behaviours
/// </summary>
[RequireComponent(typeof(CardVisual))]
public class Card : MonoBehaviour
{
    #region Field and Properties

    [field: SerializeField] public ScriptableCard CardData { get; private set; }

    #endregion
}