using System;
using Common;
using LevelEditor;
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
        public enum ControlShortcutAction
        {
            Copy = 0,
            Paste = 1,
            Cut = 2
        }
        
        public Action<bool> OnCameraMoveButtonPressed { get; set; }
        public Action<Vector2> OnCameraMoved { get; set; }
        public Action<float> OnCameraZoomed { get; set; }
        public Action OnLeftClick { get; set; }
        public Action OnRightClick { get; set; }
        public Action<bool> OnClickHold { get; set; }
        public Action<bool> OnRightClickHold { get; set; }
        public Action<ControlShortcutAction> OnControlShortcut { get; set; }
        public Action OnRotation { get; set; }

        private bool _isControlPressed;
        
        public InputLevelEditor(InputManager manager)
        {
            manager.Scheme.LevelEditor.CameraMoveButton.started += context => OnCameraMoveButtonPressed?.Invoke(context.started);
            manager.Scheme.LevelEditor.CameraMoveButton.canceled += context => OnCameraMoveButtonPressed?.Invoke(context.started);
            
            manager.Scheme.LevelEditor.CameraMoveVector.performed += context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
            manager.Scheme.LevelEditor.CameraMoveVector.canceled += context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
            
            manager.Scheme.LevelEditor.CameraZoom.performed += context => OnCameraZoomed?.Invoke(context.ReadValue<float>());
            manager.Scheme.LevelEditor.CameraZoom.canceled += context => OnCameraZoomed?.Invoke(context.ReadValue<float>());
            
            manager.Scheme.LevelEditor.LeftClick.started += context => OnLeftClick?.Invoke();
            
            manager.Scheme.LevelEditor.RightClick.started += context => OnRightClick?.Invoke();
            
            manager.Scheme.LevelEditor.ClickHold.performed += context => OnClickHold?.Invoke(context.performed);
            manager.Scheme.LevelEditor.ClickHold.canceled += context => OnClickHold?.Invoke(context.performed);
            
            manager.Scheme.LevelEditor.RightClickHold.performed += context => OnRightClickHold?.Invoke(context.performed);
            manager.Scheme.LevelEditor.RightClickHold.canceled += context => OnRightClickHold?.Invoke(context.performed);

            manager.Scheme.LevelEditor.Ctrl.started += _ => _isControlPressed = true;
            manager.Scheme.LevelEditor.Ctrl.canceled += _ => _isControlPressed = false;
            manager.Scheme.LevelEditor.C.started += _ => PressedControlShortcut(ControlShortcutAction.Copy);
            manager.Scheme.LevelEditor.V.started += _ => PressedControlShortcut(ControlShortcutAction.Paste);
            manager.Scheme.LevelEditor.X.started += _ => PressedControlShortcut(ControlShortcutAction.Cut);
            
            manager.Scheme.LevelEditor.Rotation.started += _ => OnRotation?.Invoke();
        }

        private void PressedControlShortcut(ControlShortcutAction shortcut)
        {
            if (_isControlPressed == false)
            {
                return;
            }
            OnControlShortcut?.Invoke(shortcut);
        }
    }
}