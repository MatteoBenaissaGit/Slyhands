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
        public Action OnRightClick { get; set; }
        public Action<bool> OnClickHold { get; set; }
        
        public InputLevelEditor(InputManager manager)
        {
            manager.Scheme.LevelEditor.CameraMoveButton.started += context => OnCameraMoveButtonPressed?.Invoke(context.started);
            manager.Scheme.LevelEditor.CameraMoveButton.canceled += context => OnCameraMoveButtonPressed?.Invoke(context.started);
            
            manager.Scheme.LevelEditor.CameraMoveVector.performed += context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
            manager.Scheme.LevelEditor.CameraMoveVector.canceled += context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
            
            manager.Scheme.LevelEditor.CameraZoom.performed += context => OnCameraZoomed?.Invoke(context.ReadValue<float>());
            manager.Scheme.LevelEditor.CameraZoom.canceled += context => OnCameraZoomed?.Invoke(context.ReadValue<float>());
            
            manager.Scheme.LevelEditor.ClickTap.started += context => OnClickTap?.Invoke();;
            
            manager.Scheme.LevelEditor.RightClick.started += context => OnRightClick?.Invoke();;
            
            manager.Scheme.LevelEditor.ClickHold.performed += context => OnClickHold?.Invoke(context.performed);
            manager.Scheme.LevelEditor.ClickHold.canceled += context => OnClickHold?.Invoke(context.performed);
            
            
        }
    }
}