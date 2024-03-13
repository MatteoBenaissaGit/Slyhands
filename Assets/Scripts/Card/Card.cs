using UnityEngine;

public enum CardType
{
    Vial = 0,
    Parchment = 1,
    Utility = 2,
    Enchantment = 3
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject
{
    public CardType CardType;

    //Commons Variables at each Card Type
    public string CardTitle;
    public Sprite CardSprite;
    public string CardDescription;
    public int CardPrice;

    //Specific Variables
    public int MovingDist;

    // //Test
    public string VialString;
    public int ParchmentInt;
    public bool UtilityBool;
    public float EnchantmentFloat;
}
