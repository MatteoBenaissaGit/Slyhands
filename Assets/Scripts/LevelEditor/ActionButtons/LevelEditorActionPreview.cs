using Slots;
using UnityEngine;

namespace LevelEditor.ActionButtons
{
    public class LevelEditorActionPreview : MonoBehaviour
    {
        private GameObject _gameObjectToPreview;
        private float _yOffset;

        public void SetGameObjectToPreview(GameObject gameObjectPrefabToPreview, float yOffset = 0f)
        {
            _gameObjectToPreview = Instantiate(gameObjectPrefabToPreview, transform);
            _yOffset = yOffset;
        }

        public void DestroyPreview(bool doShow)
        {
            Destroy(_gameObjectToPreview);
            _gameObjectToPreview = null;
        }

        public void UpdatePreview(SlotLocation locationToPlacePreviewOn)
        {
            if (locationToPlacePreviewOn == null)
            {
                return;
            }
            transform.position = locationToPlacePreviewOn.transform.position;
        }
    }
}