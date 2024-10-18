using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Data.Cards;

public class DeckEditorDeckManager : MonoBehaviour
{
    #region Properties

    private MissionDeckManager missionDeckManager;

    [field: SerializeField] public Cards Cards { get; private set; }
    [field: SerializeField] public List<Deck> DeckData { get; private set; }
    [field: SerializeField] public bool InDecksMenu { get; private set; }
    [field: SerializeField] public int NumberCardsInDeckMax { get; private set; }
    [field: SerializeField] public int NumberGoldCardsInDeckMax { get; private set; }

    public int NumberCardsInDeck { get; set; }
    public int NumberGoldCardsInDeck { get; set; }

    [field: BoxGroup("Predef Deck"), SerializeField] public PredefinedDeck PredefinedDeck { get; private set; }

    private Deck _selectedDeck;
    private bool _isCreatingNewDeck;

    #endregion

    #region Methods

    private void Start()
    {
        missionDeckManager = MissionDeckManager.Instance;

        InDecksMenu = true;

        //Save predefs
        Deck prefDeck = new Deck();
        prefDeck.IDCardInDeck = PredefinedDeck.IDCardInDeck;
        prefDeck.Name = PredefinedDeck.Name;

        foreach (var deck in MissionDeckManager.Instance.DeckSaveLoadManager.GetDecksData().Datas)
        {
            if (deck.Name.Contains("Predef"))
                MissionDeckManager.Instance.DeckSaveLoadManager.RemoveDeckData(deck);
        }

        missionDeckManager.DeckSaveLoadManager.SaveDeckData(prefDeck, prefDeck.Name);
        //Fin

        LoadDecksDatas();
        missionDeckManager.DeckEditorUIManager.LoadCollectionCards();
    }

    public void ShowDeck(Deck deck)
    {
        InDecksMenu = false;
        missionDeckManager.DeckEditorUIManager.LoadCardsDatasInDeckUI(deck);
        _selectedDeck = deck;
    }

    public void BackToDecksMenu()
    {
        InDecksMenu = true;
        missionDeckManager.DeckEditorUIManager.RefreshDecksButtonsList();
        LoadDecksDatas();
    }

    private void LoadDecksDatas()
    {
        DeckData = missionDeckManager.DeckSaveLoadManager.GetDecksData().Datas;
        missionDeckManager.DeckEditorUIManager.LoadDecksDatasUI(DeckData);
    }

    public void DeleteDeck()
    {
        missionDeckManager.DeckSaveLoadManager.RemoveDeckData(_selectedDeck);
        BackToDecksMenu();
    }

    public void CreateNewDeckData()
    {
        _isCreatingNewDeck = true;
        LoadDecksDatas();
    }

    #endregion
}