using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckEditorManager : MonoBehaviour
{
    public static DeckEditorManager Instance;
    
    [field: SerializeField] public DeckEditorUIManager DeckEditorUIManager { get; private set; }
    
    [field: SerializeField] public DeckEditorMissionInformationsManager DeckEditorMissionInformationsManager { get; private set; }
    [field: SerializeField] public DeckEditorDecksManager DeckEditorDecksManager { get; private set; }
    [field: SerializeField] public DeckEditorCardSortingManager DeckEditorCardSortingManager { get; private set; }
    [field: SerializeField] public DeckEditorCardCollectionManager DeckEditorCardCollectionManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another DeckEditorManager in this scene !");
        }
    }

    public virtual void LoadData(LevelInformationsData levelInformationsData)
    {
        DeckEditorUIManager.UpdateUI();
    }

    public virtual void SaveData(LevelInformationsData levelInformationsData)
    {
        
    }
}