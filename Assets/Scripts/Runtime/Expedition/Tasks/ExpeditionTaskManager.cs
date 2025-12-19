using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Inventory;

namespace WereHorse.Runtime.Expedition.Tasks {
    public class ExpeditionTaskManager : MonoBehaviour {
        public ItemObject[] objectives;
        public AreaOverlap itemCheckArea;

        public int TallyPoints() {
            int itemsInSafeArea = 0;
            Collider[] overlaps = itemCheckArea.Evaluate();
            
            foreach (ItemObject itemObject in objectives) {
                Collider col = itemObject.GetComponent<Collider>();
                if (Array.Exists(overlaps, x => x == col)) {
                    itemsInSafeArea++;
                }
            }
            
            return itemsInSafeArea;
        }
    }
}