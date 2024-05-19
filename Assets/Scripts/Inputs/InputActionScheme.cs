//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Scripts/Inputs/InputActionScheme.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActionScheme: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActionScheme()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActionScheme"",
    ""maps"": [
        {
            ""name"": ""LevelEditor"",
            ""id"": ""f82a47c9-45b1-43b1-9c85-f00b0a3cbec3"",
            ""actions"": [
                {
                    ""name"": ""CameraMoveButton"",
                    ""type"": ""Button"",
                    ""id"": ""226f4253-6fd1-4209-9873-9b6e17f40824"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraMoveVector"",
                    ""type"": ""Value"",
                    ""id"": ""ffd16df6-1737-467f-b53b-ed255313529e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""32a04d80-0da7-4a7c-ad44-a393009d45ba"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ClickTap"",
                    ""type"": ""Button"",
                    ""id"": ""4255ad11-24fd-4333-82f1-7975d4a42ef2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""c72197de-8537-47ba-98e4-b6bfa5431eca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ClickHold"",
                    ""type"": ""Button"",
                    ""id"": ""f379f635-ae61-4967-bc84-9c98f176595d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2,pressPoint=0.1)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClickHold"",
                    ""type"": ""Button"",
                    ""id"": ""64500449-ab2e-4f50-90ca-3a5fdfdcf636"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2,pressPoint=0.1)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ctrl"",
                    ""type"": ""Button"",
                    ""id"": ""63a6b683-5c40-401d-9bed-9253b23b5aca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""C"",
                    ""type"": ""Button"",
                    ""id"": ""46225cf1-d7b5-4b83-a7e7-5fbc327c240d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""V"",
                    ""type"": ""Button"",
                    ""id"": ""1c04a05f-8a7c-4f89-b195-025adfbb9a94"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""X"",
                    ""type"": ""Button"",
                    ""id"": ""d44d5433-769a-4ed5-8421-d49d88d7d53f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Button"",
                    ""id"": ""6cf3b5d6-ee0e-46a2-845a-12c9ad3b9cbe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""667973c7-f62c-4ad5-9ced-989747c65ff8"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMoveButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5588e9f8-d0a9-4f7c-8749-09445d02479a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMoveVector"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3abd31c-fc45-478d-acf5-f26ebe8266d8"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19432855-63c9-4512-8c45-e62a27ba6d71"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClickTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e668c4b8-3de6-45a5-8228-9e9e9083c35e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ClickHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3279e585-4a6c-4f51-9c65-0fe1d16b2a97"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c6c2eb8-ce1c-4d72-86e2-c8e5410bcd98"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2bc3ffef-084f-4327-9f30-62eb6a8424a8"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""C"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a3e7deb-bb74-40a0-a40a-5e037e14f4f5"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""V"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8115d1f4-45b1-4c9d-9f42-b65758d5d7e1"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53bc9245-1a3c-43a1-a2ed-f880ad301951"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClickHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""163b5ebe-b267-4d4e-b9d6-470840ae6478"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // LevelEditor
        m_LevelEditor = asset.FindActionMap("LevelEditor", throwIfNotFound: true);
        m_LevelEditor_CameraMoveButton = m_LevelEditor.FindAction("CameraMoveButton", throwIfNotFound: true);
        m_LevelEditor_CameraMoveVector = m_LevelEditor.FindAction("CameraMoveVector", throwIfNotFound: true);
        m_LevelEditor_CameraZoom = m_LevelEditor.FindAction("CameraZoom", throwIfNotFound: true);
        m_LevelEditor_ClickTap = m_LevelEditor.FindAction("ClickTap", throwIfNotFound: true);
        m_LevelEditor_RightClick = m_LevelEditor.FindAction("RightClick", throwIfNotFound: true);
        m_LevelEditor_ClickHold = m_LevelEditor.FindAction("ClickHold", throwIfNotFound: true);
        m_LevelEditor_RightClickHold = m_LevelEditor.FindAction("RightClickHold", throwIfNotFound: true);
        m_LevelEditor_Ctrl = m_LevelEditor.FindAction("Ctrl", throwIfNotFound: true);
        m_LevelEditor_C = m_LevelEditor.FindAction("C", throwIfNotFound: true);
        m_LevelEditor_V = m_LevelEditor.FindAction("V", throwIfNotFound: true);
        m_LevelEditor_X = m_LevelEditor.FindAction("X", throwIfNotFound: true);
        m_LevelEditor_Rotation = m_LevelEditor.FindAction("Rotation", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // LevelEditor
    private readonly InputActionMap m_LevelEditor;
    private List<ILevelEditorActions> m_LevelEditorActionsCallbackInterfaces = new List<ILevelEditorActions>();
    private readonly InputAction m_LevelEditor_CameraMoveButton;
    private readonly InputAction m_LevelEditor_CameraMoveVector;
    private readonly InputAction m_LevelEditor_CameraZoom;
    private readonly InputAction m_LevelEditor_ClickTap;
    private readonly InputAction m_LevelEditor_RightClick;
    private readonly InputAction m_LevelEditor_ClickHold;
    private readonly InputAction m_LevelEditor_RightClickHold;
    private readonly InputAction m_LevelEditor_Ctrl;
    private readonly InputAction m_LevelEditor_C;
    private readonly InputAction m_LevelEditor_V;
    private readonly InputAction m_LevelEditor_X;
    private readonly InputAction m_LevelEditor_Rotation;
    public struct LevelEditorActions
    {
        private @InputActionScheme m_Wrapper;
        public LevelEditorActions(@InputActionScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraMoveButton => m_Wrapper.m_LevelEditor_CameraMoveButton;
        public InputAction @CameraMoveVector => m_Wrapper.m_LevelEditor_CameraMoveVector;
        public InputAction @CameraZoom => m_Wrapper.m_LevelEditor_CameraZoom;
        public InputAction LeftClick => m_Wrapper.m_LevelEditor_ClickTap;
        public InputAction @RightClick => m_Wrapper.m_LevelEditor_RightClick;
        public InputAction @ClickHold => m_Wrapper.m_LevelEditor_ClickHold;
        public InputAction @RightClickHold => m_Wrapper.m_LevelEditor_RightClickHold;
        public InputAction @Ctrl => m_Wrapper.m_LevelEditor_Ctrl;
        public InputAction @C => m_Wrapper.m_LevelEditor_C;
        public InputAction @V => m_Wrapper.m_LevelEditor_V;
        public InputAction @X => m_Wrapper.m_LevelEditor_X;
        public InputAction @Rotation => m_Wrapper.m_LevelEditor_Rotation;
        public InputActionMap Get() { return m_Wrapper.m_LevelEditor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LevelEditorActions set) { return set.Get(); }
        public void AddCallbacks(ILevelEditorActions instance)
        {
            if (instance == null || m_Wrapper.m_LevelEditorActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_LevelEditorActionsCallbackInterfaces.Add(instance);
            @CameraMoveButton.started += instance.OnCameraMoveButton;
            @CameraMoveButton.performed += instance.OnCameraMoveButton;
            @CameraMoveButton.canceled += instance.OnCameraMoveButton;
            @CameraMoveVector.started += instance.OnCameraMoveVector;
            @CameraMoveVector.performed += instance.OnCameraMoveVector;
            @CameraMoveVector.canceled += instance.OnCameraMoveVector;
            @CameraZoom.started += instance.OnCameraZoom;
            @CameraZoom.performed += instance.OnCameraZoom;
            @CameraZoom.canceled += instance.OnCameraZoom;
            LeftClick.started += instance.OnClickTap;
            LeftClick.performed += instance.OnClickTap;
            LeftClick.canceled += instance.OnClickTap;
            @RightClick.started += instance.OnRightClick;
            @RightClick.performed += instance.OnRightClick;
            @RightClick.canceled += instance.OnRightClick;
            @ClickHold.started += instance.OnClickHold;
            @ClickHold.performed += instance.OnClickHold;
            @ClickHold.canceled += instance.OnClickHold;
            @RightClickHold.started += instance.OnRightClickHold;
            @RightClickHold.performed += instance.OnRightClickHold;
            @RightClickHold.canceled += instance.OnRightClickHold;
            @Ctrl.started += instance.OnCtrl;
            @Ctrl.performed += instance.OnCtrl;
            @Ctrl.canceled += instance.OnCtrl;
            @C.started += instance.OnC;
            @C.performed += instance.OnC;
            @C.canceled += instance.OnC;
            @V.started += instance.OnV;
            @V.performed += instance.OnV;
            @V.canceled += instance.OnV;
            @X.started += instance.OnX;
            @X.performed += instance.OnX;
            @X.canceled += instance.OnX;
            @Rotation.started += instance.OnRotation;
            @Rotation.performed += instance.OnRotation;
            @Rotation.canceled += instance.OnRotation;
        }

        private void UnregisterCallbacks(ILevelEditorActions instance)
        {
            @CameraMoveButton.started -= instance.OnCameraMoveButton;
            @CameraMoveButton.performed -= instance.OnCameraMoveButton;
            @CameraMoveButton.canceled -= instance.OnCameraMoveButton;
            @CameraMoveVector.started -= instance.OnCameraMoveVector;
            @CameraMoveVector.performed -= instance.OnCameraMoveVector;
            @CameraMoveVector.canceled -= instance.OnCameraMoveVector;
            @CameraZoom.started -= instance.OnCameraZoom;
            @CameraZoom.performed -= instance.OnCameraZoom;
            @CameraZoom.canceled -= instance.OnCameraZoom;
            LeftClick.started -= instance.OnClickTap;
            LeftClick.performed -= instance.OnClickTap;
            LeftClick.canceled -= instance.OnClickTap;
            @RightClick.started -= instance.OnRightClick;
            @RightClick.performed -= instance.OnRightClick;
            @RightClick.canceled -= instance.OnRightClick;
            @ClickHold.started -= instance.OnClickHold;
            @ClickHold.performed -= instance.OnClickHold;
            @ClickHold.canceled -= instance.OnClickHold;
            @RightClickHold.started -= instance.OnRightClickHold;
            @RightClickHold.performed -= instance.OnRightClickHold;
            @RightClickHold.canceled -= instance.OnRightClickHold;
            @Ctrl.started -= instance.OnCtrl;
            @Ctrl.performed -= instance.OnCtrl;
            @Ctrl.canceled -= instance.OnCtrl;
            @C.started -= instance.OnC;
            @C.performed -= instance.OnC;
            @C.canceled -= instance.OnC;
            @V.started -= instance.OnV;
            @V.performed -= instance.OnV;
            @V.canceled -= instance.OnV;
            @X.started -= instance.OnX;
            @X.performed -= instance.OnX;
            @X.canceled -= instance.OnX;
            @Rotation.started -= instance.OnRotation;
            @Rotation.performed -= instance.OnRotation;
            @Rotation.canceled -= instance.OnRotation;
        }

        public void RemoveCallbacks(ILevelEditorActions instance)
        {
            if (m_Wrapper.m_LevelEditorActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ILevelEditorActions instance)
        {
            foreach (var item in m_Wrapper.m_LevelEditorActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_LevelEditorActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public LevelEditorActions @LevelEditor => new LevelEditorActions(this);
    public interface ILevelEditorActions
    {
        void OnCameraMoveButton(InputAction.CallbackContext context);
        void OnCameraMoveVector(InputAction.CallbackContext context);
        void OnCameraZoom(InputAction.CallbackContext context);
        void OnClickTap(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnClickHold(InputAction.CallbackContext context);
        void OnRightClickHold(InputAction.CallbackContext context);
        void OnCtrl(InputAction.CallbackContext context);
        void OnC(InputAction.CallbackContext context);
        void OnV(InputAction.CallbackContext context);
        void OnX(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
    }
}
