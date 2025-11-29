using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Gameplay {
    public class ClientManager : NetworkBehaviourExtended {
        private static ClientManager Instance;

        public static void DisconnectFromGame() {
            if (Instance.IsHost) {
                Instance.ShutDownServer();
            }
            else {
                Instance.DisconnectSelf();
            }
        }
        
        private void Start() {
            Instance = this;

            NetworkManager.OnClientConnectedCallback += LogConnection;
            NetworkManager.OnClientDisconnectCallback += SendClientHome;
        }

        private void OnDisable() {
            NetworkManager.OnClientConnectedCallback -= LogConnection;
            NetworkManager.OnClientDisconnectCallback -= SendClientHome;
        }

        private void LogConnection(ulong clientId) {
            if (IsServer) {
                Debug.Log($"Client {clientId} connected to server");
            }
            else {
                Debug.Log($"Connected to server as client {clientId}");
            }
        }
        
        private void SendClientHome(ulong clientId) {
            if (IsServer) {
                Debug.Log($"Client {clientId} disconnected");
            }
            
            if (NetworkManager.Singleton.LocalClientId == clientId) {
                Debug.Log("Disconnected from server, returning to Main Menu");
                SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
            }
        }
        
        private void DisconnectSelf() {
            DisconnectClientRpc(Instance.NetworkManager.LocalClientId, "Manual client disconnected");
        }

        private void ShutDownServer() {
            DisconnectAllRpc();
            NetworkManager.Shutdown();
        }
        
        [Rpc(SendTo.NotServer)]
        private void DisconnectAllRpc() {
            DisconnectClientRpc(NetworkManager.LocalClientId, "Server shutting down");
        }

        [Rpc(SendTo.Server)]
        private void DisconnectClientRpc(ulong clientId, string reason) {
            Debug.Log($"Disconnecting client {clientId} | Reason: {reason}");
            NetworkManager.DisconnectClient(clientId, reason);
        }
    }
}