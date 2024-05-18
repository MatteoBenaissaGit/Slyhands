using UnityEngine;

namespace Data.Cards
{
    public enum CardEffectType
    {
        Movement = 0,
        Equipment = 1,
        Enemy = 2,
        Utility= 3
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Card/CardData", order = 1)]
    public class CardData : ScriptableObject
    {
        [field: SerializeField] public string CardName { get; private set; }
        [field: SerializeField] public string CardDescription { get; private set; }
        [field : SerializeField] public CardEffectType Type { get; private set; }
        [field: SerializeField] public Sprite CardSprite { get; private set; }
    }
}