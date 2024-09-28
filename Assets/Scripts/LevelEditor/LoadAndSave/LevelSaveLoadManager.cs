using System;
using System.Collections.Generic;
using System.IO;
using Board;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor.LoadAndSave
{
    /// <summary>
    /// This class handle the saved data of all maps
    /// </summary>
    [Serializable]
    public class LevelsData
    {
        [field:SerializeField] public List<LevelData> Datas;
        
        public LevelsData()
        {
            Datas = new List<LevelData>();
        }
    }
    
    /// <summary>
    /// This class handle the saved data of a map
    /// </summary>
    [Serializable]
    public class LevelData
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public BoardData BoardData { get; private set; }
        [field:SerializeField] public List<SlotData> SlotDataList { get; private set; }
        
        public LevelData(BoardData boardData, string name)
        {
            Name = name;
            BoardData = boardData;

            SlotDataList = new List<SlotData>(boardData.Width * boardData.Length * boardData.Height);
            foreach (SlotLocation slotLocation in BoardData.SlotLocations)
            {
                SlotData data = slotLocation?.SlotView?.Controller?.Data;
                if (data == null)
                {
                    continue;
                }
                SlotDataList.Add(data);
            }
        }
    }
    
    /// <summary>
    /// This class handle the loading and saving of levels
    /// </summary>
    public class LevelSaveLoadManager : MonoBehaviour
    {
        [SerializeField] private bool _saveAndLoadLocally;
        [SerializeField] private bool _hideResetDataButton;
        
        private string FilePath
        {
            get
            {
                return _saveAndLoadLocally ? 
                    Path.Combine(Application.persistentDataPath, "levelsData.json") : 
                    Path.Combine(Application.dataPath, "levelsData.json");
            }
        }

        public void Start()
        {
            if (File.Exists(FilePath) == false)
            {
                ResetData();
            }
            LevelsData loadedData = ReadFromJson();

            //Debug.Log($"Loaded {loadedData.Datas.Count} Levels data in {_filePath}");
        }

        /// <summary>
        /// Reset and erase all saved data
        /// </summary>
        [HideIf("_hideResetDataButton"), Button("Reset all levels data")]
        private void ResetData()
        {
            WriteToJson(new LevelsData());
        }
        
        /// <summary>
        /// Write a levels data into the json file
        /// </summary>
        /// <param name="levelsData">The levels data to save</param>
        private void WriteToJson(LevelsData levelsData)
        {
            string json = JsonUtility.ToJson(levelsData, true);
            File.WriteAllText(FilePath, json);
            
            Debug.Log($"Saved {levelsData.Datas.Count} Levels data in {FilePath}");
        }

        /// <summary>
        /// Read the levels data from the json file
        /// </summary>
        /// <returns></returns>
        private LevelsData ReadFromJson()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                LevelsData data = JsonUtility.FromJson<LevelsData>(json);
                return data;
            }
            else
            {
                Debug.LogError("JSON file not found at: " + FilePath);
                return null;
            }
        }

        /// <summary>
        /// Save a level in the game data
        /// </summary>
        /// <param name="boardData">The board data to save</param>
        /// <param name="levelName">The level's name</param>
        public void SaveLevelData(BoardData boardData, string levelName)
        {
            levelName = levelName.ToUpper();
            LevelData levelData = new LevelData(boardData, levelName);
            LevelsData loadedData = ReadFromJson();
            
            //erase same name level if it exists
            foreach (LevelData level in loadedData.Datas)
            {
                if (level.Name != levelName)
                {
                    continue;
                }
                loadedData.Datas.Remove(level);
                break;
            }
            
            loadedData.Datas.Add(levelData);
            WriteToJson(loadedData);
        }

        /// <summary>
        /// Remove a level data from game data
        /// </summary>
        /// <param name="levelData">level data to remove</param>
        public void RemoveLevelData(LevelData levelData)
        {
            LevelsData loadedData = GetLevelsData();
            loadedData.Datas.Remove(loadedData.Datas.Find(x => x.Name == levelData.Name));
            WriteToJson(loadedData);
        }

        /// <summary>
        /// Get the levels data of the game
        /// </summary>
        /// <returns>the levels data of the game</returns>
        public LevelsData GetLevelsData()
        {
            return ReadFromJson();
        }
        
        [Button("Open in explorer")]
        public void OpenInExplorer()
        {
            System.Diagnostics.Process.Start(_saveAndLoadLocally ? Application.persistentDataPath : Application.dataPath);
        }
    }
}