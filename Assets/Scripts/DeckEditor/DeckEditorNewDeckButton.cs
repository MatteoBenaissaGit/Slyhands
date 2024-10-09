using UnityEngine;
using UnityEngine.UI;

namespace DeckEditor
{
    public class DeckEditorNewDeckButton : MonoBehaviour
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
            MissionDeckManager.Instance.DeckSaveLoadManager.SaveDeckData(_deckData, _deckData.Name);
            MissionDeckManager.Instance.DeckSaveLoadManager.GetDecksData();
        }
    }
}