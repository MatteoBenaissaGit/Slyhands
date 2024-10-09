using UnityEngine;
using UnityEngine.UI;

namespace DeckEditor
{
    public class DeckEditorDeckButton : MonoBehaviour
    {
        private DeckData _deckData;
        private Button _button;

        public void Initialize(DeckData deckData)
        {
            _deckData = deckData;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            MissionDeckManager.Instance.DeckEditorDeckManager.ShowDeck(_deckData);
        }
    }
}