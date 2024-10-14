using System;
using Common;
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
        [field: SerializeField, Required] public UnityEngine.Camera Camera { get; private set; }

        public Action<Vector3, float> OnCameraRotated { get; set; }

        [SerializeField] private Transform _cameraParent;

        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _zoomSpeed = 0.1f;
        [SerializeField] private float _cameraSizeMin = 0.5f;
        [SerializeField] private float _cameraSizeMax = 10f;
        [SerializeField] private float _moveSpeedMultiplierPerZoom = 2f;
        [SerializeField] private float _edgeMovementSpeed = 2f;

        private bool _doMoveCamera;
        private Vector2 _cameraMovement;
        private float _cameraZoom;
        private float _baseSize;

        private UnityEngine.Camera _cardCamera;

        [SerializeField] private bool _isMouseOnBorder; 
        [SerializeField] private float FirstBorderThickness;
        [SerializeField] private float SecondaryBorderThickness;

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
            InputManager.Instance.LevelEditorInput.OnCameraRotate += RotateCamera;
            InputManager.Instance.CameraInput.OnMouseEdgeScreen += EdgeMovement;
            
            InputManager.Instance.CameraInput.FirstBorderThickness = FirstBorderThickness;
        }

        private void Update()
        {
            if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Camera.DOKill();
            float size = Camera.orthographicSize - _cameraZoom * _zoomSpeed;
            size = Mathf.Clamp(size, _cameraSizeMin, _cameraSizeMax);
            Camera.DOOrthoSize(size, 0.1f).SetEase(Ease.Flash);

            if (_doMoveCamera == false)
            {
                EdgeScrolling();
                return;
            }
            
            MoveCameraWithMovement(_cameraMovement);
        }

        private bool IsMouseOverGameWindow()
        {
            Vector3 mp = Input.mousePosition;
            return !(mp.x < 0) && !(mp.x > Screen.width) && !(mp.y < 0) && !(mp.y > Screen.height);
        }

        private void EdgeScrolling()
        {
            if (IsMouseOverGameWindow() == false)
                return;
            if (_edgeMovement == Vector2Int.zero)
                return;

            _isMouseOnBorder = true;
            
            var movement = new Vector2(_edgeMovement.x * _edgeMovementSpeed, _edgeMovement.y * _edgeMovementSpeed);
            MoveCameraWithMovement(movement);
        }

        private void MoveCameraWithMovement(Vector2 movement)
        {
            float zoomFactor = Camera.orthographicSize / _baseSize;
            float zoomFactorSpeed = zoomFactor * _moveSpeedMultiplierPerZoom;
            float deltaTime = Time.deltaTime * Screen.dpi;
            Vector3 xMovement = Camera.transform.right * (-movement.x * _moveSpeed * zoomFactorSpeed * deltaTime);
            Vector3 yMovement = Camera.transform.up * (-movement.y * _moveSpeed * zoomFactorSpeed * deltaTime);
            Camera.transform.localPosition += xMovement + yMovement;
        }

        private void RotateCamera(int direction)
        {
            Camera.transform.DOComplete();
            
            direction = Math.Sign(direction) * 90;

            Vector3 hitPoint = Vector3.zero;
            Plane plane = new Plane(Vector3.up, -1);
            Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit))
            {
                hitPoint = hit.point;
            }
            else if (plane.Raycast(ray, out float enter))
            {
                hitPoint = ray.GetPoint(enter);
            }

            Vector3 basePosition = Camera.transform.position;
            Quaternion baseRotation = Camera.transform.rotation;
            Camera.transform.RotateAround(hitPoint, Vector3.up, direction);
            Vector3 newPosition = Camera.transform.position;
            Quaternion newRotation = Camera.transform.rotation;
            Camera.transform.position = basePosition;
            Camera.transform.rotation = baseRotation;

            float duration = 0.3f;
            Camera.transform.DORotate(newRotation.eulerAngles, duration);
            Camera.transform.DOMove(newPosition, duration);

            Vector3 newForward = (hitPoint - newPosition).normalized;
            OnCameraRotated?.Invoke(newForward, duration);

            GizmoDrawer.DrawCross(hitPoint, new Color(1f, 0.59f, 0.14f));
        }

        private Vector2Int _edgeMovement;

        private void EdgeMovement(Vector2Int moveVector)
        {
            _edgeMovement = moveVector;
            
            
        }
    }
}