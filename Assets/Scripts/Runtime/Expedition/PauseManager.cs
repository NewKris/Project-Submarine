using System;
using UnityEngine;

namespace WereHorse.Runtime.Expedition {
    public class PauseManager : MonoBehaviour {
        public static event Action<bool> OnPauseStateChanged;

        public GameObject pauseScreen;
        
        private bool _isPaused;

        public void UnPause() {
            _isPaused = false;
            pauseScreen.SetActive(false);
            OnPauseStateChanged?.Invoke(_isPaused);
        }
        
        private void Awake() {
            GeneralInputListener.OnTogglePause += TogglePause;
        }

        private void OnDestroy() {
            GeneralInputListener.OnTogglePause -= TogglePause;
        }

        private void TogglePause() {
            _isPaused = !_isPaused;
            pauseScreen.SetActive(_isPaused);
            OnPauseStateChanged?.Invoke(_isPaused);
        }
    }
}