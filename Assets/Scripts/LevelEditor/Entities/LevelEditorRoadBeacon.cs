using TMPro;
using UnityEngine;

namespace LevelEditor.Entities
{
    /// <summary>
    /// This class manage the road beacon used to draw roads in the level editor
    /// </summary>
    public class LevelEditorRoadBeacon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetBeaconNumber(int number)
        {
            _text.text = number.ToString();
        }
    }
}
