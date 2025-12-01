using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public abstract class Station : MonoBehaviour {
        public Transform stationPivot;
        [ReadOnly] public bool occupied;

        public virtual void Activate() {
            StationInputListener.SetActive(true);
            StationInputListener.OnExit += Exit;
            PauseManager.OnPauseStateChanged += SetPauseState;
            enabled = true;
            occupied = true;
        }

        public virtual void Deactivate() {
            StationInputListener.SetActive(false);
            StationInputListener.OnExit -= Exit;
            PauseManager.OnPauseStateChanged -= SetPauseState;
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
            HandlesProxy.DrawDisc(stationPivot.position, Vector3.up, 0.5f, true, Color.yellow);
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