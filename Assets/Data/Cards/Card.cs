using UnityEngine;

namespace Data.Cards
{
    public enum CardRarityType
    {
        Bronze = 0,
        Steel = 1,
        Amethyst = 2,
        Gold = 3
    }
    
    public enum CardCategoryEffect
    {
        Movement = 0,
        DeckControl = 1,
        Equipment = 2,
        Enemy = 3
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Card/Card", order = 1)]
    public class Card : ScriptableObject
    {
        [field: SerializeField] public byte ID { get; set; }
        [field: SerializeField] public string CardName { get; set; }
        [field: SerializeField] public string CardDescription { get; set; }
        [field: SerializeField] public Sprite CardIllustrationSprite { get; set; }
        [field: SerializeField] public int CardPower { get; set; }
        [field : SerializeField] public CardRarityType RarityType { get; set; }
        [field: SerializeField] public CardCategoryEffect CategoryEffect { get; set; }
    }
}
