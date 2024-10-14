using Data.Cards;
using UnityEditor;
using UnityEngine;

namespace Card.Editor
{
    public class CardDesignerWindow : EditorWindow
    {
        //Window
        private Vector2 _scrollPosition;

        // Menu State
        private enum MenuState
        {
            CraftCard,
            ModifyCard
        }

        private MenuState currentMenuState = MenuState.CraftCard;

        // ScriptableObjects
        private Data.Cards.Card[] cardDataObjects;
        private Data.Cards.Card _selectedCard = null;

        //Card
        private string _cardName = "";
        private string _cardDescription;
        private Sprite _illustration;
        private Texture2D _illustrationTexture;
        private int _cardPower = 0;
        private CardRarityType _cardRarity;


        [MenuItem("Tools/Card Designer")]
        public static void OpenWindow()
        {
            GetWindow<CardDesignerWindow>("Card Designer");
        }

        private void OnEnable()
        {
            LoadCardDataObjects();
        }

        private void LoadCardDataObjects()
        {
            string[] guids = AssetDatabase.FindAssets("t:CardData", new[] { "Assets/Data/Cards/Cards" });
            cardDataObjects = new Data.Cards.Card[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                cardDataObjects[i] = AssetDatabase.LoadAssetAtPath<Data.Cards.Card>(path);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Card Menu", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Craft Card"))
            {
                currentMenuState = MenuState.CraftCard;
                _selectedCard = null;
                _cardName = null;
                _cardDescription = null;
                _illustration = null;
                _illustrationTexture = null;
                _cardPower = 0;
                _cardRarity = CardRarityType.Bronze;
            }

            if (GUILayout.Button("Modify Card"))
            {
                currentMenuState = MenuState.ModifyCard;
                _selectedCard = null;
                LoadCardDataObjects();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (_selectedCard != null)
            {
                GUILayout.Label("Edit Card", EditorStyles.boldLabel);
                DisplayCraftCardMenu();
            }
            else
            {
                if (currentMenuState == MenuState.CraftCard)
                {
                    GUILayout.Label("Craft Card", EditorStyles.boldLabel);

                    DisplayCraftCardMenu();
                }
                else if (currentMenuState == MenuState.ModifyCard)
                {
                    DisplayModifyCardMenu();
                }
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is Data.Cards.Card selectedCard)
            {
                currentMenuState = MenuState.ModifyCard;

                _selectedCard = selectedCard;
                LoadCardDataIntoForm(_selectedCard);

                Repaint();
            }
            else
            {
                _selectedCard = null;
            }
        }

        private void DisplayCraftCardMenu()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DisplayCardForm();

            EditorGUILayout.EndScrollView();
        }

        private void DisplayModifyCardMenu()
        {
            GUILayout.Label("Modify Existing Cards", EditorStyles.boldLabel);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (cardDataObjects != null)
            {
                foreach (var cardData in cardDataObjects)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(cardData.CardName))
                    {
                        _selectedCard = cardData;
                        currentMenuState = MenuState.CraftCard;
                        LoadCardDataIntoForm(_selectedCard);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(100)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Card",
                                $"Are you sure you want to delete {cardData.CardName} ?", "Yes", "No"))
                        {
                            DeleteCard(cardData);
                            LoadCardDataObjects();
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DisplayCardForm()
        {
            //Name
            GUILayout.Space(20);
            _cardName = EditorGUILayout.TextField("Card Name", _cardName);

            //Description
            GUILayout.Space(20);
            _cardDescription = EditorGUILayout.TextField("Card Description", _cardDescription);

            //Illustration
            GUILayout.Space(20);
            _illustration = (Sprite)EditorGUILayout.ObjectField("Illustration", _illustration, typeof(Sprite), false);
            if (_illustration != null)
            {
                _illustrationTexture = _illustration.texture;
            }

            //Power
            GUILayout.Space(20);
            _cardPower = EditorGUILayout.IntField("Card Power", _cardPower);

            //Rarity
            GUILayout.Space(20);
            _cardRarity = (CardRarityType)EditorGUILayout.EnumPopup("Card Rarity", _cardRarity);

            GUILayout.Space(20);
            GUILayout.Label("Preview:", EditorStyles.boldLabel);

            if (_illustrationTexture != null)
            {
                //Card Background
                float illustrationAspectRatio = (float)_illustrationTexture.width / _illustrationTexture.height;

                float illustrationPreviewWidth = 100f;
                float illustrationPreviewHeight = illustrationPreviewWidth / illustrationAspectRatio;

                Rect cardPreviewRect = GUILayoutUtility.GetRect(illustrationPreviewWidth, illustrationPreviewHeight);
                GUI.DrawTexture(cardPreviewRect, _illustrationTexture, ScaleMode.ScaleToFit);

                //Power
                Rect powerRect = new Rect(cardPreviewRect.width / 2 + illustrationPreviewWidth / 2 - 20f,
                    cardPreviewRect.y + illustrationPreviewHeight - 25, cardPreviewRect.width, 20);
                GUI.Label(powerRect, _cardPower.ToString());
            }
        
            GUILayout.Space(20);

            if (_selectedCard == null)
            {
                if (GUILayout.Button("Create Card"))
                {
                    CreateCard();
                }
            }
            else
            {
                if (GUILayout.Button("Save Modifications"))
                {
                    SaveCardData(_selectedCard);
                }
            }
        }

        private void LoadCardDataIntoForm(Data.Cards.Card card)
        {
            _cardName = card.CardName;
            _cardDescription = card.CardDescription;
            _illustration = card.CardIllustrationSprite;
            _cardPower = card.CardPower;
            _cardRarity = card.RarityType;
        }

        private void SaveCardData(Data.Cards.Card card)
        {
            card.CardName = _cardName;
            card.CardDescription = _cardDescription;
            card.CardIllustrationSprite = _illustration;
            card.CardPower = _cardPower;
            card.RarityType = _cardRarity;

            EditorUtility.SetDirty(card);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Card '{_cardName}' updated.");
        }

        private void CreateCard()
        {
            Data.Cards.Card newCard = CreateInstance<Data.Cards.Card>();

            newCard.CardName = _cardName;
            newCard.CardDescription = _cardDescription;
            newCard.CardIllustrationSprite = _illustration;
            newCard.CardPower = _cardPower;
            newCard.RarityType = _cardRarity;

            string assetPath = $"Assets/Data/Cards/Cards/{_cardName}.asset";
            AssetDatabase.CreateAsset(newCard, assetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Card '{_cardName}' created at {assetPath}.");
        }

        private void DeleteCard(Data.Cards.Card card)
        {
            string path = AssetDatabase.GetAssetPath(card);

            AssetDatabase.DeleteAsset(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Card '{card.CardName}' has been deleted.");
        }
    }
}