using System;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Vehicle.Tank {
    public class NetworkVehicle : NetworkBehaviourExtended {
        public TankVehicle vehicle;

        public void SetDrive(float value) {
            if (IsServer) {
                vehicle.drive = value;
            }
        }

        public void SetTurn(float value) {
            if (IsServer) {
                vehicle.turn = value;
            }
        }

        public void SetStrafe(float value) {
            if (IsServer) {
                vehicle.strafe = value;
            }
        }
        
        private void Start() {
            DoOnClient(() => {
                vehicle.enabled = false;
            });
        }
    }
}