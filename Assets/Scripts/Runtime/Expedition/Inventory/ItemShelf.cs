using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ItemShelf : Interactable {
        public static event Action<ItemShelf> OnInteracted;

        public Transform pin;
        
        public override void Interact() {
            OnInteracted?.Invoke(this);
        }
    }
}