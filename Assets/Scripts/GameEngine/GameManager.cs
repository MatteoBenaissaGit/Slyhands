using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Camera;
using Common;
using Data.Characters;
using Data.Prefabs;
using Data.Team;
using LevelEditor.LoadAndSave;
using Players;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
        public Team CurrentTurnTeam { get { return _gameManager.TeamsData.Teams[Turn % _gameManager.TeamsData.Teams.Count]; } }

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
        public TeamsData TeamsData { get; set; }
        
        [SerializeField] [TabGroup("TabGroup2", "Game Parameters", SdfIconType.Award, TextColor = "blue")]
        private string _levelToLoad;
        
        #endregion

        public TaskManager TaskManager { get; private set; }
        public GameData Data { get; private set; }


        protected override void InternalAwake()
        {
            base.InternalAwake();
            
            TaskManager = new TaskManager();
            Data = new GameData(this);
        }

        private void Start()
        {
            LevelData levelToLoad = SaveLoadManager.GetLevelsData().Datas.Find(x => String.Equals(x.Name, _levelToLoad, StringComparison.CurrentCultureIgnoreCase));
            if (levelToLoad == null)
            {
                throw new Exception($"no level with name {_levelToLoad}");
            }
            
            TeamsData.Teams.ForEach(x => x.Initialize());
            Board.LoadGameLevel(levelToLoad);

            UI.SetTurnForTeam(Data.CurrentTurnTeam);
            Data.CurrentTurnTeam.MakeTurn();
        }

        private void Update()
        {
            TaskManager.UpdateTaskQueue();
        }

        public async Task SetNextTurn()
        {
            Data.CurrentTurnTeam.Player.PlayBehavior?.EndTurn();
            
            Data.Turn++;
            UI.SetTurnForTeam(Data.CurrentTurnTeam);

            await Task.Delay(0);
            
            Data.CurrentTurnTeam.MakeTurn();
        }
    }
}