using System;
using GameEngine;
using LevelEditor;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public class BoardCharacterView : MonoBehaviour
    {
        public BoardCharacterController Controller { get; private set; }
        
        public SlotLocation CurrentSlotLocation
        {
            get
            {
                BoardController board = GameManager.Instance == null ? LevelEditorManager.Instance.Board : GameManager.Instance.Board;
                SlotLocation slotLocation = board.Data.SlotLocations[Controller.Coordinates.x, Controller.Coordinates.y, Controller.Coordinates.z];
                if (slotLocation == null)
                {
                    throw new Exception("no slot location found for the current slot coordinates");
                }
                return slotLocation;
            }
        }

        public void Initialize(BoardCharacterController controller)
        {
            Controller = controller;
            Controller.OnCharacterAction += CharacterActionView;
            
            transform.rotation = Quaternion.Euler(0, ((int)Controller.GameplayData.Orientation) * 90, 0);
        }

        private void OnDestroy()
        {
            Controller.OnCharacterAction -= CharacterActionView;
        }

        private void CharacterActionView(CharacterAction action, Vector3Int targetCoordinates = new Vector3Int())
        {
            switch (action)
            {
                case CharacterAction.Idle:
                    break;
                case CharacterAction.MoveTo:
                    MoveTo(targetCoordinates);
                    break;
                case CharacterAction.GetHit:
                    break;
                case CharacterAction.Die:
                    break;
                case CharacterAction.IsSelected:
                    CurrentSlotLocation.SetSelected(true);
                    break;
                case CharacterAction.IsUnselected:
                    CurrentSlotLocation.SetSelected(false);
                    break;
            }
        }

        private void MoveTo(Vector3Int targetCoordinates)
        {
            transform.position = Controller.Board.GetCoordinatesToWorldPosition(targetCoordinates);
        }
    }
}