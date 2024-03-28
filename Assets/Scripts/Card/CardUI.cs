using UnityEngine;

/// <summary>
/// Will update the UI of each card, depending on it's data
/// </summary>
public class CardUI : MonoBehaviour
{
    #region Field and Properties

    private Card _card;

    [Header("Prefab Elements")] [SerializeField]
    private Sprite _cardSprite;

    #endregion
}