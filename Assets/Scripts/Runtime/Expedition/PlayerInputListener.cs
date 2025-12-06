using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition {
    public class PlayerInputListener : MonoBehaviour {
        public static event Action OnExit;
        public static event Action OnGrab;
        public static event Action OnRelease;
        public static event Action OnInteract;
        public static event Action OnJump;

        private InputAction _liftAction;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _mousePositionAction;

        public static float Lift { get; private set; }
        public static Vector2 Move { get; private set; }
        public static Vector2 Look { get; private set; }
        public static Vector2 MousePosition { get; private set; }
        
        private static InputActionMap ActionMap => InputSystem.actions.actionMaps[1];

        public static void SetActive(bool active) {
            if (active) {
                ActionMap.Enable();
            }
            else {
                ActionMap.Disable();
            }
        }
        
        private void Awake() {
            ActionMap["Interact"].performed += _ => OnInteract?.Invoke();
            ActionMap["Jump"].performed += _ => OnJump?.Invoke();
            ActionMap["Grab"].performed += _ => OnGrab?.Invoke();
            ActionMap["Grab"].canceled += _ => OnRelease?.Invoke();
            ActionMap["Exit Seat"].performed += _ => OnExit?.Invoke();
            
            _liftAction = ActionMap["Lift"];
            _moveAction = ActionMap["Move"];
            _lookAction = ActionMap["Look"];
            _mousePositionAction = ActionMap["Mouse Position"];
            
            ActionMap.Enable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            Lift = _liftAction.ReadValue<float>();
            Move = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
            MousePosition =  _mousePositionAction.ReadValue<Vector2>();
        }
    }
}