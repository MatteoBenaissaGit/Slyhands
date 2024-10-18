using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using DeckEditor;
using Data.Cards;

public class DeckEditorUIManager : MonoBehaviour
{
    #region Properties

    private MissionDeckManager missionDeckManager;

    #region Mission Informations

    [BoxGroup("Mission Informations"), SerializeField] private TMP_Text _missionNameText;
    [BoxGroup("Mission Informations"), SerializeField] private Image _missionComplishedIcon;
    [BoxGroup("Mission Informations"), SerializeField] private Image _completingRewardsIcon;
    [BoxGroup("Mission Informations"), SerializeField] private TMP_Text _completingRewardsCountText;
    [BoxGroup("Mission Informations"), SerializeField] private List<Image> _starsIcons;
    [BoxGroup("Mission Informations"), SerializeField] private Image _starsRewardIcon;
    [BoxGroup("Mission Informations"), SerializeField] private TMP_Text _starsRewardText;
    [BoxGroup("Mission Informations"), SerializeField] private Image _goldRewardIcon;
    [BoxGroup("Mission Informations"), SerializeField] private TMP_Text _goldRewardText;
    [BoxGroup("Mission Informations"), SerializeField] private TMP_Text _secondaryObjectivesText;

    #endregion

    #region Decks Informations

    [field: Space(20), BoxGroup("Decks Informations"), SerializeField] private List<GameObject> _decksButtons;
    [BoxGroup("Decks Informations"), SerializeField] private List<GameObject> _deckCardsSlot;
    [BoxGroup("Decks Informations"), SerializeField] private GameObject _decksMenu;
    [BoxGroup("Decks Informations"), SerializeField] private GameObject _cardsInDeckMenu;
    [BoxGroup("Decks Informations"), SerializeField] private DeckEditorDeckButton _defaultDeckButton;
    [BoxGroup("Decks Informations"), SerializeField] private GameObject _defaultCardButtonParent;
    [BoxGroup("Decks Informations"), SerializeField] private GameObject _deckCardSlot;
    [BoxGroup("Decks Informations"), SerializeField] private GameObject _cardSlotParent;
    [BoxGroup("Decks Informations"), SerializeField] private TextMeshProUGUI _cardsInDeckText;
    [BoxGroup("Decks Informations"), SerializeField] private TextMeshProUGUI _goldCardsInDeckText;

    #endregion

    #region Cards Collection Informations

    [field: Space(20), BoxGroup("Cards Collection"), SerializeField] public List<GameObject> CollectionCardsSlot { get; private set; }

    #endregion

    #endregion

    #region Methods

    private void Start()
    {
        missionDeckManager = MissionDeckManager.Instance;
    }

    private void Update()
    {
        _decksMenu.SetActive(missionDeckManager.DeckEditorDeckManager.InDecksMenu);
        _cardsInDeckMenu.SetActive(!missionDeckManager.DeckEditorDeckManager.InDecksMenu);
    }

    public void LoadMissionDataUI()
    {
        var missionData = missionDeckManager.MissionData;

        _missionNameText.text = missionData.MissionName;

        _missionComplishedIcon.gameObject.SetActive(missionData.MissionComplished);
        _completingRewardsIcon.gameObject.SetActive(missionData.MissionComplished);
        _completingRewardsCountText.gameObject.SetActive(!missionData.MissionComplished);

        for (int i = 0; i < missionData.StarObtained; i++)
        {
            _starsIcons[i].gameObject.SetActive(true);
        }

        _starsRewardIcon.gameObject.SetActive(missionData.StarObtained == 3);

        _starsRewardText.text = (3 - missionData.StarObtained).ToString();

        _goldRewardIcon.gameObject.SetActive(missionData.GoldCardToFound == 0);
        _goldRewardText.text = missionData.GoldCardToFound.ToString();

        _secondaryObjectivesText.text = missionData.SecondaryMission;
    }

    public void LoadDecksDatasUI(List<Deck> deckData)
    {
        for (int i = 0; i < deckData.Count; i++)
        {
            _decksButtons[i].SetActive(true);
            _decksButtons[i].GetComponent<DeckEditorDeckButton>().Initialize(deckData[i]);
            _decksButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = deckData[i].Name;
        }
    }

    public void LoadCardsDatasInDeckUI(Deck deck)
    {
        RefreshCardsSlotList();

        _cardsInDeckText.text = $"{deck.IDCardInDeck.Count} / {missionDeckManager.DeckEditorDeckManager.NumberCardsInDeckMax}";

        missionDeckManager.DeckEditorDeckManager.NumberGoldCardsInDeck = deck.IDCardInDeck.Count(ID =>
            missionDeckManager.DeckEditorDeckManager.Cards.GetCardData(ID).RarityType == CardRarityType.Gold);

        _goldCardsInDeckText.text = $"{missionDeckManager.DeckEditorDeckManager.NumberGoldCardsInDeck} / {missionDeckManager.DeckEditorDeckManager.NumberGoldCardsInDeckMax}";

        for (int i = 0; i < deck.IDCardInDeck.Count; i++)
        {
            _deckCardsSlot[i].SetActive(true);

            _deckCardsSlot[i].GetComponentInChildren<TextMeshProUGUI>().text =
                missionDeckManager.DeckEditorDeckManager.Cards.GetCardData(deck.IDCardInDeck[i]).CardName;
        }
    }

    public void LoadCollectionCards()
    {
        var cards = MissionDeckManager.Instance.DeckEditorDeckManager.Cards;

        for (var i = 0; i < cards.GetAllCards.Count; i++)
        {
            CollectionCardsSlot[i].SetActive(true);
            CollectionCardsSlot[i].GetComponent<Image>().sprite = cards.GetAllCards[i].CardIllustrationSprite;
            CollectionCardsSlot[i].GetComponentInChildren<TMP_Text>().text = cards.GetAllCards[i].CardPower.ToString();
        }
    }

    private void RefreshCardsSlotList()
    {
        foreach (var slot in _deckCardsSlot)
        {
            slot.SetActive(false);
        }
    }

    public void RefreshDecksButtonsList()
    {
        foreach (var button in _decksButtons)
        {
            button.SetActive(false);
        }
    }

    #endregion
}