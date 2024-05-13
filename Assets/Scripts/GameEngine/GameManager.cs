using System;
using System.Collections.Generic;
using Board;
using Camera;
using Common;
using Data.Characters;
using Data.Prefabs;
using LevelEditor.LoadAndSave;
using Players;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace GameEngine
{
    public class GameData
    {
        public GameData(GameManager manager)
        {
            _gameManager = manager;
        }
        
        public int Turn { get; set; }
        public Team CurrentTurnTeam { get { return _gameManager.Teams[Turn % _gameManager.Teams.Count]; } }

        private GameManager _gameManager;
    }
    
    public class GameManager : Singleton<GameManager>
    {
        #region Inspector properties
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References", SdfIconType.Archive, TextColor = "red")] [field:Required] 
        public BoardController Board { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public CameraController Camera { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public LevelSaveLoadManager SaveLoadManager { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public GameInputActionsManager InputActionsManager { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "References")] [field:Required] 
        public GameUIManager UI { get; private set; }
        
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "Data", SdfIconType.Bank, TextColor = "orange")] [field:Required] 
        public PrefabsData PrefabsData { get; private set; }
        
        [field:SerializeField] [field:TabGroup("TabGroup1", "Data")] [field:Required] 
        public CharactersData CharactersData { get; private set; }
        

        [field:SerializeField] [field:TabGroup("TabGroup2", "Game Parameters", SdfIconType.Award, TextColor = "blue")] [field:Required]
        public List<Team> Teams { get; set; } = new List<Team>();
        
        [SerializeField] [TabGroup("TabGroup2", "Game Parameters", SdfIconType.Award, TextColor = "blue")]
        private string _levelToLoad;
        
        #endregion

        public TaskManager Task { get; private set; }
        public GameData Data { get; private set; }


        protected override void InternalAwake()
        {
            base.InternalAwake();
            
            Task = new TaskManager();
            Data = new GameData(this);
            
            Teams.ForEach(x => x.Initialize());
        }

        private void Start()
        {
            LevelData levelToLoad = SaveLoadManager.GetLevelsData().Datas.Find(x => x.Name == _levelToLoad);
            if (levelToLoad == null)
            {
                throw new Exception($"no level with name {_levelToLoad}");
            }
            Board.LoadGameLevel(levelToLoad);
            
            UI.SetTurnForTeam(Data.CurrentTurnTeam);
            Data.CurrentTurnTeam.MakeTurn();
        }

        private void Update()
        {
            Task.UpdateTaskQueue();
        }

        public void SetNextTurn()
        {
            Data.CurrentTurnTeam.Player.PlayBehavior?.EndTurn();
            
            Data.Turn++;
            UI.SetTurnForTeam(Data.CurrentTurnTeam);
            
            Data.CurrentTurnTeam.MakeTurn();
        }
    }
}