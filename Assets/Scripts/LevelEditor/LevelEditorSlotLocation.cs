using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// This class handle a level editor slot location where the user can put slots and modify them.
    /// It receives the raycast to make action on the slot or on the slot view related to it.
    /// </summary>
    public class LevelEditorSlotLocation : MonoBehaviour
    {
        [SerializeField, TabGroup("References"), Required] private SlotView _slotViewPrefab;

        private SlotView _slotView;
    }
}