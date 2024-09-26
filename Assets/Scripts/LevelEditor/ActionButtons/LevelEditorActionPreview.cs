using System;
using System.Linq;
using Board;
using LevelEditor.Entities;
using Slots;
using UnityEngine;

namespace LevelEditor.ActionButtons
{
    /// <summary>
    /// This class handle the transparent preview of the object that will be placed in the level editor
    /// </summary>
    public class LevelEditorActionPreview : MonoBehaviour
    {
        [SerializeField] private Material _previewMaterial;
    
        private GameObject _gameObjectToPreview;
        private BoardEntitySuperType _objectToPreviewSuperType;
        private float _orientationAngle;

        private LevelEditorCharacter _levelEditorCharacterPreviewing;

        /// <summary>
        /// Set the game object to preview
        /// </summary>
        /// <param name="gameObjectPrefabToPreview">the object to preview</param>
        /// <param name="yOffset">the yOffset to apply on the preview</param>
        public void SetGameObjectToPreview(GameObject gameObjectPrefabToPreview, BoardEntitySuperType superType, float yOffset = 0f, WorldOrientation.Orientation orientation = WorldOrientation.Orientation.North)
        {
            DestroyPreview();
            if (gameObjectPrefabToPreview == null)
            {
                return;
            }
            
            _gameObjectToPreview = Instantiate(gameObjectPrefabToPreview, transform);
            _gameObjectToPreview.GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.material = _previewMaterial);
            _gameObjectToPreview.transform.position += new Vector3(0, yOffset, 0);
            SetOrientation(orientation);

            _objectToPreviewSuperType = superType;
            
            _gameObjectToPreview.SetActive(false);
        }

        /// <summary>
        /// Destroy the current preview
        /// </summary>
        public void DestroyPreview()
        {
            if (_gameObjectToPreview == null)
            {
                return;
            }
            Destroy(_gameObjectToPreview);
            _gameObjectToPreview = null;
        }

        /// <summary>
        /// Update the preview position
        /// </summary>
        /// <param name="locationToPlacePreviewOn">the slot location to put the preview on</param>
        public void UpdatePreview(SlotLocation locationToPlacePreviewOn)
        {
            if (locationToPlacePreviewOn != null 
                && locationToPlacePreviewOn.SlotView != null 
                && locationToPlacePreviewOn.SlotView.LevelEditorCharacterOnSlot != null
                && locationToPlacePreviewOn.SlotView.LevelEditorCharacterOnSlot != LevelEditorManager.Instance.RoadModeManager.CurrentCharacter)
            {
                _levelEditorCharacterPreviewing = locationToPlacePreviewOn.SlotView.LevelEditorCharacterOnSlot;
                _levelEditorCharacterPreviewing.ShowRoadPreview(true);
            }
            else if (_levelEditorCharacterPreviewing != null)
            {
                _levelEditorCharacterPreviewing?.ShowRoadPreview(false);
                _levelEditorCharacterPreviewing = null;
            }
            
            if (locationToPlacePreviewOn == null || _gameObjectToPreview == null)
            {
                if (_gameObjectToPreview != null)
                {
                    _gameObjectToPreview.SetActive(false);
                }
                return;
            }

            bool canBePlaced = locationToPlacePreviewOn.CanEntityBePlacedHere(_objectToPreviewSuperType);
            _gameObjectToPreview?.SetActive(locationToPlacePreviewOn != null && canBePlaced);
            transform.position = locationToPlacePreviewOn.transform.position;
        }

        /// <summary>
        /// Set the orientation of the preview
        /// </summary>
        /// <param name="orientation">the orientation to set the preview to</param>
        public void SetOrientation(WorldOrientation.Orientation orientation)
        {
            if (_gameObjectToPreview == null || _gameObjectToPreview.transform == null)
            {
                return;
            }
            _orientationAngle = (int)orientation;
            Vector3 localRotationEuler = _gameObjectToPreview.transform.localRotation.eulerAngles;
            _gameObjectToPreview.transform.localRotation = Quaternion.Euler(new Vector3(localRotationEuler.x,_orientationAngle * 90, localRotationEuler.z));
        }
    }
}