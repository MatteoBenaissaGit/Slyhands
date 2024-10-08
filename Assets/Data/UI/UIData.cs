using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.UIElements
{
    [Serializable]
    public struct UIElementData
    {

        [field: SerializeReference] [field: TableColumnWidth(57, Resizable = false)] [field: PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite Icon { get; set; }
        
        [field:SerializeField] public string Id { get; set; }
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/UI", order = 2)]
    public class UIData : ScriptableObject
    {
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<UIElementData> _spritesData = new List<UIElementData>();
        
        public Sprite GetSprite(string id)
        { 
            Sprite sprite = _spritesData.Find(x => x.Id == id).Icon;
            return sprite;
        }
    }
}