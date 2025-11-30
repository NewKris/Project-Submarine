using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Lobby {
    public class LobbyController : NetworkBehaviourExtended {
        public Transform clientList;
        public GameObject listItemPrefab;
        public ClientManager clientManager;

        public void ExitLobby() {
            if (IsServer) {
                ShutDownServer();
            }
            else {
                LeaveServer();
            }
        }

        public void StartExpedition() {
            if (IsServer) {
                NetworkManager.SceneManager.LoadScene("Expedition", LoadSceneMode.Single);
            }
        }
        
        private void Start() {
            DoOnServer(CreateServerEvents);
            DoOnClient(CreateClientEvents);
            DoOnAll(DrawConnectedClients);
        }

        private void OnDisable() {
            DoOnServer(DisposeServerEvents);
            DoOnClient(DisposeClientEvents);
        }

        private void EscapeLobby(ulong clientId) {
            ReturnToTitle();
        }
        
        private void LeaveServer() {
            clientManager.DisconnectSelf($"[-] Client #{NetworkManager.LocalClientId} left the lobby");
        }

        private void CreateClientEvents() {
            NetworkManager.OnClientDisconnectCallback += EscapeLobby;
        }

        private void DisposeClientEvents() {
            NetworkManager.OnClientDisconnectCallback -= EscapeLobby;
        }

        private void CreateServerEvents() {
            NetworkManager.OnClientConnectedCallback += ClientConnected;
            NetworkManager.OnClientDisconnectCallback += ClientDisconnected;
        }
        
        private void DisposeServerEvents() {
            NetworkManager.OnClientConnectedCallback -= ClientConnected;
            NetworkManager.OnClientDisconnectCallback -= ClientDisconnected;
        }
        
        private void ShutDownServer() {
            DisposeServerEvents();
            NetworkManager.Singleton.Shutdown();
            ReturnToTitle();
        }

        private void ReturnToTitle() {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        }
        
        private void ClientDisconnected(ulong clientId) {
            Debug.Log($"[-] Client #{clientId} disconnected");
            RefreshClientListRpc();
        }
        
        private void ClientConnected(ulong clientId) {
            Debug.Log($"[+] Client #{clientId} connected");
            RefreshClientListRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        private void RefreshClientListRpc() {
            clientList.DestroyChildren();
            DrawConnectedClients();
        }

        private void DrawConnectedClients() {
            foreach (ulong id in NetworkManager.ConnectedClientsIds) {
                LobbyClient client = Instantiate(listItemPrefab, clientList).GetComponent<LobbyClient>();
                client.clientName.text = id == 0 ? "Host" : $"Client #{id}";
            }
        }
    }
}