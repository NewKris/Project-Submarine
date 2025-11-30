using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Expedition.Submarine;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class PilotStation : Station {
        public SubmarineBody submarineBody;
        
        public override void Activate() {
            PilotSeatInputListener.SetActive(true);
            enabled = true;
            occupied = true;
        }
        
        public override void Deactivate() {
            PilotSeatInputListener.SetActive(false);
            submarineBody.SendSteerValuesRpc(0, 0, 0);
            enabled = false;
            occupied = false;
        }

        private void Awake() {
            PilotSeatInputListener.OnExit += Exit;
            PauseManager.OnPauseStateChanged += SetPauseState;
            enabled = false;
        }

        private void OnDestroy() {
            PilotSeatInputListener.OnExit -= Exit;
            PauseManager.OnPauseStateChanged -= SetPauseState;
        }

        private void Update() {
            submarineBody.SendSteerValuesRpc(
                PilotSeatInputListener.Thrust, 
                PilotSeatInputListener.Yaw, 
                PilotSeatInputListener.Lift
            );
        }
        
        private void Exit() {
            PlayerCharacter.ownedCharacter.DePossessStation();
        }

        private void SetPauseState(bool isPaused) {
            if (enabled) {
                PilotSeatInputListener.SetActive(!isPaused);
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}