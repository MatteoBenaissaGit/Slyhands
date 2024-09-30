using System;
using System.Collections.Generic;
using Data.Prefabs;
using DG.Tweening;
using GameEngine;
using LevelEditor;
using LevelEditor.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
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
        public SlotController Controller { get; private set; }
        public LevelEditorCharacter LevelEditorCharacterOnSlot { get; private set; }

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

        /// <summary>
        /// This method initialize the slot view
        /// </summary>
        /// <param name="controller">the controller the view is related to</param>
        public void Initialize(SlotController controller)
        {
            Controller = controller;

            Color transparentBaseColor = new Color(1f, 1f, 1f, 0f);
            _actionFeedbackSpriteRenderer.color = transparentBaseColor;
            _selectionFeedbackSpriteRenderer.color = transparentBaseColor;
            _arrowFeedbackSpriteRenderer.color = transparentBaseColor;

            CreateObstacle(GetPrefabsData().GetPrefab(Controller.Data.Obstacle.PrefabId));
            CreateCharacterOnSlot(GetPrefabsData().GetPrefab(Controller.Data.LevelEditorCharacter.PrefabId));

            SetSlotOrientation(Controller.Data.Orientation);

            //slot type setup
            if (Controller.Data.SlotTypeReferenceId != null)
            {
                SetSlotTypeReference(Controller.Data.SlotTypeReferenceId);
            }
            
            //initialize scale tween if level editor
            if (LevelEditorManager.Instance != null)
            {
                transform.DOKill();
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }

        private PrefabsData GetPrefabsData()
        {
            return LevelEditorManager.Instance == null
                ? GameManager.Instance.PrefabsData
                : LevelEditorManager.Instance.PrefabsData;
        }

        private void OnDestroy()
        {
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
            if (obstaclePrefab == GetPrefabsData().GetPrefab(Controller.Data.Obstacle.PrefabId) && _obstacleParent.childCount > 0)
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

            if (Controller.Data.LevelEditorCharacter.Has)
            {
                Debug.Log("destroy character");
                CreateCharacterOnSlot(null);
            }

            Controller.Data.Obstacle.PrefabId = GetPrefabsData().GetPrefabId(obstaclePrefab);

            GameObject obstacleInstantiated = Instantiate(obstaclePrefab, _obstacleParent, true);
            obstacleInstantiated.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (LevelEditorManager.Instance != null) //tween if level editor
            {
                obstacleInstantiated.transform.DOComplete();
                Vector3 scale = obstacleInstantiated.transform.localScale;
                obstacleInstantiated.transform.localScale = Vector3.zero;
                obstacleInstantiated.transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
            }

            SetObstacleOrientation(Controller.Data.Obstacle.Orientation);
        }

        public void SetObstacleOrientation(WorldOrientation.Orientation orientation)
        {
            Controller.Data.Obstacle.Orientation = orientation;
            _obstacleParent.rotation = Quaternion.Euler(0, (int) orientation * 90, 0);
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
            Controller.Data.Obstacle.PrefabId = null;
        }

        /// <summary>
        /// This method set a new entity on the slot and destroy the current if there was one
        /// </summary>
        /// <param name="levelEditorCharacterPrefab">the prefab of the entity to put</param>
        public LevelEditorCharacter CreateCharacterOnSlot(GameObject levelEditorCharacterPrefab)
        {
            //if the character is the same as the current one, return
            if (levelEditorCharacterPrefab == GetPrefabsData().GetPrefab(Controller.Data.LevelEditorCharacter.PrefabId) && LevelEditorCharacterOnSlot != null)
            {
                return null;
            }
            
            //if there is a character on the slot, destroy it
            if (LevelEditorCharacterOnSlot != null)
            {
                Debug.Log("Destroy character on slot");
                DestroyCharacterOnSlot();
            }

            //if the prefab is null or the prefab is not a character, return
            if (levelEditorCharacterPrefab == null || levelEditorCharacterPrefab.TryGetComponent(out LevelEditorCharacter levelEditorCharacter) == false)
            {
                Controller.Data.LevelEditorCharacter.PrefabId = null;
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

            LevelEditorCharacterOnSlot = Instantiate(levelEditorCharacter, transform);
            LevelEditorCharacterOnSlot.transform.localPosition = new Vector3(0, 0.5f, 0);
            
            Controller.Data.LevelEditorCharacter.PrefabId = GetPrefabsData().GetPrefabId(levelEditorCharacterPrefab);
            LevelEditorCharacterOnSlot.Initialize(Controller);

            return LevelEditorCharacterOnSlot;
        }
        
        /// <summary>
        /// This method destroy the character on the slot
        /// </summary>
        public void DestroyCharacterOnSlot()
        {
            if (LevelEditorCharacterOnSlot == null || LevelEditorCharacterOnSlot.gameObject == null)
            {
                return;
            }
            Destroy(LevelEditorCharacterOnSlot.gameObject);
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
        public SlotView SetSlotOrientation(WorldOrientation.Orientation orientation)
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
            //location
            // Handles.Label(transform.position, $"{Controller.Data.Coordinates.x},{Controller.Data.Coordinates.y},{Controller.Data.Coordinates.z}");
            
            //square
            Gizmos.color = Color.red;
            float height = 0.5f;
            float lenght = 0.5f;
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,-lenght), transform.position + new Vector3(-lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(-lenght,height,lenght), transform.position + new Vector3(lenght,height,lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,lenght), transform.position + new Vector3(lenght,height,-lenght));
            Gizmos.DrawLine(transform.position + new Vector3(lenght,height,-lenght), transform.position + new Vector3(-lenght,height,-lenght));
            
            //orientation
            if (Controller == null)
            {
                return;
            }
            Vector2 orientationVector = WorldOrientation.GetDirection(Controller.Data.Orientation);
            // Gizmos.DrawLine(transform.position + new Vector3(0,0.5f,0), transform.position + new Vector3(orientationVector.x, 1f, orientationVector.y) * 0.5f);
        }

#endif
    }
}