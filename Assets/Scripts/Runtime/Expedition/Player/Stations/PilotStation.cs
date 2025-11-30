using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Expedition.Submarine;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class PilotStation : Station {
        public SubmarineBody submarineBody;
        
        public override void Activate() {
            PilotSeatInputListener.SetActive(true);
            enabled = true;
        }
        
        public override void Deactivate() {
            PilotSeatInputListener.SetActive(false);
            submarineBody.SendSteerValuesRpc(0, 0, 0);
            enabled = false;
        }

        private void Awake() {
            PilotSeatInputListener.OnExit += Exit;
            enabled = false;
        }

        private void OnDestroy() {
            PilotSeatInputListener.OnExit -= Exit;
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
    }
}