using System.Collections.Generic;
using GameEngine;
using Slots;
using UnityEngine;

namespace Board.Characters
{
    public abstract class BoardCharacterState
    {
        protected BoardCharacterController Controller { get; private set; }
        
        public bool CanPlay { get; protected set; } = true;
        
        public BoardCharacterState(BoardCharacterController controller)
        {
            Controller = controller;
        }
        
        public abstract void Start();
        public abstract void Play();
        public abstract void Quit();
        
        protected void MoveTowardPosition(Vector3Int position)
        {
            List<SlotController> pathToPosition = GameManager.Instance.Board.GetPath(Controller.Coordinates, position, PathFindingOption.IgnoreCharacters);
            List<SlotController> accessibleSlots = GameManager.Instance.Board.GetAccessibleSlotsBySlot(Controller.CurrentSlot, Controller.GameplayData.CurrentMovementPoints);
            pathToPosition.RemoveAll(x => accessibleSlots.Contains(x) == false);
            
            pathToPosition.Insert(0, Controller.CurrentSlot);
            WorldOrientation.Orientation controllerOrientation =
                    pathToPosition[^1].Coordinates == position ? 
                        WorldOrientation.GetDirection(pathToPosition[^2].Coordinates, pathToPosition[^1].Coordinates) :
                        WorldOrientation.GetDirection(pathToPosition[^1].Coordinates, position);
            pathToPosition.RemoveAt(0);
            
            Controller.OnCharacterAction.Invoke(CharacterAction.MoveTo, new object[] { pathToPosition, controllerOrientation});
        }
        
        protected void Rotate()
        {
            WorldOrientation.Orientation orientation = Controller.GameplayData.Orientation;
            orientation++;
            if ((int)orientation > 3)
            {
                orientation = 0;
            }
            Controller.GameplayData.Orientation = orientation;
            
            Controller.OnCharacterAction.Invoke(CharacterAction.Rotate, new object[]{orientation});
        }
    }
}