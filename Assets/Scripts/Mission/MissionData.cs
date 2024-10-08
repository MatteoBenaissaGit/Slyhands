using UnityEngine;

namespace Mission
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Mission/MissionData", order = 1)]
    public class MissionData : ScriptableObject
    {
        [field: SerializeField] public DeckData DeckData { get; set; }
        [field: SerializeField] public string LevelSaveName { get; set; }
    
        [field: SerializeField] public string MissionName { get; set; }
        [field: SerializeField] public bool MissionComplished { get; set; }
        [field: SerializeField] public int MissionRewardsCount { get; set; }
        [field: SerializeField] public int StarObtained { get; set; }
        [field: SerializeField] public int GoldCardToFound { get; set; }
        [field: SerializeField] public string SecondaryMission { get; set; }
    }
}