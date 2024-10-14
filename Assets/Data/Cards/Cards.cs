using System.Collections.Generic;
using UnityEngine;

namespace Data.Cards
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Cards/Cards", order = 1)]
    public class Cards : ScriptableObject
    {
        [field: SerializeField] private List<Card> _cards { get; set; }

        public List<Card> GetAllCards => _cards;
        
        public Card GetCardData(byte ID)
        {
            return _cards.Find(x => x.ID == ID);
        }
    }
}