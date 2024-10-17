using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeckEditor;
using Data.Cards;
using Sirenix.OdinInspector;
using TMPro;

public class DeckEditorDeckManager : MonoBehaviour
{
    [field: SerializeField] public Cards Cards { get; private set; }
    [field: SerializeField] public List<Deck> DeckData { get; set; }
    [field: SerializeField] public List<GameObject> DecksButtons { get; set; }
    [field: SerializeField] public List<GameObject> CardsSlot { get; set; }

    [SerializeField] private bool _inDecksMenu;

    [SerializeField] private GameObject _decksMenu;
    [SerializeField] private GameObject _cardsInDeckMenu;

    [SerializeField] private DeckEditorDeckButton _defaultDeckButton;
    [SerializeField] private GameObject _defaultCardButtonParent;

    [SerializeField] private GameObject _cardSlot;
    [SerializeField] private GameObject _cardSlotParent;

    [SerializeField] private int _nbCardsInDeckMax;
    private int _nbCardsInDeck;
    [SerializeField] private TextMeshProUGUI _cardsInDeckText;

    [SerializeField] private int _nbGoldCardsInDeckMax;
    private int _nbGoldCardsInDeck;
    [SerializeField] private TextMeshProUGUI _goldCardsInDeckText;

    private Deck _selectedDeck;
    private bool _isCreatingNewDeck;

    [BoxGroup("Predef Deck")]
    [field: SerializeField]
    public PredefinedDeck PredefinedDeck { get; set; }

    private void Start()
    {
        //TODO save predefs
        Deck prefDeck = new Deck();
        prefDeck.IDCardInDeck = PredefinedDeck.IDCardInDeck;
        prefDeck.Name = PredefinedDeck.Name;

        foreach (var deck in MissionDeckManager.Instance.DeckSaveLoadManager.GetDecksData().Datas)
        {
            if (deck.Name.Contains("Predef"))
                MissionDeckManager.Instance.DeckSaveLoadManager.RemoveDeckData(deck);
        }

        MissionDeckManager.Instance.DeckSaveLoadManager.SaveDeckData(prefDeck, prefDeck.Name);

        LoadDecksDatas();
    }

    private void Update()
    {
        _decksMenu.SetActive(_inDecksMenu);
        _cardsInDeckMenu.SetActive(!_inDecksMenu);
    }

    public void ShowDeck(Deck deck)
    {
        _inDecksMenu = false;
        LoadCardsDatasInDeck(deck);
        _selectedDeck = deck;
    }

    public void BackToDecksMenu()
    {
        _inDecksMenu = true;
        RefreshDecksButtonsList();
        LoadDecksDatas();
    }

    private void RefreshDecksButtonsList()
    {
        foreach (var button in DecksButtons)
        {
            button.SetActive(false);
        }
    }

    private void RefreshCardsSlotList()
    {
        foreach (var slot in CardsSlot)
        {
            slot.SetActive(false);
        }
    }
    
    private void LoadDecksDatas()
    {
        DeckData = MissionDeckManager.Instance.DeckSaveLoadManager.GetDecksData().Datas;

        Debug.Log($"Loading decks from datas : {DeckData.Count} decks found");

        for (int i = 0; i < DeckData.Count; i++)
        {
            DecksButtons[i].SetActive(true);
            DecksButtons[i].GetComponent<DeckEditorDeckButton>().Initialize(DeckData[i]);
            //DeckData[i].Name = $"Deck {i + 1}";
            DecksButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = DeckData[i].Name;
        }
    }

    private void LoadCardsDatasInDeck(Deck deck)
    {
        RefreshCardsSlotList();
            
        _cardsInDeckText.text = $"{deck.IDCardInDeck.Count} / {_nbCardsInDeckMax}";

        _nbGoldCardsInDeck = deck.IDCardInDeck.Count(ID => Cards.GetCardData(ID).RarityType == CardRarityType.Gold);
        _goldCardsInDeckText.text = $"{_nbGoldCardsInDeck} / {_nbGoldCardsInDeckMax}";

        for (int i = 0; i < deck.IDCardInDeck.Count; i++)
        {
            CardsSlot[i].SetActive(true);

            CardsSlot[i].GetComponentInChildren<TextMeshProUGUI>().text =
                Cards.GetCardData(deck.IDCardInDeck[i]).CardName;
        }
    }

    public void DeleteDeck()
    {
        Debug.Log(_selectedDeck.Name);
        MissionDeckManager.Instance.DeckSaveLoadManager.RemoveDeckData(_selectedDeck);
        BackToDecksMenu();
    }

    public void CreateNewDeckData()
    {
        Deck newDeck = new Deck();
        _isCreatingNewDeck = true;

        LoadDecksDatas();
    }
}