using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This abstract class serve as a base for all level editors menus
    /// </summary>
    public abstract class LevelEditorUIMenu : MonoBehaviour
    {
        [field:SerializeField] [field:Required]
        public CanvasGroup CanvasGroup { get; private set; } 
        
        [field:SerializeField] [field:Required]
        protected Button CancelButton { get; set; }

        protected virtual void Awake()
        {
            CancelButton.onClick.AddListener(Cancel);
        }

        protected virtual void OnDestroy()
        {
            CancelButton.onClick.RemoveListener(Cancel);
        }

        protected virtual void Cancel()
        {
            LevelEditorManager.Instance.UI.HideMenu();
        }
    }
}
