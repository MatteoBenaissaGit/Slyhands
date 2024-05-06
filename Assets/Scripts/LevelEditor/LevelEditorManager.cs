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
    }
}
