using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Slots
{
    /// <summary>
    /// This class handle slot location where there can be a slot or not.
    /// It receives the raycast to make action on the slot or on the slot view related to it.
    /// </summary>
    public class SlotLocation : MonoBehaviour
    {
        public Vector2Int Coordinates { get; set; }
        public bool IsEditable { get; private set; }
        
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _emptyEditableSprite;
        [SerializeField, TabGroup("References"), Required] private Collider _clickRaycastCollider;

        private SlotView _slotView;

        public void SetSlotOnLocation(SlotView view)
        {
            if (_slotView != null)
            {
                DestroySlotViewOnLocation();
            }

            _slotView = view;
            _slotView.transform.parent = transform;
            _emptyEditableSprite.gameObject.SetActive(false);
        }

        public void DestroySlotViewOnLocation()
        {
            if (_slotView == null)
            {
                return;
            }
            Destroy(_slotView.gameObject);
            _emptyEditableSprite.gameObject.SetActive(IsEditable);
        }

        public void SetEditable(bool isEditable)
        {
            IsEditable = isEditable;
            
            _clickRaycastCollider.enabled = IsEditable;
            _emptyEditableSprite.gameObject.SetActive(_slotView == null && IsEditable);
        }
    }
}