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
        public Vector2Int Coordinates { get; set; }
        
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _emptySprite;

        private SlotView _slotView;

        public void SetSlotOnLocation(SlotView view)
        {
            if (_slotView != null)
            {
                DestroySlot();
            }

            _slotView = view;
            _emptySprite.gameObject.SetActive(false);
        }

        public void DestroySlot()
        {
            if (_slotView != null)
            {
                return;
            }
            Destroy(_slotView);
            _emptySprite.gameObject.SetActive(true);
        }
    }
}