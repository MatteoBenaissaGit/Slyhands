using System;
using Board;
using Camera;
using Common;
using LevelEditor.LoadAndSave;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public class GameManager : Singleton<GameManager>
    {
        [field:SerializeField] [field:TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red")] [field:Required]
        public BoardController Board { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public CameraController Camera { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public LevelSaveLoadManager SaveLoadManager { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public GameInputActionsManager InputActionsManager { get; private set; }

        [SerializeField] private string _levelToLoad;

        private void Start()
        {
            LevelData levelToLoad = SaveLoadManager.GetLevelsData().Datas.Find(x => x.Name == _levelToLoad);
            if (levelToLoad == null)
            {
                throw new Exception($"no level with name {_levelToLoad}");
            }
            Board.LoadGameLevel(levelToLoad);
        }
    }
}