using System;
using DG.Tweening;
using Inputs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Camera
{
    /// <summary>
    /// This class manage the camera used in the level editor
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [field:SerializeField, Required] public UnityEngine.Camera Camera { get; private set; }
        
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _zoomSpeed = 0.1f;
        [SerializeField] private float _cameraSizeMin = 0.5f;
        [SerializeField] private float _cameraSizeMax = 10f;
        [SerializeField] private float _moveSpeedMultiplierPerZoom = 2f;

        private bool _doMoveCamera;
        private Vector2 _cameraMovement;
        private float _cameraZoom;
        private float _baseSize;

        private UnityEngine.Camera _cardCamera;
        
        private void Start()
        {
            _baseSize = Camera.orthographicSize;
            
            if (InputManager.Instance == null)
            {
                throw new Exception("no input manager");
            }
            InputManager.Instance.LevelEditorInput.OnCameraMoveButtonPressed += (bool doMove) => _doMoveCamera = doMove;
            InputManager.Instance.LevelEditorInput.OnCameraMoved += (Vector2 movementValue) => _cameraMovement = movementValue;
            InputManager.Instance.LevelEditorInput.OnCameraZoomed += (float zoomValue) => _cameraZoom = zoomValue;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            Camera.DOKill();
            float size = Camera.orthographicSize - _cameraZoom * _zoomSpeed;
            size = Mathf.Clamp(size, _cameraSizeMin, _cameraSizeMax);
            Camera.DOOrthoSize(size, 0.1f).SetEase(Ease.Flash);
            
            if (_doMoveCamera == false)
            {
                return;
            }

            float zoomFactor = Camera.orthographicSize / _baseSize;
            float zoomFactorSpeed = zoomFactor * _moveSpeedMultiplierPerZoom;
            float deltaTime = Time.deltaTime * Screen.dpi;
            Vector3 xMovement = Camera.transform.right * (-_cameraMovement.x * _moveSpeed * zoomFactorSpeed * deltaTime);
            Vector3 yMovement = Camera.transform.up * (-_cameraMovement.y * _moveSpeed * zoomFactorSpeed * deltaTime);
            Camera.transform.localPosition += xMovement + yMovement;
        }
    }
}