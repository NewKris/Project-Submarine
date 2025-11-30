using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class PilotSeatInputListener : MonoBehaviour {
        public static event Action OnExit;
        
        public static float Thrust { get; private set; }
        public static float Yaw { get; private set; }
        public static float Lift { get; private set; }

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

            _thrustAction = ActionMap["Thrust"];
            _yawAction = ActionMap["Yaw"];
            _liftAction = ActionMap["Lift"];
            
            ActionMap.Disable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            Thrust = _thrustAction.ReadValue<float>();
            Yaw = _yawAction.ReadValue<float>();
            Lift = _liftAction.ReadValue<float>();
        }
    }
}