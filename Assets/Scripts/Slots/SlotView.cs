﻿using System;
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

        #region Private values

        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _actionFeedbackSpriteRenderer;
        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _selectionFeedbackSpriteRenderer;
        [TabGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _arrowFeedbackSpriteRenderer;

        [TabGroup("References"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _obstacleParent;

        #endregion

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
            
            _obstacleParent.gameObject.SetActive(Controller.Data.HasObstacle);
        }

        private void OnDestroy()
        {
            Controller.OnSlotAction -= SlotActionView;
            Controller = null;
        }

        /// <summary>
        /// Set the obstacle object on the slot
        /// </summary>
        /// <param name="obstaclePrefab">The object prefab to put as obstacle</param>
        public void SetObstacle(GameObject obstaclePrefab)
        {
            if (obstaclePrefab == null)
            {
                return;
            }
            
            _obstacleParent.gameObject.SetActive(true);
            Controller.Data.HasObstacle = true;
            Controller.Data.ObstaclePrefab = obstaclePrefab;

            GameObject obstacle = Instantiate(obstaclePrefab, _obstacleParent, true);
            obstacle.transform.SetLocalPositionAndRotation(Vector3.up, Quaternion.identity);
        }

        /// <summary>
        /// Set if there is an obstacle or not
        /// </summary>
        /// <param name="isThereObstacle">Is there an obstacle ?</param>
        public void SetObstacle(bool isThereObstacle)
        {
            _obstacleParent.gameObject.SetActive(isThereObstacle);
            Controller.Data.HasObstacle = isThereObstacle;
        }
        
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