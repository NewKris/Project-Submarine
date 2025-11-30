using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Lobby {
    public class LobbyController : NetworkBehaviourExtended {
        public Transform clientList;
        public GameObject listItemPrefab;
        public ConnectionManager connectionManager;

        public void ExitLobby() {
            if (IsServer) {
                connectionManager.ShutDownServer();
            }
            else {
                connectionManager.LeaveServer();
            }
        }

        public void StartExpedition() {
            if (IsServer) {
                NetworkManager.SceneManager.LoadScene("Expedition", LoadSceneMode.Single);
            }
        }
        
        private void Start() {
            connectionManager.OnClientConnected += ClientConnected;
            connectionManager.OnClientDisconnected += ClientDisconnected;
            DoOnAll(DrawConnectedClients);
        }

        private void OnDisable() {
            connectionManager.OnClientConnected -= ClientConnected;
            connectionManager.OnClientDisconnected -= ClientDisconnected;
        }

        private void ClientDisconnected(ulong clientId) {
            RefreshClientListRpc();
        }
        
        private void ClientConnected(ulong clientId) {
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