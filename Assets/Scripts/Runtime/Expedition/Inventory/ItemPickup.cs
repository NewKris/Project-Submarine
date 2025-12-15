using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;

namespace WereHorse.Runtime.Expedition.Inventory {
    [RequireComponent(typeof(ItemObject))]
    public class ItemPickup : Interactable {
        public static event Action<ItemObject> OnInteracted;
        
        public override void Interact() {
            OnInteracted?.Invoke(GetComponent<ItemObject>());
        }
    }
}