using System;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class PilotStation : Station {
        public override void Activate() {
            PilotSeatInputListener.SetActive(true);
        }
        
        public override void Deactivate() {
            PilotSeatInputListener.SetActive(false);
        }

        private void Awake() {
            PilotSeatInputListener.OnExit += Exit;
        }

        private void OnDestroy() {
            PilotSeatInputListener.OnExit -= Exit;
        }

        private void Exit() {
            PlayerCharacter.ownedCharacter.DePossessStation();
        }
    }
}