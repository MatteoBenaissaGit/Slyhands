using Data.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace DeckEditor
{
    public class DeckEditorNewDeckButton : MonoBehaviour
    {
        private Deck _deck;
        private Button _button;

        public void Initialize(Deck deck)
        {
            _deck = deck;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            MissionDeckManager.Instance.DeckSaveLoadManager.SaveDeckData(_deck, _deck.Name);
            MissionDeckManager.Instance.DeckSaveLoadManager.GetDecksData();
        }
    }
}