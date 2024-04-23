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

        private LevelEditorCharacter _levelEditorCharacterOnSlot;

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

            CreateObstacle(Controller.Data.Obstacle.Has ? Controller.Data.Obstacle.Prefab : null);
            CreateCharacterOnSlot(Controller.Data.Character.Has ? Controller.Data.Character.Prefab : null);

            SetSlotOrientation(Controller.Data.Orientation);

            //slot type setup
            if (Controller.Data.SlotTypeReferenceId != null)
            {
                SetSlotTypeReference(Controller.Data.SlotTypeReferenceId);
            }
            
            //initialize scale tween
            transform.DOKill();
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
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
            //if the obstacle is the same as the current one, return
            if (obstaclePrefab == Controller.Data.Obstacle.Prefab && _obstacleParent.childCount > 0)
            {
                return;
            }
            
            _obstacleParent.gameObject.SetActive(obstaclePrefab != null);
            
            //if the prefab is null and there is an obstacle, destroy the current obstacle
            if (obstaclePrefab == null || _obstacleParent.childCount > 0)
            {
                DestroyObstacleOnSlot();
            }

            //if the slot is not a base slot or the prefab is null, return
            if (obstaclePrefab == null || Controller.Data.Type != SlotType.Base)
            {
                return;
            }

            if (Controller.Data.Character.Has)
            {
                Debug.Log("destroy chara");
                CreateCharacterOnSlot(null);
            }

            Controller.Data.Obstacle.Prefab = obstaclePrefab;

            GameObject obstacle = Instantiate(obstaclePrefab, _obstacleParent, true);
            obstacle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            obstacle.transform.DOComplete();
            Vector3 scale = obstacle.transform.localScale;
            obstacle.transform.localScale = Vector3.zero;
            obstacle.transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// This method destroy all child objects beneath the obstacle parent transform
        /// </summary>
        public void DestroyObstacleOnSlot()
        {
            for (int i = 0; i < _obstacleParent.childCount; i++)
            {
                GameObject child = _obstacleParent.GetChild(i).gameObject;
                child.transform.DOComplete();
                child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(child));
            }
            Controller.Data.Obstacle.Prefab = null;
        }

        /// <summary>
        /// This method set a new entity on the slot and destroy the current if there was one
        /// </summary>
        /// <param name="levelEditorCharacterPrefab">the prefab of the entity to put</param>
        public LevelEditorCharacter CreateCharacterOnSlot(GameObject levelEditorCharacterPrefab)
        {
            //if the character is the same as the current one, return
            if (levelEditorCharacterPrefab == Controller.Data.Character.Prefab && _levelEditorCharacterOnSlot != null)
            {
                return null;
            }
            
            //if there is a character on the slot, destroy it
            if (_levelEditorCharacterOnSlot != null)
            {
                Debug.Log("Destroy character on slot");
                DestroyCharacterOnSlot();
            }

            //if the prefab is null or the prefab is not a character, return
            if (levelEditorCharacterPrefab == null || levelEditorCharacterPrefab.TryGetComponent(out LevelEditorCharacter levelEditorCharacter) == false)
            {
                Controller.Data.Character.Prefab = null;
                return null;
            }

            //if the slot has an obstacle, destroy it
            if (Controller.Data.Obstacle.Has)
            {
                DestroyObstacleOnSlot();
            }
            
            //if the slot is not a base slot, return
            if (Controller.Data.Type != SlotType.Base)
            {
                return null;
            }

            _levelEditorCharacterOnSlot = Instantiate(levelEditorCharacter, transform);
            _levelEditorCharacterOnSlot.transform.localPosition = new Vector3(0, 0.5f, 0);
            
            Controller.Data.Character.Prefab = levelEditorCharacterPrefab;
            _levelEditorCharacterOnSlot.Initialize(Controller);

            return _levelEditorCharacterOnSlot;
        }
        
        /// <summary>
        /// This method destroy the character on the slot
        /// </summary>
        public void DestroyCharacterOnSlot()
        {
            Destroy(_levelEditorCharacterOnSlot.gameObject);
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
        public SlotView SetSlotTypeReference(string slotId)
        {
            _slotTypeReferences.ForEach(x => x.SetSlotGameObject(false));
            
            SlotTypeReference slot = _slotTypeReferences.Find(x => x.ID == slotId);
            if (slot == null)
            {
                return this;
            }
            slot.SetSlotGameObject(true);
            
            Controller.Data.Type = slot.SlotType;
            Controller.Data.SlotTypeReferenceId = slot.ID;

            return this;
        }

        /// <summary>
        /// Set the slot orientation to the data and rotate the view to match it
        /// </summary>
        /// <param name="orientation">The orientation to set for the slot view</param>
        public SlotView SetSlotOrientation(Orientation orientation)
        {
            transform.rotation = Quaternion.Euler(0, (int) orientation * 90, 0);
            Controller.Data.Orientation = orientation;
            return this;
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
            //square
            Gizmos.color = Color.red;
            float height = 0.5f;
            float lenght = 0.5f;
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,-lenght), transform.position + new Vector3(-lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,lenght), transform.position + new Vector3(lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,lenght), transform.position + new Vector3(lenght,height,-lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,-lenght), transform.position + new Vector3(-lenght,height,-lenght));
            
            //orientation
            Vector2 orientationVector = Controller.Data.Orientation switch
            {
                Orientation.North => new Vector2(0,1),
                Orientation.East => new Vector2(1, 0),
                Orientation.South => new Vector2(0,-1),
                Orientation.West => new Vector2(-1, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
            Gizmos.DrawLine(transform.position + new Vector3(0,0.5f,0), 
                transform.position + new Vector3(orientationVector.x, 1f, orientationVector.y) * 0.5f);
        }

#endif
    }
}