using System;
using System.Collections.Generic;
using DG.Tweening;
using LevelEditor.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Slots
{
    /// <summary>
    /// This class handle a reference and the id of a slot type in the slot view
    /// </summary>
    [Serializable]
    public class SlotTypeReference
    {
        [field:SerializeField] public string ID { get; private set; }
        [field:SerializeField] public GameObject SlotGameObject { get; private set; }
        [field:SerializeField] public SlotType SlotType { get; private set; }

        public void SetSlotGameObject(bool isActive)
        {
            SlotGameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// This class handle the view and the visual feedbacks of the slot
    /// </summary>
    [SelectionBase]
    public class SlotView : MonoBehaviour
    {
        /// <summary>
        /// The slot controller related to the slot view
        /// </summary>
        public SlotController Controller { get; private set; }

        #region Private fields

        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _actionFeedbackSpriteRenderer;
        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _selectionFeedbackSpriteRenderer;
        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _arrowFeedbackSpriteRenderer;

        [TabGroup("References"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _obstacleParent;
        
        [SerializeField]
        private List<SlotTypeReference> _slotTypeReferences = new List<SlotTypeReference>();

        #endregion

        private GameObject _levelEditorCharacterOnSlotGameObject;

        /// <summary>
        /// This method initialize the slot view
        /// </summary>
        /// <param name="controller">the controller the view is related to</param>
        public void Initialize(SlotController controller)
        {
            Controller = controller;
            Controller.OnSlotAction += SlotActionView;

            Color transparentBaseColor = new Color(1f, 1f, 1f, 0f);
            _actionFeedbackSpriteRenderer.color = transparentBaseColor;
            _selectionFeedbackSpriteRenderer.color = transparentBaseColor;
            _arrowFeedbackSpriteRenderer.color = transparentBaseColor;

            CreateObstacle(Controller.Data.HasObstacle ? Controller.Data.ObstaclePrefab : null);
            CreateCharacterOnSlot(Controller.Data.HasCharacter ? Controller.Data.CharacterPrefab : null);

            if (Controller.Data.SlotTypeReferenceId != null)
            {
                SetSlotTypeReference(Controller.Data.SlotTypeReferenceId);
            }
        }

        private void OnDestroy()
        {
            Controller.OnSlotAction -= SlotActionView;
            Controller = null;
        }

        #region Obstacles

        /// <summary>
        /// Set the obstacle object on the slot
        /// </summary>
        /// <param name="obstaclePrefab">The object prefab to put as obstacle</param>
        public void CreateObstacle(GameObject obstaclePrefab)
        {
            if (obstaclePrefab == Controller.Data.ObstaclePrefab && _obstacleParent.childCount > 0)
            {
                return;
            }
            
            _obstacleParent.gameObject.SetActive(obstaclePrefab != null);
            
            if (obstaclePrefab == null || _obstacleParent.childCount > 0)
            {
                DestroyCurrentObstacle();
            }

            if (obstaclePrefab == null)
            {
                return;
            }

            if (Controller.Data.HasCharacter)
            {
                CreateCharacterOnSlot(null);
            }
            
            Controller.Data.ObstaclePrefab = obstaclePrefab;

            GameObject obstacle = Instantiate(obstaclePrefab, _obstacleParent, true);
            obstacle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            obstacle.transform.DOComplete();
            Vector3 scale = obstacle.transform.localScale;
            obstacle.transform.localScale = Vector3.zero;
            obstacle.transform.DOScale(scale, 0.2f);
        }

        /// <summary>
        /// This method destroy all child objects beneath the obstacle parent transform
        /// </summary>
        private void DestroyCurrentObstacle()
        {
            for (int i = 0; i < _obstacleParent.childCount; i++)
            {
                GameObject child = _obstacleParent.GetChild(i).gameObject;
                child.transform.DOComplete();
                child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(child));
            }
            Controller.Data.ObstaclePrefab = null;
        }

        /// <summary>
        /// This method set a new entity on the slot and destroy the current if there was one
        /// </summary>
        /// <param name="levelEditorCharacterPrefab">the prefab of the entity to put</param>
        public void CreateCharacterOnSlot(GameObject levelEditorCharacterPrefab)
        {
            if (levelEditorCharacterPrefab == Controller.Data.CharacterPrefab && _levelEditorCharacterOnSlotGameObject != null)
            {
                return;
            }
            
            if (_levelEditorCharacterOnSlotGameObject != null)
            {
                Destroy(_levelEditorCharacterOnSlotGameObject);
            }

            if (levelEditorCharacterPrefab == null || levelEditorCharacterPrefab.TryGetComponent(out LevelEditorCharacter character) == false)
            {
                Controller.Data.CharacterPrefab = null;
                return;
            }

            if (Controller.Data.HasObstacle)
            {
                DestroyCurrentObstacle();
            }
            
            _levelEditorCharacterOnSlotGameObject = Instantiate(levelEditorCharacterPrefab, transform);
            _levelEditorCharacterOnSlotGameObject.transform.SetLocalPositionAndRotation(new Vector3(0,0.5f,0), Quaternion.identity);
            
            Controller.Data.CharacterPrefab = levelEditorCharacterPrefab;
            character.Initialize(Controller);
        }
        
        #endregion
        
        #region Actions

        private void SlotActionView(SlotAction action, bool isValid)
        {
            switch (action)
            {
                case SlotAction.None:
                    break;
                case SlotAction.Hovered:
                    break;
                case SlotAction.Selected:
                    break;
                case SlotAction.Walkable:
                    break;
                case SlotAction.Attackable:
                    break;
                case SlotAction.GetDestroyed:
                    DestroySlot();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        /// <summary>
        /// Destroy the slot either in editor or in game
        /// </summary>
        private void DestroySlot()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
                return;
            }
            DestroyImmediate(gameObject);
        }

        #endregion

        #region Slot parameters

        /// <summary>
        /// Set the type and type id of the slot
        /// </summary>
        /// <param name="slotId">the id of the slot to place</param>
        public void SetSlotTypeReference(string slotId)
        {
            _slotTypeReferences.ForEach(x => x.SetSlotGameObject(false));
            
            SlotTypeReference slot = _slotTypeReferences.Find(x => x.ID == slotId);
            if (slot == null)
            {
                return;
            }
            slot.SetSlotGameObject(true);
            
            Controller.Data.Type = slot.SlotType;
            Controller.Data.SlotTypeReferenceId = slot.ID;
        }

        #endregion
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            DrawSquareGizmo();
        }

        /// <summary>
        /// Draw a red square above the slot
        /// </summary>
        private void DrawSquareGizmo()
        {
            Gizmos.color = Color.red;
            float height = 0.5f;
            float lenght = 0.5f;
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,-lenght), transform.position + new Vector3(-lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,lenght), transform.position + new Vector3(lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,lenght), transform.position + new Vector3(lenght,height,-lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,-lenght), transform.position + new Vector3(-lenght,height,-lenght));
        }

#endif
    }
}