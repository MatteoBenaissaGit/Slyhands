using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditorDeckManager : MonoBehaviour
{
    [SerializeField] private bool _inDecksMenu;

    [SerializeField] private GameObject _decksMenu;
    [SerializeField] private GameObject _cardsinDeckMenu;

    private DeckData[] deckDataObjects;

    [SerializeField] private GameObject DefaultDeckButton;
    [SerializeField] private GameObject DefaultCardButtonParent;

    private void Start()
    {
        LoadDecksDatas();
    }

    private void Update()
    {
        _decksMenu.SetActive(_inDecksMenu);
        _cardsinDeckMenu.SetActive(!_inDecksMenu);
    }

    public void ClickOnDeck()
    {
        _inDecksMenu = false;

        LoadCardsDatasInDeck(deckDataObjects[0]);
    }

    public void BackToDecksMenu()
    {
        _inDecksMenu = true;
    }

    private void LoadDecksDatas()
    {
        string[] guids = AssetDatabase.FindAssets("t:DeckData", new[] { "Assets/Data/Cards/Decks" });
        deckDataObjects = new DeckData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            deckDataObjects[i] = AssetDatabase.LoadAssetAtPath<DeckData>(path);

            GameObject newDeckButton = Instantiate(DefaultDeckButton, transform.position, Quaternion.identity);

            newDeckButton.transform.SetParent(DefaultCardButtonParent.transform);

            newDeckButton.GetComponent<Button>().onClick.AddListener(() => ClickOnDeck());
        }
    }

    private void LoadCardsDatasInDeck(DeckData deckData)
    {
    }
}