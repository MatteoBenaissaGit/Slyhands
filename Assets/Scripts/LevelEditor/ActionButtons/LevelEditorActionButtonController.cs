using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.ActionButtons
{
    public enum LevelEditorActionButtonType
    {
        Selection = 0,
        Paint = 1,
        Add = 2
    }
    
    /// <summary>
    /// This class handle the control of a level editor action button
    /// </summary>
    public class LevelEditorActionButtonController : MonoBehaviour
    {
        [field:SerializeField] [field:Required] 
        public Button Button { get; private set; }
        
        [field:SerializeField]
        public LevelEditorActionButtonType Type { get; private set; }
        
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private Texture2D _cursorTexture;
        [SerializeField] private Vector2 _cursorHotSpot;
        [SerializeField, Required] private Image _background;

        private void Awake()
        {
            SetSelected(false,true);
        }

        /// <summary>
        /// Visual show that this button is selected or not
        /// </summary>
        /// <param name="isSelected">is button selected ?</param>
        /// <param name="doInstant">make animation instant ?</param>
        public virtual void SetSelected(bool isSelected, bool doInstant = false)
        {
            SelectedEffect(isSelected, doInstant);
            
            Cursor.SetCursor(_cursorTexture, _cursorHotSpot, CursorMode.Auto);

            if (isSelected)
            {
                transform.DOKill();
                transform.DOPunchScale(Vector3.one * 0.1f, doInstant ? 0f : 0.2f);
            }
        }

        /// <summary>
        /// Make the selection effect on an image
        /// </summary>
        /// <param name="image">the image to apply the effect to</param>
        /// <param name="isSelected">is button selected ?</param>
        /// <param name="doInstant">make animation instant ?</param>
        protected void SelectedEffect(Image image, bool isSelected, bool doInstant)
        {
            image.DOKill();
            image.DOFade(isSelected ? 1 : 0.6f, doInstant ? 0f : 0.1f);
        }

        /// <summary>
        /// Make the selection effect on all the button
        /// </summary>
        /// <param name="isSelected">is button selected ?</param>
        /// <param name="doInstant">make animation instant ?</param>
        protected void SelectedEffect(bool isSelected, bool doInstant = false)
        {
            SelectedEffect(_icon, isSelected, doInstant);
            SelectedEffect(_background, isSelected, doInstant);
        }
    }
}
