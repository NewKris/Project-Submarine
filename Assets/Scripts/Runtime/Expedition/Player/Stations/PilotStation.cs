using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Expedition.Submarine;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class PilotStation : Station {
        public SubmarineBody submarineBody;
        
        public override void Deactivate() {
            base.Deactivate();
            submarineBody.SendSteerValuesRpc(0, 0, 0);
        }

        private void Update() {
            submarineBody.SendSteerValuesRpc(
                StationInputListener.Forward, 
                StationInputListener.Right, 
                StationInputListener.Up
            );
        }
    }
}