using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorManager : Singleton<LevelEditorManager>
    {
        [field:SerializeField] [field:Required] public LevelEditorUI UI { get; private set; }
        [field:SerializeField] [field:Required] public LevelEditorBoard Board { get; private set; }
    }
}
