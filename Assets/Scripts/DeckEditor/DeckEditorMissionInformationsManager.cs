using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckEditorMissionInformationsManager : DeckEditorManager
{
    #region Properties

    private string MissionName;
    private bool MissionAccomplished;
    private int StarObtained;
    private int GoldCardsInLevelCount;
    private string MissionSecondaryObjectives;

    #endregion

    #region Methods

    public override void LoadData(LevelInformationsData levelInformationsData)
    {
        MissionName = levelInformationsData.MissionName;
        MissionAccomplished = levelInformationsData.MissionAccomplished;
        StarObtained = levelInformationsData.StarObtained;
        GoldCardsInLevelCount = levelInformationsData.GoldCardsInLevelCount;
        MissionSecondaryObjectives = levelInformationsData.MissionSecondaryObjectivesText;
        
        base.LoadData(levelInformationsData);
    }

    public override void SaveData(LevelInformationsData levelInformationsData)
    {
        levelInformationsData.MissionAccomplished = MissionAccomplished;
        levelInformationsData.StarObtained = StarObtained;
        levelInformationsData.GoldCardsInLevelCount = GoldCardsInLevelCount;
        levelInformationsData.MissionSecondaryObjectivesText = MissionSecondaryObjectives;
    }
    
    #endregion
}