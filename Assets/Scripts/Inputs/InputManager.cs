using System;
using Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    /// <summary>
    /// This class handle the input scheme and create classes to handle each action scheme
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        public InputActionScheme Scheme { get; private set; }
        public InputLevelEditor LevelEditorInput { get; private set; }

        protected override void InternalAwake()
        {
            Scheme = new InputActionScheme();
            Scheme.Enable();

            LevelEditorInput = new InputLevelEditor(this);
        }
    }

    /// <summary>
    /// This class handle the inputs related to the level editor
    /// </summary>
    public class InputLevelEditor
    {
        public Action<bool> OnCameraMoveButtonPressed { get; set; }
        public Action<Vector2> OnCameraMoved { get; set; }
        public Action<float> OnCameraZoomed { get; set; }
        public Action OnClickTap { get; set; }
        public Action<bool> OnClickHold { get; set; }
        
        public InputLevelEditor(InputManager manager)
        {
            manager.Scheme.LevelEditor.CameraMoveButton.started += SetCameraMoveButton;
            manager.Scheme.LevelEditor.CameraMoveButton.canceled += SetCameraMoveButton;
            
            manager.Scheme.LevelEditor.CameraMoveVector.performed += SetCameraMoved;
            manager.Scheme.LevelEditor.CameraMoveVector.canceled += SetCameraMoved;
            
            manager.Scheme.LevelEditor.CameraZoom.performed += SetCameraZoomed;
            manager.Scheme.LevelEditor.CameraZoom.canceled += SetCameraZoomed;
            
            manager.Scheme.LevelEditor.ClickTap.started += SetClickTap;
            
            manager.Scheme.LevelEditor.ClickHold.performed += SetClickHold;
            manager.Scheme.LevelEditor.ClickHold.canceled += SetClickHold;
        }

        private void SetCameraMoveButton(InputAction.CallbackContext context)
        {
            OnCameraMoveButtonPressed?.Invoke(context.started);
        }

        private void SetCameraMoved(InputAction.CallbackContext context)
        {
            OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void SetCameraZoomed(InputAction.CallbackContext context)
        {
            OnCameraZoomed?.Invoke(context.ReadValue<float>());
        }
        
        private void SetClickTap(InputAction.CallbackContext context)
        {
            OnClickTap?.Invoke();
        }

        private void SetClickHold(InputAction.CallbackContext context)
        {
            OnClickHold?.Invoke(context.performed);
        }
    }
}