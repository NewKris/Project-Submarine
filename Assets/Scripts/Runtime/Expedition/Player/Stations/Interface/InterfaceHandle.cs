using UnityEngine;
using UnityEngine.EventSystems;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public class InterfaceHandle : MonoBehaviour {
        public InterfaceControl interfaceControl;

        public void Grab() {
            interfaceControl.OnHandleStart();
        }

        public void Release() {
            interfaceControl.OnHandleStop();
        }
    }
}
