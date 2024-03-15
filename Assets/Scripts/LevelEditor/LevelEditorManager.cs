using Board;
using Common;
using Sirenix.OdinInspector;
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
    }
}
