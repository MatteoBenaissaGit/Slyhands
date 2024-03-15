using Sirenix.OdinInspector;
using UnityEngine;

namespace Slots
{
    /// <summary>
    /// This class handle slot location where there can be a slot or not.
    /// It receives the raycast to make action on the slot or on the slot view related to it.
    /// </summary>
    public class SlotLocation : MonoBehaviour
    {
        [SerializeField, TabGroup("References"), Required] private SlotView _slotViewPrefab;
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _emptySprite;

        private SlotView _slotView;

        public void SetSlot(SlotView slot = null)
        {
            _slotView = slot;
            _emptySprite.gameObject.SetActive(slot == null);
        }
    }
}