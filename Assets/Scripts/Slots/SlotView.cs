using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Slots
{
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

        #endregion

        private GameObject _entityOnSlot;

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

            SetObstacle(Controller.Data.HasObstacle ? Controller.Data.ObstaclePrefab : null);
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
        public void SetObstacle(GameObject obstaclePrefab)
        {
            if (obstaclePrefab == null || _obstacleParent.childCount > 0)
            {
                DestroyCurrentObstacle();
            }
            
            _obstacleParent.gameObject.SetActive(obstaclePrefab != null);
            Controller.Data.ObstaclePrefab = obstaclePrefab;

            if (obstaclePrefab == null)
            {
                return;
            }
            
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
        }

        /// <summary>
        /// This method set a new entity on the slot and destroy the current if there was one
        /// </summary>
        /// <param name="entityPrefab">the prefab of the entity to put</param>
        public void SetEntity(GameObject entityPrefab)
        {
            if (_entityOnSlot != null)
            {
                Destroy(_entityOnSlot);
            }

            if (entityPrefab == null)
            {
                return;
            }
            
            _entityOnSlot = Instantiate(entityPrefab, transform);
            _entityOnSlot.transform.SetLocalPositionAndRotation(new Vector3(0,0.5f,0), Quaternion.identity);

            Controller.Data.EntityPrefab = entityPrefab;
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