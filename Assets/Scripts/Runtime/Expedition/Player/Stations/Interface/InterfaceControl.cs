using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Player.Stations.Interface {
    public abstract class InterfaceControl : NetworkBehaviourExtended {
        public void Activate() {
            enabled = true;
        }

        public void Deactivate() {
            enabled = false;
        }
        
        public abstract void OnHandleStart();
        public abstract void OnHandleStop();
    }
}