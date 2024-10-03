using Mission;
using UnityEngine;

public class MissionDeckManager : MonoBehaviour
{
    #region Properties

    public static MissionDeckManager Instance;

    [field: SerializeField] public DeckEditorDeckManager DeckEditorDeckManager { get; private set; }
    [field: SerializeField] public DeckEditorUIManager DeckEditorUIManager { get; private set; }
    [field: SerializeField] public MissionData MissionData { get; private set; }

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another MissionDeckManager in this scene !");
        }
    }

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