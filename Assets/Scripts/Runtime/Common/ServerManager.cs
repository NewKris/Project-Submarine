using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WereHorse.Runtime.Common {
    public class ServerManager : NetworkBehaviourExtended {
        public event Action<ulong> OnClientConnected;
        public event Action<ulong> OnClientDisconnected;
        
        public void ShutDownServer() {
            DisposeServerEvents();
            Debug.Log("=== Shutting down server... ===");
            NetworkManager.Singleton.Shutdown();
            ReturnToTitle();
        }
        
        public void LeaveServer() {
            DisconnectSelf($"[-] Client #{NetworkManager.LocalClientId} left the lobby");
        }

        private void Start() {
            DoOnServer(CreateServerEvents);
            DoOnClient(() => {
                Debug.Log($"[+] Connected to server as Client #{NetworkManager.LocalClientId}");
                CreateClientEvents();
            });
        }

        private void OnDisable() {
            DoOnServer(DisposeServerEvents);
            DoOnClient(DisposeClientEvents);
        }

        private void CreateServerEvents() {
            NetworkManager.OnClientConnectedCallback += ClientConnected;
            NetworkManager.OnClientDisconnectCallback += ClientDisconnected;
        }
        
        private void DisposeServerEvents() {
            NetworkManager.OnClientConnectedCallback -= ClientConnected;
            NetworkManager.OnClientDisconnectCallback -= ClientDisconnected;
        }
        
        private void CreateClientEvents() {
            NetworkManager.OnClientDisconnectCallback += EscapeToTitle;
        }

        private void DisposeClientEvents() {
            NetworkManager.OnClientDisconnectCallback -= EscapeToTitle;
        }

        private void DisconnectSelf(string reason) {
            if (NetworkManager.IsConnectedClient) {
                Debug.Log("=== Disconnecting from server ===");
                DisconnectClientRpc(NetworkManager.LocalClientId, reason);
            }
        }

        [Rpc(SendTo.Server)]
        private void DisconnectClientRpc(ulong clientId, string reason) {
            NetworkManager.DisconnectClient(clientId, reason);
        }
        
        private void EscapeToTitle(ulong _) {
            Debug.Log("[-] Disconnected from server");
            ReturnToTitle();
        }
        
        private void ReturnToTitle() {
            Debug.Log("=== Returning to title ===");
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }

        private void ClientConnected(ulong clientId) {
            Debug.Log($"[+] Client #{clientId} connected");
            OnClientConnected?.Invoke(clientId);
        }

        private void ClientDisconnected(ulong clientId) {
            Debug.Log($"[-] Client #{clientId} disconnected");
            OnClientDisconnected?.Invoke(clientId);
        }
    }
}