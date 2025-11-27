using System;
using Unity.Netcode;

namespace WereHorse.Runtime.Common {
    public class NetworkBehaviourExtended : NetworkBehaviour {
        protected void DoOnAll(Action callback) {
            callback();
        }

        protected void DoOnOwner(Action callback) {
            if (IsOwner) {
                callback();
            }
        }

        protected void DoOnServer(Action callback) {
            if (IsServer || IsHost) {
                callback();
            }
        }

        protected void DoOnClient(Action callback) {
            if (!IsServer && !IsHost) {
                callback();
            }
        }
    }
}