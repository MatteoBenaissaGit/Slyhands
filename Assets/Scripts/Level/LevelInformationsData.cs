using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Level/LevelInformationsData", order = 1)]
public class LevelInformationsData : ScriptableObject
{
    [field: SerializeField] public string MissionName { get; set; }
    [field: SerializeField] public bool MissionAccomplished { get; set; }
    [field: SerializeField] public int CompletingRewardsCount { get; set; }
    [field: SerializeField] public int GoldCardsInLevelCount { get; set; }
    
    //A remplacer par un scriptable "MissionSecondaryObjectivesData"
    [field: SerializeField] public string MissionSecondaryObjectivesText { get; set; }

    [field: SerializeField] public int StarObtained = 0;
}
