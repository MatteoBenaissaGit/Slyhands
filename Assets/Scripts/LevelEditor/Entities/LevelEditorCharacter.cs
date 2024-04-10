using Board;
using Slots;
using UnityEngine;

namespace LevelEditor.Entities
{
    public class LevelEditorCharacter : MonoBehaviour
    {
        [field:SerializeField] public int Team { get; private set; }
        
        public Vector3Int Coordinates
        {
            get
            {
                return Slot == null ? Vector3Int.zero : Slot.Coordinates;
            }
        }

        public SlotController Slot { get; private set; }

        public void Initialize(SlotController slot)
        {
            Slot = slot;
        }
    }
}
