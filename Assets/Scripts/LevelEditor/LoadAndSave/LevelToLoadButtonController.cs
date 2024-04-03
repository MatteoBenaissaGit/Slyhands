using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.LoadAndSave
{
    /// <summary>
    /// This class handle the button used to select a level to load in the load menu
    /// </summary>
    public class LevelToLoadButtonController : MonoBehaviour
    {
        [field:SerializeField] [field:Required] public RectTransform Rect { get; private set; }
        
        public LevelData Data { get; private set; }
        
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private TMP_Text _nameText;

        public Button Initialize(LevelData level)
        {
            _nameText.text = level.Name;
            Data = level;
            SetSelected(false);
            return _button;
        }

        public void SetSelected(bool isSelected)
        {
            _button.image.color = isSelected ? Color.white : new Color(1f, 1f, 1f, 0.6f);
        }
    }
}
