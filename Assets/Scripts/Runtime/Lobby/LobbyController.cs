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
        public ServerManager serverManager;

        public void ExitLobby() {
            if (IsServer) {
                serverManager.ShutDownServer();
            }
            else {
                serverManager.LeaveServer();
            }
        }

        public void StartExpedition() {
            if (IsServer) {
                NetworkManager.SceneManager.LoadScene("Expedition", LoadSceneMode.Single);
            }
        }
        
        private void Start() {
            serverManager.OnClientConnected += ClientConnected;
            serverManager.OnClientDisconnected += ClientDisconnected;
            DoOnAll(DrawConnectedClients);
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable() {
            serverManager.OnClientConnected -= ClientConnected;
            serverManager.OnClientDisconnected -= ClientDisconnected;
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