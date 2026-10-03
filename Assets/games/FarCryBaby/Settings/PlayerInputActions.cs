// GENERATED AUTOMATICALLY FROM 'Assets/Settings/PlayerInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
    public class @PlayerInputActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerControls"",
            ""id"": ""9cfd7604-d082-490e-9126-9fe77f9b68f5"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9bc8da39-2918-455c-886f-aff1cd3c1fb6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookMouse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a8b0e026-e6aa-4dcc-9342-6b8fa478f758"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookGamepad"",
                    ""type"": ""PassThrough"",
                    ""id"": ""222fba7e-b97f-4656-9cef-0edf922c3329"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""30f8bb9d-4649-4a6e-9c59-f7a96fa67687"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""fcddcc9b-e4aa-4707-b8f6-738cbe6a660a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""155813cf-49ef-4200-bb57-6bfc04f5a8fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""8f69c209-47bd-41cd-b97e-256a5196cad4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MapView"",
                    ""type"": ""Button"",
                    ""id"": ""5cf78b91-b6e0-42b6-8c2e-04f9177ec669"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""6b4efaec-7123-4318-9e33-debd052fb06e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8d64569b-0324-470c-b267-0c74e5328fa7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5bb21607-79b7-407d-a12e-c801023e9ff4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""28b92a98-885f-464e-bbb1-e90a28ac88b8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e9c7b88e-5309-49b1-9084-ed1bff5cb63d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftStick"",
                    ""id"": ""75207e4f-b40f-4bc8-b2ed-d17bcb1ecd91"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b8b39224-22e7-4d97-8cc2-1385207d6883"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d1110e44-eb12-44cc-9588-86d318cf00f9"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7241b837-7932-4cd1-9123-c97a7dc7c42e"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""55ac6227-c312-480e-b610-a4d7f3f6004e"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7ee9089e-fde0-452e-90e9-ace44e4bf3a8"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2"",
                    ""groups"": ""Mouse;Keyboard"",
                    ""action"": ""LookMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0e7d80f-4aa1-4a16-af34-fe233442a561"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""10ed115d-1baf-4af4-ab85-4419ce25a1bc"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91be836b-c628-4494-b9cf-2bb07d6e9f3b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse;Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6d66d16-57c9-4fbc-a76e-98993dbe1fcd"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c1be87e-2e0f-40c4-99e0-07c61d5f209d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse;Keyboard"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""267e5697-62d9-4414-8fd4-21a5b105c445"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bee1946f-4b89-40e1-87dd-11fe69d32c62"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2c9ca9b-df4a-4625-b316-ae099a486576"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""RightStick"",
                    ""id"": ""17a0de8f-c125-4351-a98e-44877950566d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookGamepad"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cbf843a1-588f-4653-b08d-bbab2d520fee"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""29aab088-1eda-4778-be9e-22c5949353da"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6c4371df-531c-4a59-908b-292ec4dc1bf0"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""bd8d9f92-e7b7-419b-af0a-88ee03287561"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LookGamepad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2a4e90c2-2c72-4f24-8a5b-809efad9be7c"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Mouse"",
                    ""action"": ""MapView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23da2ca2-abd2-4c80-9237-01c8db2f7023"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MapView"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mouse"",
            ""bindingGroup"": ""Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // PlayerControls
            m_PlayerControls = asset.FindActionMap("PlayerControls", throwIfNotFound: true);
            m_PlayerControls_Move = m_PlayerControls.FindAction("Move", throwIfNotFound: true);
            m_PlayerControls_LookMouse = m_PlayerControls.FindAction("LookMouse", throwIfNotFound: true);
            m_PlayerControls_LookGamepad = m_PlayerControls.FindAction("LookGamepad", throwIfNotFound: true);
            m_PlayerControls_Crouch = m_PlayerControls.FindAction("Crouch", throwIfNotFound: true);
            m_PlayerControls_Fire = m_PlayerControls.FindAction("Fire", throwIfNotFound: true);
            m_PlayerControls_Aim = m_PlayerControls.FindAction("Aim", throwIfNotFound: true);
            m_PlayerControls_Jump = m_PlayerControls.FindAction("Jump", throwIfNotFound: true);
            m_PlayerControls_MapView = m_PlayerControls.FindAction("MapView", throwIfNotFound: true);
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

        // PlayerControls
        private readonly InputActionMap m_PlayerControls;
        private IPlayerControlsActions m_PlayerControlsActionsCallbackInterface;
        private readonly InputAction m_PlayerControls_Move;
        private readonly InputAction m_PlayerControls_LookMouse;
        private readonly InputAction m_PlayerControls_LookGamepad;
        private readonly InputAction m_PlayerControls_Crouch;
        private readonly InputAction m_PlayerControls_Fire;
        private readonly InputAction m_PlayerControls_Aim;
        private readonly InputAction m_PlayerControls_Jump;
        private readonly InputAction m_PlayerControls_MapView;
        public struct PlayerControlsActions
        {
            private @PlayerInputActions m_Wrapper;
            public PlayerControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_PlayerControls_Move;
            public InputAction @LookMouse => m_Wrapper.m_PlayerControls_LookMouse;
            public InputAction @LookGamepad => m_Wrapper.m_PlayerControls_LookGamepad;
            public InputAction @Crouch => m_Wrapper.m_PlayerControls_Crouch;
            public InputAction @Fire => m_Wrapper.m_PlayerControls_Fire;
            public InputAction @Aim => m_Wrapper.m_PlayerControls_Aim;
            public InputAction @Jump => m_Wrapper.m_PlayerControls_Jump;
            public InputAction @MapView => m_Wrapper.m_PlayerControls_MapView;
            public InputActionMap Get() { return m_Wrapper.m_PlayerControls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerControlsActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerControlsActions instance)
            {
                if (m_Wrapper.m_PlayerControlsActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMove;
                    @LookMouse.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookMouse;
                    @LookMouse.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookMouse;
                    @LookMouse.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookMouse;
                    @LookGamepad.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookGamepad;
                    @LookGamepad.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookGamepad;
                    @LookGamepad.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnLookGamepad;
                    @Crouch.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnCrouch;
                    @Crouch.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnCrouch;
                    @Crouch.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnCrouch;
                    @Fire.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnFire;
                    @Aim.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAim;
                    @Aim.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAim;
                    @Aim.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnAim;
                    @Jump.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnJump;
                    @MapView.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMapView;
                    @MapView.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMapView;
                    @MapView.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnMapView;
                }
                m_Wrapper.m_PlayerControlsActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @LookMouse.started += instance.OnLookMouse;
                    @LookMouse.performed += instance.OnLookMouse;
                    @LookMouse.canceled += instance.OnLookMouse;
                    @LookGamepad.started += instance.OnLookGamepad;
                    @LookGamepad.performed += instance.OnLookGamepad;
                    @LookGamepad.canceled += instance.OnLookGamepad;
                    @Crouch.started += instance.OnCrouch;
                    @Crouch.performed += instance.OnCrouch;
                    @Crouch.canceled += instance.OnCrouch;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                    @Aim.started += instance.OnAim;
                    @Aim.performed += instance.OnAim;
                    @Aim.canceled += instance.OnAim;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @MapView.started += instance.OnMapView;
                    @MapView.performed += instance.OnMapView;
                    @MapView.canceled += instance.OnMapView;
                }
            }
        }
        public PlayerControlsActions @PlayerControls => new PlayerControlsActions(this);
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        private int m_MouseSchemeIndex = -1;
        public InputControlScheme MouseScheme
        {
            get
            {
                if (m_MouseSchemeIndex == -1) m_MouseSchemeIndex = asset.FindControlSchemeIndex("Mouse");
                return asset.controlSchemes[m_MouseSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IPlayerControlsActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLookMouse(InputAction.CallbackContext context);
            void OnLookGamepad(InputAction.CallbackContext context);
            void OnCrouch(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnAim(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnMapView(InputAction.CallbackContext context);
        }
    }
}
