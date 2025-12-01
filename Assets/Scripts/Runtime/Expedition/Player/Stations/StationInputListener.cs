using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class StationInputListener : MonoBehaviour {
        public static event Action OnExit;
        
        public static float Forward { get; private set; }
        public static float Right { get; private set; }
        public static float Up { get; private set; }

        private InputAction _thrustAction;
        private InputAction _yawAction;
        private InputAction _liftAction;
        
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

            _thrustAction = ActionMap["Forward"];
            _yawAction = ActionMap["Right"];
            _liftAction = ActionMap["Up"];
            
            ActionMap.Disable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            Forward = _thrustAction.ReadValue<float>();
            Right = _yawAction.ReadValue<float>();
            Up = _liftAction.ReadValue<float>();
        }
    }
}