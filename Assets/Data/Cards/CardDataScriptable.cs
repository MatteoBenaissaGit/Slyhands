using UnityEngine;

namespace Data.Cards
{
    [CreateAssetMenu(fileName = "Data", menuName = "Card/CardData", order = 1)]
    public class CardDataScriptable : ScriptableObject
    {
        #region Fields and Properties
    
        [field: SerializeField] public string CardName { get; private set; }
        [field: SerializeField] public string CardDescription { get; private set; }
        [field: SerializeField] public int PlayerCost { get; private set; }
        [field: SerializeField] public Sprite SubjectSprite { get; private set; }
        [field: SerializeField] public Sprite CardOrnament { get; private set; }
        [field : SerializeField] public CardEffectType EffectType { get; private set; }
    
        #endregion
    }

    public enum CardEffectType
    {
        Vial = 0,
        Parchment = 1,
        Utility = 2,
        Spell = 3
    }
}