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

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(300, 10, 100, 100));

            if (GUILayout.Button("Return to Lobby") && IsHost) {
            }
            
            if (GUILayout.Button("Return to Title")) {
            }
            
            GUILayout.EndArea();
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