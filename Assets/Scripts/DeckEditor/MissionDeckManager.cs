using Common;
using Mission;
using UnityEngine;

public class MissionDeckManager : Singleton<MissionDeckManager>
{
    #region Properties

    [field: SerializeField] public DeckSaveLoadManager DeckSaveLoadManager { get; set; }
    [field: SerializeField] public DeckEditorDeckManager DeckEditorDeckManager { get; private set; }
    [field: SerializeField] public DeckEditorUIManager DeckEditorUIManager { get; private set; }
    [field: SerializeField] public MissionData MissionData { get; private set; }

    #endregion

    #region Methods

    private void Start()
    {
        LoadDeckEditorDeckManager();    
    }
    
    private void LoadDeckEditorDeckManager()
    {
        DeckEditorUIManager.LoadMissionData();
    }

    #endregion
}