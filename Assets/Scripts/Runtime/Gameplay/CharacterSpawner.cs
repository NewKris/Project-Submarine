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

            PlayerCharacter instance = NetworkManager.SpawnManager
                .InstantiateAndSpawn(networkPrefab, clientId, false, true)
                .GetComponent<PlayerCharacter>();

            instance.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            
            Debug.Log($"Spawned character for client {clientId}");
        }
    }
}