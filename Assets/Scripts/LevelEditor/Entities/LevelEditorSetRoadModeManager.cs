using LevelEditor.ActionButtons;
using Slots;
using UnityEngine;

namespace LevelEditor.Entities
{
    public class LevelEditorSetRoadModeManager
    {
        public LevelEditorCharacter CurrentCharacter { get; private set; }
        
        public void StartMode(LevelEditorCharacter character)
        {
            CurrentCharacter = character;

            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(slotLocation.SlotView.LevelEditorCharacterOnSlot == CurrentCharacter);
            }
            
            LevelEditorManager.Instance.UI.Shortcuts.SetShortcuts(LevelEditorActionButtonType.SetRoad);
        }

        public void ExitMode()
        {
            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(true);
            }
            
            //TODO save road for character
        }
    }
}