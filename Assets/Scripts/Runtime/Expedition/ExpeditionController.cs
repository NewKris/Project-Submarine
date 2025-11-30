using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition {
    public class ExpeditionController : NetworkBehaviourExtended {
        public GameObject playerCharacterPrefab;
        public Transform spawnPoint;
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
            
            PlayerCharacter instance = NetworkManager.SpawnManager.InstantiateAndSpawn(
                networkPrefab: prefab, 
                ownerClientId: clientId, 
                destroyWithScene: true, 
                isPlayerObject: true
            ).GetComponent<PlayerCharacter>();
            
            instance.SetPositionAndRotationRpc(spawnPoint.position, spawnPoint.rotation);
        }
    }
}