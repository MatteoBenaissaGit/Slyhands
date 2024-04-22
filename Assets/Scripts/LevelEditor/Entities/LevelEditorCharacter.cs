using Board;
using DG.Tweening;
using Slots;
using Unity.VisualScripting;
using UnityEngine;

namespace LevelEditor.Entities
{
    /// <summary>
    /// This class handle a level editor character
    /// </summary>
    public class LevelEditorCharacter : MonoBehaviour
    {
        [field:SerializeField] public int Team { get; private set; }
        
        public Vector3Int Coordinates => Slot == null ? Vector3Int.zero : Slot.Coordinates;
        public SlotController Slot { get; private set; }
        public Orientation Orientation { get; private set; }

        public void Initialize(SlotController slot)
        {
            Slot = slot;
        }

        public LevelEditorCharacter SetCharacterOrientation(Orientation orientation)
        {
            Orientation = orientation;
            //TODO not working properly
            //transform.rotation = Quaternion.Euler(0, (int) orientation * 90, 0);
            return this;
        }
    }
}
