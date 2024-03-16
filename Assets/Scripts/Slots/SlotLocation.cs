using System;
using LevelEditor;
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
        public Vector3Int Coordinates { get; set; }
        public bool IsEditable { get; private set; } = true;
        public SlotView SlotView { get; private set; }
        
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _emptyEditableSprite;
        [SerializeField, TabGroup("References"), Required] private Collider _clickRaycastCollider;
        [SerializeField, TabGroup("References"), Required] private GameObject _hoveredFeedback;

        private Color _baseColor;
        private Color _hoveredColor;
        private Color _notEditableColor;

        private void Awake()
        {
            _baseColor = _emptyEditableSprite.color;
            _hoveredColor = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 1f);
            _notEditableColor = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0f);
            
            _hoveredFeedback.SetActive(false);
        }

        public void SetSlotOnLocation(SlotView view)
        {
            if (SlotView != null)
            {
                DestroySlotViewOnLocation();
            }

            SlotView = view;
            SlotView.transform.parent = transform;
            _emptyEditableSprite.gameObject.SetActive(false);
        }

        public void DestroySlotViewOnLocation()
        {
            if (SlotView == null)
            {
                return;
            }
            Destroy(SlotView.gameObject);
            _emptyEditableSprite.gameObject.SetActive(IsEditable);
        }

        public void SetEditable(bool isEditable)
        {
            IsEditable = isEditable;
            
            _clickRaycastCollider.enabled = IsEditable;
            _emptyEditableSprite.gameObject.SetActive(SlotView == null);
            _emptyEditableSprite.color = IsEditable ? _baseColor : _notEditableColor;
        }

        public void SetHovered(bool isHovered)
        {
            _emptyEditableSprite.color = isHovered ? _hoveredColor : _baseColor;
            
            if (Coordinates.y == 0)
            {
                return;
            }

            Vector3 position = LevelEditorManager.Instance.Board.GetCoordinatesToWorldPosition(new Vector3Int(Coordinates.x, 0, Coordinates.z));

            for (int y = Coordinates.y - 1; y >= 0 ; y--)
            {
                SlotLocation location = LevelEditorManager.Instance.Board.Data.SlotLocations[Coordinates.x, y, Coordinates.z];
                if (location.SlotView == null)
                {
                    continue;
                }
                position = location.transform.position;
                break;
            }
            
            _hoveredFeedback.SetActive(isHovered);
            _hoveredFeedback.transform.position = position;
        }
    }
}