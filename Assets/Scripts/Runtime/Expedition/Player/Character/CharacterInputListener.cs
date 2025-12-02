using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class CharacterInputListener : MonoBehaviour {
        public static event Action OnInteract;
        public static event Action OnJump;

        private InputAction _liftAction;
        private InputAction _moveAction;
        private InputAction _lookAction;

        public static float Lift { get; private set; }
        public static Vector2 Move { get; private set; }
        public static Vector2 Look { get; private set; }
        
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
            
            _liftAction = ActionMap["Lift"];
            _moveAction = ActionMap["Move"];
            _lookAction = ActionMap["Look"];
            
            ActionMap.Enable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }

        private void Update() {
            Lift = _liftAction.ReadValue<float>();
            Move = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
        }
    }
}