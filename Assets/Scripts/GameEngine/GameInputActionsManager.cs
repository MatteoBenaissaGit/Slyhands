using LevelEditor;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEngine
{
    public class GameInputActionsManager : MonoBehaviour
    {
        private SlotLocation _currentHoveredLocation;
        private RaycastHit[] _mouseClickRaycastHits = new RaycastHit[6];
        
        private void Update()
        {
            SlotLocation location = GetCurrentHoveredSlotLocation();
            if (location == null || location != _currentHoveredLocation)
            {
                GameManager.Instance.Board.CurrentHoveredLocation = location;
            }
        }
        
        private SlotLocation GetCurrentHoveredSlotLocation()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return null;
            }
            
            Ray ray = GameManager.Instance.Camera.Camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _mouseClickRaycastHits);
            for (int i = 0; i < hits; i++)
            {
                if (_mouseClickRaycastHits[i].collider.TryGetComponent(out SlotLocation location) == false
                    || location.IsUsable == false)
                {
                    continue;
                }
                return location;
            }

            return null;
        }
    }
}