using System;
using UnityEngine;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ShelfManager : MonoBehaviour {
        private static ShelfManager Instance;
        
        private ItemShelf[] _shelves;

        public static ItemShelf GetShelf(int index) {
            return Instance._shelves[index];
        }
        
        public static int GetIndex(ItemShelf shelf) {
            return Array.IndexOf(Instance._shelves, shelf);
        }

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            _shelves = FindObjectsByType<ItemShelf>(FindObjectsSortMode.None);
        }
    }
}