﻿using System;
using Common;
using LevelEditor;
using LevelEditor.ActionButtons;
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
        public InputCameraController CameraInput { get; private set; }

        public InputCardController CardControllerInput { get; private set; }

        protected override void InternalAwake()
        {
            Scheme = new InputActionScheme();
            Scheme.Enable();

            LevelEditorInput = new InputLevelEditor(this);
            CardControllerInput = new InputCardController(this);
            CameraInput = new InputCameraController(this);
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
            Cut = 2,
            Save = 3
        }

        public Action<bool> OnCameraMoveButtonPressed { get; set; }
        public Action<Vector2> OnCameraMoved { get; set; }
        public Action<float> OnCameraZoomed { get; set; }
        public Action<float> OnHeightShortcut { get; set; }
        public Action OnLeftClick { get; set; }
        public Action OnRightClick { get; set; }
        public Action<bool> OnClickHold { get; set; }
        public Action<bool> OnRightClickHold { get; set; }
        public Action<ControlShortcutAction> OnControlShortcut { get; set; }
        public Action<LevelEditorActionButtonType> OnActionShortcut { get; set; }
        public Action OnRotation { get; set; }
        public Action<int> OnCameraRotate { get; set; }
        public Action OnTabPressed { get; set; }
        public Action OnEnterPressed { get; set; }

        private bool _isControlPressed;

        public InputLevelEditor(InputManager manager)
        {
            manager.Scheme.LevelEditor.CameraMoveButton.started +=
                context => OnCameraMoveButtonPressed?.Invoke(context.started);
            manager.Scheme.LevelEditor.CameraMoveButton.canceled +=
                context => OnCameraMoveButtonPressed?.Invoke(context.started);

            manager.Scheme.LevelEditor.CameraMoveVector.performed +=
                context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());
            manager.Scheme.LevelEditor.CameraMoveVector.canceled +=
                context => OnCameraMoved?.Invoke(context.ReadValue<Vector2>());

            manager.Scheme.LevelEditor.MouseScroll.performed += context => ScrolledMouse(context.ReadValue<float>());
            manager.Scheme.LevelEditor.MouseScroll.canceled += context => ScrolledMouse(context.ReadValue<float>());

            manager.Scheme.LevelEditor.LeftClick.started += context => OnLeftClick?.Invoke();

            manager.Scheme.LevelEditor.RightClick.started += context => OnRightClick?.Invoke();

            manager.Scheme.LevelEditor.ClickHold.performed += context => OnClickHold?.Invoke(context.performed);
            manager.Scheme.LevelEditor.ClickHold.canceled += context => OnClickHold?.Invoke(context.performed);

            manager.Scheme.LevelEditor.RightClickHold.performed +=
                context => OnRightClickHold?.Invoke(context.performed);
            manager.Scheme.LevelEditor.RightClickHold.canceled +=
                context => OnRightClickHold?.Invoke(context.performed);

            manager.Scheme.LevelEditor.Ctrl.started += _ => _isControlPressed = true;
            manager.Scheme.LevelEditor.Ctrl.canceled += _ => _isControlPressed = false;
            manager.Scheme.LevelEditor.C.started += _ => PressedControlShortcut(ControlShortcutAction.Copy);
            manager.Scheme.LevelEditor.V.started += _ => PressedControlShortcut(ControlShortcutAction.Paste);
            manager.Scheme.LevelEditor.X.started += _ => PressedControlShortcut(ControlShortcutAction.Cut);
            manager.Scheme.LevelEditor.S.started += _ => PressedControlShortcut(ControlShortcutAction.Save);

            manager.Scheme.LevelEditor.Selection.started +=
                _ => PressedActionShortcut(LevelEditorActionButtonType.Selection);
            manager.Scheme.LevelEditor.Paint.started += _ => PressedActionShortcut(LevelEditorActionButtonType.Paint);
            manager.Scheme.LevelEditor.Obstacle.started +=
                _ => PressedActionShortcut(LevelEditorActionButtonType.AddObstacle);
            manager.Scheme.LevelEditor.Character.started +=
                _ => PressedActionShortcut(LevelEditorActionButtonType.AddCharacter);

            manager.Scheme.LevelEditor.Rotation.started += _ => OnRotation?.Invoke();

            manager.Scheme.LevelEditor.CameraRotationLeft.started += _ => OnCameraRotate?.Invoke(1);
            manager.Scheme.LevelEditor.CameraRotationRight.started += _ => OnCameraRotate?.Invoke(-1);

            manager.Scheme.LevelEditor.Tab.started += _ => OnTabPressed?.Invoke();
            manager.Scheme.LevelEditor.Enter.started += _ => OnEnterPressed?.Invoke();
        }

        private void PressedActionShortcut(LevelEditorActionButtonType action)
        {
            if (_isControlPressed)
            {
                return;
            }

            OnActionShortcut?.Invoke(action);
        }

        private void PressedControlShortcut(ControlShortcutAction shortcut)
        {
            if (_isControlPressed == false)
            {
                return;
            }

            OnControlShortcut?.Invoke(shortcut);
        }

        private void ScrolledMouse(float value)
        {
            if (_isControlPressed)
            {
                OnHeightShortcut?.Invoke(Math.Sign(value));
                return;
            }

            OnCameraZoomed?.Invoke(value);
        }
    }

    /// <summary>
    /// This class handle the inputs related to cards control
    /// </summary>
    public class InputCardController
    {
        public Action OnLeftClickDown { get; set; }
        public Action OnLeftClickUp { get; set; }
        public Action OnMouseMoved { get; set; }
        public Action OnMouseStopMoved { get; set; }

        public InputCardController(InputManager manager)
        {
            manager.Scheme.CardController.LeftClickPress.started += context => OnLeftClickDown?.Invoke();
            manager.Scheme.CardController.LeftClickPress.canceled += context => OnLeftClickUp?.Invoke();

            manager.Scheme.CardController.MouseMove.started += context => OnMouseMoved?.Invoke();
            manager.Scheme.CardController.MouseMove.canceled += context => OnMouseStopMoved?.Invoke();
        }
    }

    public class InputCameraController
    {
        public Action<Vector2Int> OnMouseEdgeScreen { get; set; }

        public float FirstBorderThickness;
        public float SecondaryBorderThickness;
        
        public InputCameraController(InputManager manager)
        {
            manager.Scheme.LevelEditor.CameraMoveVector.performed += CheckEdgeScreen;
        }

        private void CheckEdgeScreen(InputAction.CallbackContext context)
        {
            Vector2Int edgeVector = Vector2Int.zero;

            
            if (Input.mousePosition.y >= Screen.height - FirstBorderThickness)
            {
                edgeVector.y = -1;
            }
            else if (Input.mousePosition.y <= FirstBorderThickness)
            {
                edgeVector.y = 1;
            }

            if (Input.mousePosition.x >= Screen.width - FirstBorderThickness)
            {
                edgeVector.x = -1;
            }
            else if (Input.mousePosition.x <= FirstBorderThickness)
            {
                edgeVector.x = 1;
            }

            OnMouseEdgeScreen?.Invoke(edgeVector);
        }
    }
}