using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.Team
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Teams", order = 1)]
    public class TeamsData : ScriptableObject
    {
        [field:TableList]
        [field: SerializeField] public List<Players.Team> Teams { get; private set; } = new List<Players.Team>();
    }
}