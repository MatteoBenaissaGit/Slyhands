using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorSlotLocation : MonoBehaviour
    {
        [SerializeField, TabGroup("References"), Required] private SlotView _slotViewPrefab;
    }
}