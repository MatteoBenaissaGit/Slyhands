using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelEditorActionButtonController : MonoBehaviour
    {
        [field:SerializeField] [field:Required] 
        public Button Button { get; private set; }
        
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private Image _background;

        private void Awake()
        {
            SetSelected(false,true);
        }

        public void SetSelected(bool isSelected, bool doInstant = false)
        {
            SelectedEffect(_icon, isSelected, doInstant);
            SelectedEffect(_background, isSelected, doInstant);

            if (isSelected)
            {
                transform.DOKill();
                transform.DOPunchScale(Vector3.one * 0.1f, doInstant ? 0f : 0.2f);
            }
        }

        private void SelectedEffect(Image image, bool isSelected, bool doInstant)
        {
            image.DOKill();
            image.DOFade(isSelected ? 1 : 0.6f, doInstant ? 0f : 0.1f);
        }
    }
}
