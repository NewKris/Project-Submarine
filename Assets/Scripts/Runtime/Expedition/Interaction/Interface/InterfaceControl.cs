using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Interaction.Interface {
    public abstract class InterfaceControl : NetworkBehaviourExtended {
        public virtual bool LockPlayer() {
            return false;
        }
        
        public abstract void OnHandleStart();
        public abstract void OnHandleStop();
    }
}