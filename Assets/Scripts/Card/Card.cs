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

    // // //Vial Card
    [Tooltip("Variable Test number 1 for Vial Card")]
    public string TestVariableVial1;

    [Tooltip("Variable Test number 2 for Vial Card")]
    public bool TestVariableVial2;

    // // //Parchment Card
    [Tooltip("Variable Test number 1 for Parchment Card")]
    public int TestVariableParchment1;

    [Tooltip("Variable Test number 2 for Parchment Card")]
    public string TestVariableParchment2;

    [Tooltip("Variable Test number 3 for Parchment Card")]
    public float TestVariableParchment3;

    // // //Utility Card
    [Tooltip("Variable Test number 1 for Utility Card")]
    public bool TestVariableUtility1;

    [Tooltip("Variable Test number 2 for Utility Card")]
    public bool TestVariableUtility2;

    // // //Enchantment Card
    [Tooltip("Variable Test number 1 for Enchantment Card")]
    public string TestVariableEnchantment1;

    [Tooltip("Variable Test number 2 for Enchantment Card")]
    public string TestVariableEnchantment2;

    [Tooltip("Variable Test number 3 for Enchantment Card")]
    public float TestVariableEnchantment3;
}