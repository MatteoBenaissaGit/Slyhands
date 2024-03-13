using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Slots
{
    /// <summary>
    /// This class handle the view and the visual feedbacks of the slot
    /// </summary>
    public class SlotView : MonoBehaviour
    {
        public SlotController Controller { get; private set; }

        #region Private values

        [BoxGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _actionFeedbackSpriteRenderer;
        [BoxGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _selectionFeedbackSpriteRenderer;
        [BoxGroup("Feedback sprites"), SerializeField, Required, ChildGameObjectsOnly] 
        private SpriteRenderer _arrowFeedbackSpriteRenderer;

        #endregion

        /// <summary>
        /// This method initialize the slot view
        /// </summary>
        /// <param name="controller">the controller the view is related to</param>
        public void Initialize(SlotController controller)
        {
            Controller = controller;
            Controller.OnSlotAction += SlotActionView;
        }

        private void OnDestroy()
        {
            Controller.OnSlotAction -= SlotActionView;
            Controller = null;
        }

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
                    if (Application.isPlaying)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    DestroyImmediate(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}