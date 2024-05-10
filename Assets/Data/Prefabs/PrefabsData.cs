using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Prefabs
{
    [Serializable]
    public struct PrefabData
    {
        [field:SerializeField] public string Id { get; set; }
        [field:SerializeField] public GameObject Prefab { get; set; }
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Prefabs", order = 1)]
    public class PrefabsData : ScriptableObject
    {
        [SerializeField] private List<PrefabData> _prefabsDatas = new List<PrefabData>();
        
        public GameObject GetPrefab(string id)
        { 
            GameObject prefab = _prefabsDatas.Find(x => x.Id == id).Prefab;
            return prefab;
        }

        public string GetPrefabId(GameObject prefab)
        {
            string id = _prefabsDatas.Find(x => x.Prefab == prefab).Id;
            return id;
        }
    }
}