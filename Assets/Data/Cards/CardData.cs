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
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Card/CardData", order = 1)]
    public class CardData : ScriptableObject
    {
        [field: SerializeField] public string CardName { get; set; }
        [field: SerializeField] public string CardDescription { get; set; }
        [field: SerializeField] public Sprite CardIllustrationSprite { get; set; }
        [field: SerializeField] public int CardPower { get; set; }
        [field : SerializeField] public CardRarityType RarityType { get; set; }
    }
}