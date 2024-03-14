using System;
using DG.Tweening;
using Inputs;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// This class manage the camera used in the level editor
    /// </summary>
    public class LevelEditorCameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _zoomSpeed = 0.1f;
        [SerializeField] private float _cameraSizeMin = 0.5f;
        [SerializeField] private float _cameraSizeMax = 10f;
        [SerializeField] private float _moveSpeedMultiplierPerZoom = 2f;

        private bool _doMoveCamera;
        private Vector2 _cameraMovement;
        private float _cameraZoom;
        private float _baseSize;
        
        private void Start()
        {
            _baseSize = _camera.orthographicSize;
            
            InputManager.Instance.LevelEditorInput.OnCameraMoveButtonPressed += (bool doMove) => _doMoveCamera = doMove;
            InputManager.Instance.LevelEditorInput.OnCameraMoved += (Vector2 movementValue) => _cameraMovement = movementValue;
            InputManager.Instance.LevelEditorInput.OnCameraZoomed += (float zoomValue) => _cameraZoom = zoomValue;
        }

        private void Update()
        {
            _camera.DOKill();
            float size = _camera.orthographicSize - _cameraZoom * _zoomSpeed;
            size = Mathf.Clamp(size, _cameraSizeMin, _cameraSizeMax);
            _camera.DOOrthoSize(size, 0.1f).SetEase(Ease.Flash);
            
            if (_doMoveCamera == false)
            {
                return;
            }

            float zoomFactor = _camera.orthographicSize / _baseSize;
            float zoomFactorSpeed = zoomFactor * _moveSpeedMultiplierPerZoom;
            Vector3 xMovement = _camera.transform.right * (-_cameraMovement.x * _moveSpeed * zoomFactorSpeed);
            Vector3 yMovement = _camera.transform.up * (-_cameraMovement.y * _moveSpeed * zoomFactorSpeed);
            _camera.transform.localPosition += xMovement + yMovement;
        }
    }
}