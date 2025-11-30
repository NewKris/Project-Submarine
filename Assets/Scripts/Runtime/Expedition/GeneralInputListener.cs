using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WereHorse.Runtime.Expedition {
    public class GeneralInputListener : MonoBehaviour {
        public static event Action OnTogglePause;
        
        private static InputActionMap ActionMap => InputSystem.actions.actionMaps[0];

        private void Awake() {
            ActionMap["Pause"].performed += _ => OnTogglePause?.Invoke();
            ActionMap.Enable();
        }

        private void OnDestroy() {
            ActionMap.Dispose();
        }
    }
}