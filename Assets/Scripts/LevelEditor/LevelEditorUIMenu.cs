using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public abstract class LevelEditorUIMenu : MonoBehaviour
    {
        [field:SerializeField] [field:Required]
        public CanvasGroup CanvasGroup { get; private set; } 
        
        [field:SerializeField] [field:Required]
        protected Button CancelButton { get; set; }
    }
}
