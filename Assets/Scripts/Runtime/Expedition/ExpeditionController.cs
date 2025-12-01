using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition {
    public class ExpeditionController : NetworkBehaviourExtended {
        public GameObject playerCharacterPrefab;
        public Transform[] spawnPoints;
        public ServerManager serverManager;

        public void ReturnToLobby() {
            if (IsHost) {
                NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
        }

        public void ExitGame() {
            if (IsHost) {
                serverManager.ShutDownServer();
            }
            else {
                serverManager.LeaveServer();
            }
        }
        
        private void Start() {
            DoOnAll(() => {
                SpawnCharacterRpc(NetworkManager.LocalClientId);
            });
        }

        private void ShutDownServer() {
            
        }

        [Rpc(SendTo.Server)]
        private void SpawnCharacterRpc(ulong clientId) {
            NetworkObject prefab = NetworkManager.GetNetworkPrefabOverride(playerCharacterPrefab)
                .GetComponent<NetworkObject>();

            Transform spawn = spawnPoints[GetClientIndex(clientId)];
            
            NetworkManager.SpawnManager.InstantiateAndSpawn(
                networkPrefab: prefab, 
                ownerClientId: clientId, 
                destroyWithScene: true, 
                isPlayerObject: true,
                position: spawn.position,
                rotation: spawn.rotation
            ).GetComponentInChildren<PlayerCharacter>();
        }

        private int GetClientIndex(ulong clientId) {
            int index = 0;
            
            foreach (ulong id in NetworkManager.ConnectedClientsIds) {
                if (id == clientId) {
                    return index;
                }
                
                index++;
            }

            return 0;
        }
    }
}