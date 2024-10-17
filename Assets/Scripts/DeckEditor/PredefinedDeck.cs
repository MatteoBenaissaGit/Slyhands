using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/PredefDeck", order = 0)]
public class PredefinedDeck : ScriptableObject
{
    public List<byte> IDCardInDeck;
    public string Name = "";
}
