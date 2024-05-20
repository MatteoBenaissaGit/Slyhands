using TMPro;
using UnityEngine;

namespace LevelEditor.Entities
{
    public class LevelEditorRoadBeacon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetBeaconNumber(int number)
        {
            _text.text = number.ToString();
        }
    }
}
