using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using Werehorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Gameplay {
    public class CharacterSpawner : NetworkBehaviourExtended {
        public GameObject playerCharacterPrefab;
        public Transform spawnPoint;
        
        private void Start() {
            DoOnServer(() => {
                NetworkManager.ConnectedClientsIds.ForEach(SpawnPlayerCharacter);
                NetworkManager.OnClientConnectedCallback += SpawnPlayerCharacter;
            });
        }

        private void OnDisable() {
            DoOnServer(() => {
                NetworkManager.OnClientConnectedCallback -= SpawnPlayerCharacter;
            });
        }

        private void SpawnPlayerCharacter(ulong clientId) {
            NetworkObject networkPrefab = NetworkManager.GetNetworkPrefabOverride(playerCharacterPrefab)
                .GetComponent<NetworkObject>();

            NetworkObject instance = NetworkManager.SpawnManager
                .InstantiateAndSpawn(networkPrefab, clientId, true, true);
            
            instance.transform.position = spawnPoint.position;
            instance.transform.rotation = spawnPoint.rotation;
        }
    }
}