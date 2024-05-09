using DG.Tweening;
using Slots;
using UnityEngine;

namespace LevelEditor.Entities
{
    /// <summary>
    /// This class handle a level editor character
    /// </summary>
    public class LevelEditorCharacter : MonoBehaviour
    {
        [field: SerializeField] public int Team { get; private set; }
        public Vector3Int Coordinates => Slot == null ? Vector3Int.zero : Slot.Coordinates;
        public SlotController Slot { get; private set; }

        /// <summary>
        /// Initialize the character with the slot he's on
        /// </summary>
        /// <param name="slot">the slot on which the character is</param>
        public void Initialize(SlotController slot)
        {
            Slot = slot;
            SetCharacterOrientation(Slot.Data.LevelEditorCharacter.Orientation);

            transform.DOKill();
            Vector3 scale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
        }
        
        /// <summary>
        /// Set the character's orientation and change its rotation to match it
        /// </summary>
        /// <param name="orientation">The orientation to put the character in</param>
        /// <returns>The character</returns>
        public LevelEditorCharacter SetCharacterOrientation(Orientation orientation)
        {
            Slot.Data.LevelEditorCharacter.Orientation = orientation;
            transform.rotation = Quaternion.Euler(0, ((int)orientation) * 90, 0);
            return this;
        }
    }
}
