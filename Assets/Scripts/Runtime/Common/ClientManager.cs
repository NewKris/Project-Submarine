using Unity.Netcode;
using UnityEngine;

namespace WereHorse.Runtime.Common {
    public class ClientManager : NetworkBehaviourExtended {
        public void DisconnectSelf(string reason) {
            if (NetworkManager.IsConnectedClient) {
                Debug.Log("[-] Disconnecting from server");
                DisconnectClientRpc(NetworkManager.LocalClientId, reason);
            }
        }

        [Rpc(SendTo.Server)]
        private void DisconnectClientRpc(ulong clientId, string reason) {
            NetworkManager.DisconnectClient(clientId, reason);
        }
    }
}