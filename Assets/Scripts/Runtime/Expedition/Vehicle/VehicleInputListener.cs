using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Vehicle {
    public class VehicleInputListener : MonoBehaviour {
        public static Vector2 Move { get; private set; }
        public static float Rotate { get; private set; }

        private InputAction _moveAction;
        private InputAction _rotateAction;
        
        private static InputActionMap ActionMap => InputSystem.actions.actionMaps[2];

        private void Awake() {
            _moveAction = ActionMap["Move"];
            _rotateAction = ActionMap["Rotate"];
            
            ActionMap.Enable();
        }

        private void Update() {
            Move = _moveAction.ReadValue<Vector2>();
            Rotate = _rotateAction.ReadValue<float>();
        }
    }
}