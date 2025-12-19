using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ItemShelf : Interactable {
        public static event Action<ItemShelf> OnInteracted;

        public Transform pin;
        public int[] allowedIds;
        [ReadOnly] public ItemObject currentItem;

        public bool CanHoldItem(ItemObject item) {
            return allowedIds.Length == 0 || Array.Exists(allowedIds, id => id == item.itemId);
        }
        
        public override void Interact() {
            OnInteracted?.Invoke(this);
        }

        private void Reset() {
            gameObject.layer = LayerMask.NameToLayer("Interaction");
        }
    }
}