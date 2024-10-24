using System;
using System.Collections.Generic;
using System.IO;
using Board;
using Data.Cards;
using LevelEditor.LoadAndSave;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


/// <summary>
/// This class handle the saved data of all maps
/// </summary>
[Serializable]
public class DecksData
{
    public List<Deck> Datas;

    public DecksData()
    {
        Datas = new List<Deck>();
    }
}


public class DeckSaveLoadManager : MonoBehaviour
{
    [SerializeField] private bool _saveAndLoadLocally;
    [SerializeField] private bool _hideResetDataButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            string[] guids = AssetDatabase.FindAssets("t:DeckData", new string[] { "Assets/Data/Cards/Decks" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Deck deck = AssetDatabase.LoadAssetAtPath<Deck>(path);
                SaveDeckData(deck, deck.Name);
                
            }
        }
    }

    private string FilePath
    {
        get
        {
            return _saveAndLoadLocally
                ? Path.Combine(Application.persistentDataPath, "decksData.json")
                : Path.Combine(Application.dataPath, "decksData.json");
        }
    }

    public void Start()
    {
        if (File.Exists(FilePath) == false)
        {
            ResetData();
        }

        DecksData loadedData = ReadFromJson();

        //Debug.Log($"Loaded {loadedData.Datas.Count} Decks data in {_filePath}");
    }

    /// <summary>
    /// Reset and erase all saved data
    /// </summary>
    [HideIf("_hideResetDataButton"), Button("Reset all decks data")]
    private void ResetData()
    {
        WriteToJson(new DecksData());
    }

    /// <summary>
    /// Write a dekcs data into the json file
    /// </summary>
    /// <param name="decksData">The decks data to save</param>
    private void WriteToJson(DecksData decksData)
    {
        string json = JsonUtility.ToJson(decksData, true);
        File.WriteAllText(FilePath, json);

        Debug.Log($"Saved {decksData.Datas.Count} Decks data in {FilePath}");
    }

    /// <summary>
    /// Read the decks data from the json file
    /// </summary>
    /// <returns></returns>
    private DecksData ReadFromJson()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            DecksData data = JsonUtility.FromJson<DecksData>(json);
            return data;
        }
        else
        {
            Debug.LogError("JSON file not found at: " + FilePath);
            return null;
        }
    }

    /// <summary>
    /// Save a deck in the game data
    /// </summary>
    /// <param name="deckName">The deck's name</param>
    public void SaveDeckData(Deck deckData, string deckName)
    {
        deckName = deckName.ToUpper();
        DecksData loadedData = ReadFromJson();

        //erase same name deck if it exists
        foreach (Deck deck in loadedData.Datas)
        {
            if (deck.Name != deckName)
            {
                continue;
            }

            loadedData.Datas.Remove(deck);
            break;
        }

        loadedData.Datas.Add(deckData);
        WriteToJson(loadedData);
    }

    /// <summary>
    /// Remove a deck data from game data
    /// </summary>
    /// <param name="deckData">deck data to remove</param>
    public void RemoveDeckData(Deck deckData)
    {
        DecksData loadedData = GetDecksData();
        loadedData.Datas.Remove(loadedData.Datas.Find(x => x.Name == deckData.Name));
        WriteToJson(loadedData);
    }

    /// <summary>
    /// Get the decks data of the game
    /// </summary>
    /// <returns>the decks data of the game</returns>
    public DecksData GetDecksData()
    {
        return ReadFromJson();
    }

    [Button("Open in explorer")]
    public void OpenInExplorer()
    {
        System.Diagnostics.Process.Start(_saveAndLoadLocally ? Application.persistentDataPath : Application.dataPath);
    }
}