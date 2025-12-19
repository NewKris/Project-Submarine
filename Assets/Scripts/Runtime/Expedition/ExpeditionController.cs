using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Tasks;
using WereHorse.Runtime.Utility;

namespace WereHorse.Runtime.Expedition {
    public class ExpeditionController : NetworkBehaviourExtended {
        public GameObject playerCharacterPrefab;
        public Transform[] spawnPoints;
        public ServerManager serverManager;
        public ExpeditionTaskManager taskManager;

        public static NetworkObject GetPlayerCharacter(ulong playerId) {
            return NetworkManager.Singleton.SpawnManager.PlayerObjects
                .FirstOrDefault(x => x.OwnerClientId == playerId);
        }
        
        [Rpc(SendTo.Server)]
        public void ExtractRpc() {
            Debug.Log($"Total Points: {taskManager.TallyPoints()}");
            ReturnToLobby();
        }
        
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
            );
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

        private void OnDrawGizmos() {
            foreach (Transform spawnPoint in spawnPoints) {
                HandlesProxy.DrawDisc(spawnPoint.position, spawnPoint.up, 0.5f, false, Color.yellow);
                HandlesProxy.DrawRay(spawnPoint.position, spawnPoint.forward, 3, false, Color.red);
            }
        }
    }
}