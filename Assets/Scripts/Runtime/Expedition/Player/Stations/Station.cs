using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class Station : MonoBehaviour {
        public Transform stationPivot;
        public float cameraDirection;
        [ReadOnly] public bool occupied;

        public void Activate() {
            StationInputListener.SetActive(true);
            StationInputListener.OnExit += Exit;
            PauseManager.OnPauseStateChanged += SetPauseState;
            Cursor.lockState = CursorLockMode.None;
            enabled = true;
            occupied = true;
        }

        public void Deactivate() {
            StationInputListener.SetActive(false);
            StationInputListener.OnExit -= Exit;
            PauseManager.OnPauseStateChanged -= SetPauseState;
            Cursor.lockState = CursorLockMode.Locked;
            enabled = false;
            occupied = false;
        }
        
        private void Awake() {
            enabled = false;
        }
        
        private void OnDestroy() {
            if (enabled) {
                StationInputListener.OnExit -= Exit;
                PauseManager.OnPauseStateChanged -= SetPauseState;
            }
        }

        private void OnDrawGizmos() {
            if (stationPivot) {
                Vector3 cameraDir = Quaternion.AngleAxis(cameraDirection, Vector3.right) * Vector3.forward;
                cameraDir = stationPivot.TransformDirection(cameraDir).normalized;
                
                HandlesProxy.DrawDisc(stationPivot.position, Vector3.up, 0.5f, true, Color.yellow);
                HandlesProxy.DrawRay(stationPivot.position + Vector3.up * 1.6f, cameraDir, 3, false, Color.red);
            }
        }
        
        private void SetPauseState(bool isPaused) {
            if (enabled) {
                StationInputListener.SetActive(!isPaused);
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
        
        private void Exit() {
            PlayerCharacter.ownedCharacter.DePossessStation();
        }
    }
}