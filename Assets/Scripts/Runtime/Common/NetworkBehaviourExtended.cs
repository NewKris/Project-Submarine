using System;
using Unity.Netcode;

namespace WereHorse.Runtime.Common {
    public class NetworkBehaviourExtended : NetworkBehaviour {
        protected bool NetworkInitialized => NetworkManager.Singleton != null;
        
        protected void DoOnNonOwners(Action callback) {
            if (NetworkInitialized && !IsOwner) {
                callback();
            }
        }
        
        protected void DoOnAll(Action callback) {
            if (NetworkInitialized) {
                callback();
            }
        }

        protected void DoOnOwner(Action callback) {
            if (NetworkInitialized && IsOwner) {
                callback();
            }
        }

        protected void DoOnServer(Action callback) {
            if (NetworkInitialized && (IsServer || IsHost)) {
                callback();
            }
        }

        protected void DoOnClient(Action callback) {
            if (NetworkInitialized && !IsServer && !IsHost) {
                callback();
            }
        }
    }
}