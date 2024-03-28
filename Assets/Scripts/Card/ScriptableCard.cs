using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Card/ScriptableCard", order = 1)]
public class ScriptableCard : ScriptableObject
{
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField] public string CardDescription { get; private set; }
    [field: SerializeField] public int PlayerCost { get; private set; }
    [field: SerializeField] public Sprite CardSprite { get; private set; }
    [field: SerializeField] public Sprite CardOrnament { get; private set; }
    
    [field : SerializeField] public CardEffectType EffectType { get; private set; }
}

public enum CardEffectType
{
    Vial = 0,
    Parchment = 1,
    Utility = 2,
    Spell = 3
}