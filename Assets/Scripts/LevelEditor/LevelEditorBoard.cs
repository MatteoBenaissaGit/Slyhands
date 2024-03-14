using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// This class manage the level editor board where the user can create the level
    /// </summary>
    public class LevelEditorBoard : MonoBehaviour
    {
        /// <summary>
        /// Get the board width (X) and length (Y)
        /// </summary>
        public Vector2Int GetBoardDimensions => new Vector2Int(_width, _length);
        
        [SerializeField, TabGroup("Dimensions"), Range(0, 256)] private int _width;
        [SerializeField, TabGroup("Dimensions"), Range(0, 256)] private int _length;

        [SerializeField, TabGroup("References"), Required] private LevelEditorSlotLocation _slotLocationPrefab;

        private Transform _slotParent;
        private LevelEditorSlotLocation[,] _slotLocations;

        private void Awake()
        {
            _slotParent = new GameObject("SlotParent").transform;
            _slotParent.parent = transform;

            _slotLocations = new LevelEditorSlotLocation[_width, _length];
        }

        /// <summary>
        /// Create a new blank board of the defined settings size
        /// </summary>
        public void CreateNewBoard()
        {
            ClearBoard();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _length; y++)
                {
                    _slotLocations[x, y] = Instantiate(_slotLocationPrefab, transform.position + new Vector3(x,0,y), Quaternion.identity);
                    _slotLocations[x, y].transform.parent = _slotParent;
                }
            }
        }

        /// <summary>
        /// Clear the board from all slots on it
        /// </summary>
        private void ClearBoard()
        {
            foreach (LevelEditorSlotLocation slot in _slotLocations)
            {
                if (slot == null || slot.gameObject == null)
                {
                    continue;
                }
                Destroy(slot.gameObject);
            }
            _slotLocations = new LevelEditorSlotLocation[_width, _length];
        }
    }
}
