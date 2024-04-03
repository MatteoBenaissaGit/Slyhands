using Board;
using Camera;
using Common;
using LevelEditor.LoadAndSave;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// This class handle the referencing and the management of all level editor elements
    /// </summary>
    public class LevelEditorManager : Singleton<LevelEditorManager>
    {
        [field:SerializeField] [field:Required] public LevelEditorUI UI { get; private set; }
        [field:SerializeField] [field:Required] public BoardController Board { get; private set; }
        [field:SerializeField] [field:Required] public CameraController Camera { get; private set; }
        [field:SerializeField] [field:Required] public LevelSaveLoadManager SaveLoadManager { get; private set; }

        public SlotLocation CurrentSelectedLocation
        {
            get => _currentSelectedLocation;
            set
            {
                _currentSelectedLocation?.SetSelected(false);
                _currentSelectedLocation = value;
                _currentSelectedLocation?.SetSelected(true);
            }
        }
        private SlotLocation _currentSelectedLocation;

        public SlotLocation CurrentHoveredLocation
        {
            get => _currentHoveredLocation;
            set
            {
                _currentHoveredLocation?.SetHovered(false);
                _currentHoveredLocation = value;
                _currentHoveredLocation?.SetHovered(true);
            }
        }
        private SlotLocation _currentHoveredLocation;

    }
}
