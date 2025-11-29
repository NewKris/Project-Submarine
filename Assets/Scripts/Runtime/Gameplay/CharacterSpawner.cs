using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using Werehorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Gameplay {
    public class CharacterSpawner : MonoBehaviour {
        public GameObject playerCharacterPrefab;
        public Transform spawnPoint;
        
        private Dictionary<ulong, PlayerCharacter> _spawnedCharacters;

        private bool IsServer => NetworkManager.Singleton && NetworkManager.Singleton.IsServer;
        
        private void Start() {
            if (IsServer) {
                Debug.Log("Initializing CharacterSpawner");
                _spawnedCharacters = new Dictionary<ulong, PlayerCharacter>(8);
                
                NetworkManager.Singleton.ConnectedClientsIds.ForEach(SpawnPlayerCharacter);
                
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerCharacter;
                NetworkManager.Singleton.OnClientDisconnectCallback += DespawnPlayerCharacter;
                NetworkManager.Singleton.OnServerStopped += DisposeCharacterSpawner;
            }
        }

        private void OnDestroy() {
            if (IsServer) {
                DisposeCharacterSpawner(false);
            }
        }

        private void DisposeCharacterSpawner(bool _) {
            Debug.Log("Disposing CharacterSpawner");
            
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerCharacter;
            NetworkManager.Singleton.OnClientDisconnectCallback -= DespawnPlayerCharacter;
            NetworkManager.Singleton.OnServerStopped -= DisposeCharacterSpawner;
        }
        
        private void PlaceCharacterOnSpawnPoint(PlayerCharacter playerCharacter) {
            playerCharacter.SetPositionAndRotationRpc(spawnPoint.position, spawnPoint.rotation);
        }
        
        private void SpawnPlayerCharacter(ulong clientId) {
            NetworkObject networkPrefab = NetworkManager.Singleton
                .GetNetworkPrefabOverride(playerCharacterPrefab)
                .GetComponent<NetworkObject>();

            PlayerCharacter character = NetworkManager.Singleton.SpawnManager
                .InstantiateAndSpawn(networkPrefab, clientId, true, true)
                .GetComponent<PlayerCharacter>();
                
            _spawnedCharacters.Add(clientId, character);
            
            PlaceCharacterOnSpawnPoint(character);
            
            Debug.Log($"Spawned character for client {clientId}");
        }

        private void DespawnPlayerCharacter(ulong clientId) {
            if (_spawnedCharacters.TryGetValue(clientId, out PlayerCharacter character)) {
                Destroy(character.gameObject);
                _spawnedCharacters.Remove(clientId);
                
                Debug.Log($"Despawned character for client {clientId}");
            }
        }
    }
}