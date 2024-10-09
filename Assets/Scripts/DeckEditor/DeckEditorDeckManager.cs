using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeckEditor;
using Data.Cards;
using TMPro;

public class DeckEditorDeckManager : MonoBehaviour
{
    [field: SerializeField] public List<DeckData> DeckDataObjects { get; set; }

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

    private void Start()
    {
        LoadDecksDatas();
    }

    private void Update()
    {
        _decksMenu.SetActive(_inDecksMenu);
        _cardsInDeckMenu.SetActive(!_inDecksMenu);
    }

    public void ShowDeck(DeckData deckData)
    {
        _inDecksMenu = false;
        LoadCardsDatasInDeck(deckData);
    }


    public void BackToDecksMenu()
    {
        _inDecksMenu = true;
    }

    private void LoadDecksDatas()
    {
        for (int i = 0; i < DeckDataObjects.Count; i++)
        {
            DeckEditorDeckButton newDeckButton =
                Instantiate(_defaultDeckButton, transform.position, Quaternion.identity);
            newDeckButton.transform.SetParent(_defaultCardButtonParent.transform);
            newDeckButton.GetComponentInChildren<TextMeshProUGUI>().text = DeckDataObjects[i].name;
            newDeckButton.Initialize(DeckDataObjects[i]);
        }
    }

    private void LoadCardsDatasInDeck(DeckData deckData)
    {
        _cardsInDeckText.text = $"{deckData.CardsInDeck.Count} / {_nbCardsInDeckMax}";

        _nbGoldCardsInDeck = deckData.CardsInDeck.Count(card => card.RarityType == CardRarityType.Gold);
        _goldCardsInDeckText.text = $"{_nbGoldCardsInDeck} / {_nbGoldCardsInDeckMax}";

        for (int i = 0; i < deckData.CardsInDeck.Count; i++)
        {
            GameObject newCardSlot = Instantiate(_cardSlot, transform.position, Quaternion.identity);

            newCardSlot.transform.SetParent(_cardSlotParent.transform);

            newCardSlot.GetComponentInChildren<TextMeshProUGUI>().text = deckData.CardsInDeck[i].CardName;
        }
    }
}