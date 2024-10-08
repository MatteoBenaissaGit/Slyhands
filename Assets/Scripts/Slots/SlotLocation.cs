using System;
using Board;
using Board.Characters;
using DG.Tweening;
using GameEngine;
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
        public bool IsUsable { get; private set; } = true;
        public SlotView SlotView { get; private set; }
        
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _usableSprite;
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _selectedSprite;
        [SerializeField, TabGroup("References"), Required] private SpriteRenderer _detectionFeedbackSprite;
        [SerializeField, TabGroup("References"), Required] private Collider _clickRaycastCollider;
        [SerializeField, TabGroup("References"), Required] private GameObject _hoveredFeedback;

        private Color _baseUsableSpriteColor;
        private Color _baseSelectedSpriteColor;
        private Color _hoveredColor;
        private Color _notEditableColor;

        private void Awake()
        {
            _baseUsableSpriteColor = _usableSprite.color;
            _hoveredColor = new Color(_baseUsableSpriteColor.r, _baseUsableSpriteColor.g, _baseUsableSpriteColor.b, 1f);
            _notEditableColor = new Color(_baseUsableSpriteColor.r, _baseUsableSpriteColor.g, _baseUsableSpriteColor.b, 0f);
            
            _baseSelectedSpriteColor = _selectedSprite.color;
            _detectionFeedbackSprite.color = new Color(1, 1, 1, 0f);
            
            _hoveredFeedback.SetActive(false);
            _selectedSprite.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set a slot view to this slot location
        /// </summary>
        /// <param name="view">the view to set on this location, set null to clear to location</param>
        public void SetSlotViewOnLocation(SlotView view)
        {
            if (SlotView != null)
            {
                DestroySlotViewOnLocation();
            }

            SlotView = view;
            SlotView.transform.parent = transform;
            _usableSprite.gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy the slot view that is associated to this location
        /// </summary>
        public void DestroySlotViewOnLocation()
        {
            if (SlotView == null)
            {
                return;
            }
            Destroy(SlotView.gameObject);
            _usableSprite.gameObject.SetActive(IsUsable);
        }

        /// <summary>
        /// Set the editable property of the location
        /// </summary>
        /// <param name="isEditable">is the slot location editable ?</param>
        public void SetUsable(bool isEditable)
        {
            IsUsable = isEditable;
            
            _clickRaycastCollider.enabled = IsUsable;
            _usableSprite.gameObject.SetActive(SlotView == null);
            _usableSprite.color = IsUsable ? _baseUsableSpriteColor : _notEditableColor;
        }

        /// <summary>
        /// Set the hovered feedbacks of the slot location
        /// </summary>
        /// <param name="isHovered">show the feedback ?</param>
        public void SetHovered(bool isHovered)
        {
            //empty editable sprite
            _usableSprite.color = isHovered ? _hoveredColor : _baseUsableSpriteColor;
            if (isHovered)
            {
                _usableSprite.gameObject.SetActive(true);
            }
            else if (SlotView != null)
            {
                _usableSprite.gameObject.SetActive(false);
            }
            
            //entity
            if (SlotView != null && SlotView.Controller.HasCharacter(out var character))
            {
                character.OnCharacterAction.Invoke(isHovered ? CharacterAction.IsHovered : CharacterAction.IsLeaved, new object[]{character.GameplayData.Team.Color});
            }
            
            if (Coordinates.y == 0)
            {
                return;
            }

            //-----------------------
            //LEVEL EDITOR ONLY BELOW 
            //-----------------------

            //feedback placement
            if (LevelEditorManager.Instance == null)
            {
                return;
            }
            BoardController board = LevelEditorManager.Instance.Board;
            Vector3 position = board.GetCoordinatesToWorldPosition(new Vector3Int(Coordinates.x, 0, Coordinates.z));
            for (int y = Coordinates.y - 1; y >= 0 ; y--)
            {
                SlotLocation location = board.Data.SlotLocations[Coordinates.x, y, Coordinates.z];
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

        /// <summary>
        /// Set the selected feedback of the location
        /// </summary>
        /// <param name="isSelected">show the feedback ?</param>
        public void SetSelected(bool isSelected)
        {
            _selectedSprite.color = _baseSelectedSpriteColor;
            _selectedSprite.gameObject.SetActive(isSelected);
        }
        
        public void SetAccessible(bool isAccessible)
        {
            _selectedSprite.color = new Color(0.4f, 1f, 0.5f);
            if (isAccessible == false)
            {
                SetSelected(false);
            }
        }

        /// <summary>
        /// Check if an entity type can be placed on the slot location
        /// </summary>
        /// <param name="superType">The super type to check for</param>
        public bool CanEntityBePlacedHere(BoardEntitySuperType superType)
        {
            if (SlotView == null)
            {
                return superType == BoardEntitySuperType.Slot;
            }
            
            bool canBePlaced = true;
            
            switch (superType)
            {
                case BoardEntitySuperType.Character:
                    canBePlaced = SlotView.Controller.Data.Type == SlotType.Base;
                    break;
                case BoardEntitySuperType.Building:
                    break;
                case BoardEntitySuperType.Obstacle:
                    canBePlaced = SlotView.Controller.Data.Type != SlotType.Ramp;
                    break;
            }
            
            return canBePlaced;
        }
        
        /// <summary>
        /// Show the detection feedback on the slot location
        /// </summary>
        /// <param name="doShow">show or hide the sprite</param>
        /// <param name="color">the color of the sprite</param>
        public void ShowDetection(bool doShow, Color color)
        {
            _detectionFeedbackSprite.DOKill();
            color.a = _detectionFeedbackSprite.color.a;
            _detectionFeedbackSprite.color = color;
            _detectionFeedbackSprite.DOFade(doShow ? 0.5f : 0f, 0.2f);
        }
    }
}