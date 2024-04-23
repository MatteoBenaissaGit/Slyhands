using System;
using System.Collections.Generic;
using LevelEditor.ActionButtons;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public class ShortcutsGroup
    {
        [field:SerializeField] public LevelEditorActionButtonType ActionType { get; private set; }
        [field: SerializeField] public List<GameObject> Shortcuts { get; private set; } = new List<GameObject>();

        public void Set(bool doShow)
        {
            Shortcuts.ForEach(x => x.SetActive(doShow));
        }
    }
    
    public class LevelEditorUIShortcuts : MonoBehaviour
    {
        [field: SerializeField] public List<ShortcutsGroup> ShortcutsGroups { get; private set; } = new List<ShortcutsGroup>();
        
        public void SetShortcuts(LevelEditorActionButtonType actionType)
        {
            ShortcutsGroups.ForEach(x => x.Set(false));
            ShortcutsGroups.Find(x => x.ActionType == actionType)?.Set(true);
        }
    }
}
