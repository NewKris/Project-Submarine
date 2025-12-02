using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class StationInputListener : MonoBehaviour {
        public static event Action OnExit;
        
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
            
            ActionMap.Disable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }
    }
}