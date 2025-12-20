using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Expedition.Inventory;
using WereHorse.Runtime.Utility.Attributes;
using Random = UnityEngine.Random;

namespace WereHorse.Runtime.Expedition.Tasks {
    public class ExpeditionTaskManager : MonoBehaviour {
        public GameObject[] possibleObjectives;
        public TaskSpawn[] spawnPoints;
        public int objectiveCount;
        public int maxDuplicateObjectives;
        public AreaOverlap itemCheckArea;

        private HashSet<GameObject> _objectives;
        
        public void SpawnObjectives() {
            _objectives = new HashSet<GameObject>();
            int[] spawnCount = new int[possibleObjectives.Length];
            HashSet<int> closedSpawns = new HashSet<int>();
            
            for (int i = 0; i < objectiveCount; i++) {
                GameObject objPrefab = GetRandomObjectiveIndex(spawnCount);
                Vector3 spawn = GetRandomSpawn(closedSpawns);
                
                NetworkObject instance = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                    networkPrefab: objPrefab.GetComponent<NetworkObject>(), 
                    position : spawn, 
                    rotation: Quaternion.identity
                );
                
                Debug.Log(spawn);
                instance.GetComponent<Rigidbody>().position = spawn;
                
                _objectives.Add(instance.gameObject);
            }
        }
        
        public int TallyPoints() {
            int itemsInSafeArea = 0;
            Collider[] overlaps = itemCheckArea.Evaluate();
            
            foreach (GameObject objective in _objectives) {
                Collider col = objective.GetComponent<Collider>();
                if (Array.Exists(overlaps, x => x == col)) {
                    itemsInSafeArea++;
                }
            }
            
            return itemsInSafeArea;
        }

        private GameObject GetRandomObjectiveIndex(int[] closedObjectives) {
            int objIndex;
            
            do {
                objIndex = Random.Range(0, possibleObjectives.Length);
            } while (closedObjectives[objIndex] > maxDuplicateObjectives);
                
            closedObjectives[objIndex]++;
            
            return possibleObjectives[objIndex];
        }

        private Vector3 GetRandomSpawn(HashSet<int> closedSpawns) {
            int spawnIndex;

            do {
                spawnIndex = Random.Range(0, spawnPoints.Length);
            } while(closedSpawns.Contains(spawnIndex));

            closedSpawns.Add(spawnIndex);
            return spawnPoints[spawnIndex].GetRandomPoint();
        }
    }
}