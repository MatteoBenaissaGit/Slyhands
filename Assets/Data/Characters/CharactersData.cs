using System;
using System.Collections.Generic;
using Board.Characters;
using UnityEngine;

namespace Data.Characters
{
    [Serializable]
    public class CharacterData
    {
        [field:SerializeField] public CharacterType Type { get; private set; }    
        [field:SerializeField] public int Life { get; private set; }    
        [field:SerializeField] public int MovementPoints { get; private set; }    
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Characters", order = 1)]
    public class CharactersData : ScriptableObject
    {
        [SerializeField] private List<CharacterData> Characters = new List<CharacterData>();
        
        public CharacterData GetCharacterData(CharacterType type)
        {
            CharacterData character = Characters.Find(x => x.Type == type);
            if (character == null)
            {
                throw new Exception($"CharacterData not found for type {type}");
            }
            return character;
        }
    }
}