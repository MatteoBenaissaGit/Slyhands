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
        private CardData[] cardDataObjects;
        private CardData selectedCardData = null;

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
            cardDataObjects = new CardData[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                cardDataObjects[i] = AssetDatabase.LoadAssetAtPath<CardData>(path);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Card Menu", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Craft Card"))
            {
                currentMenuState = MenuState.CraftCard;
                selectedCardData = null;
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
                selectedCardData = null;
                LoadCardDataObjects();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (selectedCardData != null)
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
            if (Selection.activeObject is CardData selectedCard)
            {
                currentMenuState = MenuState.ModifyCard;

                selectedCardData = selectedCard;
                LoadCardDataIntoForm(selectedCardData);

                Repaint();
            }
            else
            {
                selectedCardData = null;
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
                        selectedCardData = cardData;
                        currentMenuState = MenuState.CraftCard;
                        LoadCardDataIntoForm(selectedCardData);
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

            if (selectedCardData == null)
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
                    SaveCardData(selectedCardData);
                }
            }
        }

        private void LoadCardDataIntoForm(CardData cardData)
        {
            _cardName = cardData.CardName;
            _cardDescription = cardData.CardDescription;
            _illustration = cardData.CardIllustrationSprite;
            _cardPower = cardData.CardPower;
            _cardRarity = cardData.RarityType;
        }

        private void SaveCardData(CardData cardData)
        {
            cardData.CardName = _cardName;
            cardData.CardDescription = _cardDescription;
            cardData.CardIllustrationSprite = _illustration;
            cardData.CardPower = _cardPower;
            cardData.RarityType = _cardRarity;

            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Card '{_cardName}' updated.");
        }

        private void CreateCard()
        {
            CardData newCard = CreateInstance<CardData>();

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

        private void DeleteCard(CardData cardData)
        {
            string path = AssetDatabase.GetAssetPath(cardData);

            AssetDatabase.DeleteAsset(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Card '{cardData.CardName}' has been deleted.");
        }
    }
}