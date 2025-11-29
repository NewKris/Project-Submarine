using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Common {
    public class NetworkPanel : MonoBehaviour {
        private static NetworkPanel Instance;

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (NetworkManager.Singleton) {
                if (IsConnected()) {
                    GUILayout.Label($"Playing as: {ConnectionType()}");
                }

                if (NetworkManager.Singleton.IsServer) {
                    GUILayout.Label("Connected Players:");

                    foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                        GUILayout.Label(clientId.ToString());
                    }
                }
            }
            
            GUILayout.EndArea();
        }

        private void Awake() {
            Singleton.SetSingleton(ref Instance, this);
        }

        private void OnDestroy() {
            Singleton.UnsetSingleton(ref Instance, this);
        }

        private bool IsConnected() {
            return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer;
        }
        
        private string ConnectionType() {
            return NetworkManager.Singleton.IsServer ? "Host" : "Client";
        }
    }
}