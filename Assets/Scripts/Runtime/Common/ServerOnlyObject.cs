using Unity.Netcode;
using UnityEngine;

namespace WereHorse.Runtime.Common {
    public class ServerOnlyObject : MonoBehaviour {
        private void Awake() {
            gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }
    }
}