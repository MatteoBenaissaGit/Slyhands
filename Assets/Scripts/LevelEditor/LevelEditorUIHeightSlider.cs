using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the height slider used to select the current floor the user want to modify.
    /// </summary>
    public class LevelEditorUIHeightSlider : MonoBehaviour
    {
        [SerializeField, Required] private Slider _slider;
        [SerializeField, Required] private TMP_Text _heightValueText;

        private int _currentHeight;
        
        public void SetSlider(bool doShow)
        {
            gameObject.SetActive(doShow);
            if (doShow == false)
            {
                return;
            }
            
            _slider.minValue = 0;
            _slider.maxValue = LevelEditorManager.Instance.Board.Data.Height - 1;
            Debug.Log(_slider.maxValue);
            
            SetSliderHeight(0);
            
            _slider.onValueChanged.AddListener(SetSliderHeight);
        }

        private void SetSliderHeight(float value)
        {
            _currentHeight = (int)value;
            _slider.value = _currentHeight;
            _heightValueText.text = _currentHeight.ToString();
            
            //board method to show floor
        }
    }
}
