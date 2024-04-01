using System;
using System.Collections.Generic;
using System.IO;
using Board;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public class LevelsData
    {
        public List<LevelData> Datas;
        
        public LevelsData()
        {
            Datas = new List<LevelData>();
        }
    }
    
    [Serializable]
    public class LevelData
    {
        public string Name;
        public BoardData BoardData;
        public SlotData[,,] SlotDataArray;
        
        public LevelData(BoardData boardData, string name)
        {
            Name = name;
            BoardData = boardData;
            SlotDataArray = new SlotData[BoardData.Width, BoardData.Height, BoardData.Length];
            foreach (SlotLocation slotLocation in BoardData.SlotLocations)
            {
                SlotData slotData = slotLocation?.SlotView?.Controller?.Data;
                if (slotData == null)
                {
                    continue;
                }
                SlotDataArray[slotLocation.Coordinates.x, slotLocation.Coordinates.y, slotLocation.Coordinates.z] = slotData;
            }
        }
    }
    
    public class LevelSaveLoadManager : MonoBehaviour
    {
        private string _filePath => Path.Combine(Application.persistentDataPath, "levelsData.json");

        public void Start()
        {
            if (File.Exists(_filePath) == false)
            {
                ResetData();
            }
            LevelsData loadedData = ReadFromJson();

            Debug.Log($"Loaded {loadedData.Datas.Count} Levels data in {_filePath}");
        }

        [Button("Reset all levels data")]
        private void ResetData()
        {
            WriteToJson(new LevelsData());
        }
        
        private void WriteToJson(LevelsData levelData)
        {
            string json = JsonUtility.ToJson(levelData);
            File.WriteAllText(_filePath, json);
            
            Debug.Log($"Saved {levelData.Datas.Count} Levels data in {_filePath}");
        }

        private LevelsData ReadFromJson()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                LevelsData data = JsonUtility.FromJson<LevelsData>(json);
                return data;
            }
            else
            {
                Debug.LogError("JSON file not found at: " + _filePath);
                return null;
            }
        }

        public void SaveLevelData(BoardData boardData, string levelName)
        {
            LevelData levelData = new LevelData(boardData, levelName);
            LevelsData loadedData = ReadFromJson();
            loadedData.Datas.Add(levelData);
            WriteToJson(loadedData);
        }

        public LevelsData GetLevelsData()
        {
            return ReadFromJson();
        }
    }
}