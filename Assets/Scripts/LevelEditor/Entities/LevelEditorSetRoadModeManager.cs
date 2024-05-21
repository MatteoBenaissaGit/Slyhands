using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using DG.Tweening;
using Inputs;
using LevelEditor.ActionButtons;
using Slots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LevelEditor.Entities
{
    public enum RoadFollowMode
    {
        PingPong = 0,
        Loop = 1,
    }
    
    public class LevelEditorSetRoadModeManager
    {
        public LevelEditorCharacter CurrentCharacter { get; private set; }

        private List<Vector3Int> _currentRoadPositions;
        private List<LevelEditorRoadBeacon> _currentBeacons;
        
        private LineRenderer _roadLineRenderer;
        private RoadFollowMode _currentMode;

        public void StartMode(LevelEditorCharacter character)
        {
            CurrentCharacter = character;
            _currentRoadPositions = new List<Vector3Int>(){CurrentCharacter.Coordinates};
            _currentBeacons = new List<LevelEditorRoadBeacon>() { null };

            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(slotLocation.SlotView.LevelEditorCharacterOnSlot == CurrentCharacter);
            }
            
            LevelEditorManager.Instance.UI.Shortcuts.SetShortcuts(LevelEditorActionButtonType.SetRoad);

            _roadLineRenderer = Object.Instantiate(LevelEditorManager.Instance.PrefabsData.GetPrefab("LevelEditorRoadLine")).GetComponent<LineRenderer>();
            Color color = CurrentCharacter.GetTeam().TeamColor;
            _roadLineRenderer.startColor = color;
            _roadLineRenderer.endColor = color;
            
            InputManager.Instance.LevelEditorInput.OnCameraMoved += _ => UpdateRoad();
            InputManager.Instance.LevelEditorInput.OnLeftClick += AddBeacon;
            InputManager.Instance.LevelEditorInput.OnRightClick += RemoveBeacon;
            
            LevelEditorManager.Instance.UI.Shortcuts.SetRoadShortcuts.ForEach(x => x.SetActive(true));
        }

        public void ExitMode()
        {
            foreach (SlotLocation slotLocation in LevelEditorManager.Instance.Board.Data.SlotLocations)
            {
                slotLocation.SlotView?.LevelEditorCharacterOnSlot?.SetActive(true);
            }
            
            Object.Destroy(_roadLineRenderer);
            _currentRoadPositions = null;
            _currentBeacons.FindAll(x => x != null).ForEach(x => Object.Destroy(x.gameObject));
            
            InputManager.Instance.LevelEditorInput.OnCameraMoved -= _ => UpdateRoad();
            InputManager.Instance.LevelEditorInput.OnLeftClick -= AddBeacon;
            InputManager.Instance.LevelEditorInput.OnRightClick -= RemoveBeacon;

            CurrentCharacter = null;
            
            LevelEditorManager.Instance.UI.Shortcuts.SetRoadShortcuts.ForEach(x => x.SetActive(false));
        }

        private void AddBeacon()
        {
            SlotLocation slotLocation = LevelEditorManager.Instance.Board.CurrentHoveredLocation;
            if (slotLocation == null || slotLocation.SlotView == null)
            {
                return;
            }
            
            _currentRoadPositions.Add(slotLocation.Coordinates);

            if (slotLocation.SlotView.LevelEditorCharacterOnSlot == CurrentCharacter)
            {
                _currentBeacons.Add(null);
                return;
            }
            
            _currentBeacons.Add(Object.Instantiate(LevelEditorManager.Instance.PrefabsData.GetPrefab("LevelEditorRoadBeacon").GetComponent<LevelEditorRoadBeacon>()));
            
            LevelEditorRoadBeacon beacon = _currentBeacons[^1];
            beacon.transform.position = LevelEditorManager.Instance.Board.GetCoordinatesToWorldPosition(slotLocation.Coordinates);
            beacon.SetBeaconNumber(_currentBeacons.Count-1);
            beacon.transform.parent = slotLocation.SlotView.transform;
            
            int yOffset = _currentRoadPositions.FindAll(x => x == slotLocation.Coordinates).Count - 1;
            beacon.transform.position += new Vector3(0, yOffset * 0.9f, 0);

            beacon.transform.localScale = Vector3.zero;
            beacon.transform.DOComplete();
            beacon.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InOutBounce);
        }

        private void RemoveBeacon()
        {
            if (_currentRoadPositions.Count <= 1)
            {
                return;
            }
            
            _currentRoadPositions.RemoveAt(_currentRoadPositions.Count - 1);
            
            LevelEditorRoadBeacon beaconToRemove = _currentBeacons[^1];
            _currentBeacons.RemoveAt(_currentBeacons.Count - 1);
            if (beaconToRemove != null)
            {
                beaconToRemove.transform.DOComplete();
                beaconToRemove.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InExpo).OnComplete(() => Object.Destroy(beaconToRemove.gameObject));
            }
            
            UpdateRoad();
        }
        
        private void UpdateRoad()
        {
            BoardController board = LevelEditorManager.Instance.Board;
            
            if (_currentRoadPositions == null 
                || _currentRoadPositions.Count == 0)
            {
                return;
            }

            SetLineRendererForWorldRoad(_currentRoadPositions, _roadLineRenderer);
        }

        public void SetLineRendererForWorldRoad(List<Vector3Int> roadPositions, LineRenderer lineRenderer)
        {
            List<Vector3> positions = GetRoadInWorldForPositions(roadPositions);
            lineRenderer.positionCount = positions.Count;

            Dictionary<Vector3, int> numberOfOccurence = new Dictionary<Vector3, int>();
            for (int i = 0; i < positions.Count; i++)
            {
                if (numberOfOccurence.TryAdd(positions[i], 1) == false)
                {
                    numberOfOccurence[positions[i]]++;
                }

                lineRenderer.SetPosition(i,
                    positions[i] + new Vector3(0, 0.1f + (0.2f * (numberOfOccurence[positions[i]] - 1)), 0));
            }
        }

        private List<Vector3> GetRoadInWorldForPositions(List<Vector3Int> roadPositions)
        {
            BoardController board = LevelEditorManager.Instance.Board;
            
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < roadPositions.Count; i++)
            {
                if (i > 0)
                {
                    positions.RemoveAt(positions.Count - 1);
                }

                positions.Add(board.GetCoordinatesToWorldPosition(roadPositions[i]));
                if (i + 1 < roadPositions.Count)
                {
                    SlotController startSlot = board.GetSlotFromCoordinates(roadPositions[i]);
                    SlotController endSlot = board.GetSlotFromCoordinates(roadPositions[i + 1]);
                    List<SlotController> pathSlots = board.GetPathFromSlotToSlot(startSlot, endSlot);

                    pathSlots.ForEach(x => positions.Add(board.GetCoordinatesToWorldPosition(x.Coordinates)));
                }
            }

            SlotController lastRoadPositionSlot = board.GetSlotFromCoordinates(roadPositions[^1]);
            
            bool isCurrentHoveredSlotUsable = board.CurrentHoveredLocation != null
                                              && board.CurrentHoveredLocation.CanEntityBePlacedHere(BoardEntitySuperType.Character)
                                              && board.CurrentHoveredLocation.SlotView.Controller.Data.Obstacle.Has == false;
            
            //if there is a current hovered slot and if it's usable, show the path to it 
            if (isCurrentHoveredSlotUsable && CurrentCharacter != null) 
            {
                List<SlotController> currentHoveredPathSlots =
                    board.GetPathFromSlotToSlot(lastRoadPositionSlot, board.CurrentHoveredLocation.SlotView.Controller);
                currentHoveredPathSlots.ForEach(x => positions.Add(board.GetCoordinatesToWorldPosition(x.Coordinates)));
            }

            if (_currentMode == RoadFollowMode.Loop && CurrentCharacter != null)
            {
                SlotController loopStartSlot = board.GetSlotFromCoordinates(isCurrentHoveredSlotUsable ? board.CurrentHoveredLocation.Coordinates : roadPositions[^1]);
                SlotController loopEndSlot = CurrentCharacter.Slot;
                List<Vector3> loopPath = board.GetPathFromSlotToSlot(loopStartSlot, loopEndSlot).ConvertAll(x => board.GetCoordinatesToWorldPosition(x.Coordinates));
                
                positions.AddRange(loopPath);
            }

            return positions;
        }

        public void SaveRoad()
        {
            if (_currentRoadPositions.Count <= 1)
            {
                return;
            }
            
            List<Vector3Int> roadToSave = new List<Vector3Int>(_currentRoadPositions){};
            if (_currentMode == RoadFollowMode.Loop)
            {
                roadToSave.Add(CurrentCharacter.Coordinates);
            }
            CurrentCharacter.SetRoad(roadToSave.ToArray());
            
            LevelEditorManager.Instance.UI.SetMode(EditorMode.BasicEditor);
        }

        public void ChangeRoadMode(Button buttonMode)
        {
            RoadFollowMode newMode = _currentMode switch
            {
                RoadFollowMode.PingPong => RoadFollowMode.Loop,
                RoadFollowMode.Loop => RoadFollowMode.PingPong,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _currentMode = newMode;
            
            buttonMode.GetComponentInChildren<TMP_Text>().text = $"change mode :\n{newMode.ToString()}";

            buttonMode.interactable = false;
            buttonMode.interactable = true;
        }
    }
}