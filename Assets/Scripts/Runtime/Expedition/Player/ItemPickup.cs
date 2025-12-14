using UnityEngine;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Inventory;
using WereHorse.Runtime.Expedition.Player.Character;

namespace WereHorse.Runtime.Expedition.Player {
    [RequireComponent(typeof(ItemObject))]
    public class ItemPickup : Interactable {
        public override void Interact() {
            PlayerCharacter.ownedCharacter.PickUpItem(GetComponent<ItemObject>());
        }
    }
}