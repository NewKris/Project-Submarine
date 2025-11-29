using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Gameplay.Player.Character {
    public class CharacterInputListener : MonoBehaviour {
        public static event Action OnJump;
        public static event Action OnCrouchStart;
        public static event Action OnCrouchEnd;
        public static event Action OnExitStation;
        public static event Action OnToggleMouse;
        public static event Action OnInteract;

        private InputAction _moveAction;
        private InputAction _lookAction;

        public static Vector2 Move { get; private set; }
        public static Vector2 Look { get; private set; }
        
        private InputActionMap ActionMap => InputSystem.actions.actionMaps[1];
        
        private void Awake() {
            ActionMap["Jump"].performed += _ => OnJump?.Invoke();
            ActionMap["Crouch"].started += _ => OnCrouchStart?.Invoke();
            ActionMap["Crouch"].canceled += _ => OnCrouchEnd?.Invoke();
            ActionMap["Toggle Mouse"].performed += _ => OnToggleMouse?.Invoke();
            ActionMap["Interact"].performed += _ => OnInteract?.Invoke();
            ActionMap["Exit Station"].performed += _ => OnExitStation?.Invoke();
            
            _moveAction = ActionMap["Move"];
            _lookAction = ActionMap["Look"];
            
            ActionMap.Enable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            Move = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
        }
    }
}