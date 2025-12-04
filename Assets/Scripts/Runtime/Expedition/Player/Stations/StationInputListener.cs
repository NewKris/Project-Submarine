using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class StationInputListener : MonoBehaviour {
        public static event Action OnExit;
        public static event Action OnGrab;
        public static event Action OnRelease;

        private InputAction _mousePositionAction;
        private InputAction _deltaMouseAction;
        
        public static Vector2 MousePosition { get; private set; }
        public static Vector2 DeltaMouse { get; private set; }
        
        private static InputActionMap ActionMap => InputSystem.actions.actionMaps[2];

        public static void SetActive(bool active) {
            if (active) {
                ActionMap.Enable();
            }
            else {
                ActionMap.Disable();
            }
        }

        private void Awake() {
            ActionMap["Exit"].performed += _ => OnExit?.Invoke();
            ActionMap["Grab"].performed += _ => OnGrab?.Invoke();
            ActionMap["Grab"].canceled += _ => OnRelease?.Invoke();
            
            _mousePositionAction = ActionMap["Mouse Position"];
            _deltaMouseAction = ActionMap["Delta Mouse"];
            
            ActionMap.Disable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            MousePosition = _mousePositionAction.ReadValue<Vector2>();
            DeltaMouse = _deltaMouseAction.ReadValue<Vector2>();
        }
    }
}