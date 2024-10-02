using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditorMissionInformationsUIManager : DeckEditorUIManager
{
    [SerializeField] private TMP_Text MissionNameText;
    
    [SerializeField] private Image MissionAccomplishedIcon;
    [SerializeField] private Image MissionAccomplishedCardIcon;
    [SerializeField] private TMP_Text MissionAccomplishedCardCountText;
    
    [SerializeField] private List<Image> StarsObtained;
    [SerializeField] private Image StarsMissingCardIcon;
    [SerializeField] private TMP_Text StarsMissingCardCountText;
    
    [SerializeField] private Image GoldCardIcon;
    [SerializeField] private TMP_Text GoldCardMissingCountText;

    [SerializeField] private TMP_Text MissionSecondaryObjectivesText;

    public override void UpdateUI()
    {
        
    }
}
