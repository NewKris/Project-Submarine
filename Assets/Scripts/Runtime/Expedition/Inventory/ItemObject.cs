using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ItemObject : NetworkBehaviourExtended {
        private Transform _pin;

        [Rpc(SendTo.Server)]
        public void PlaceOnShelfRpc(int shelfIndex) {
            Pin(ShelfManager.GetShelf(shelfIndex).pin);
            ToggleColliderRpc(true);
        }
        
        [Rpc(SendTo.Server)]
        public void PickUpRpc(ulong byPlayer) {
            PlayerCharacter playerCharacter = ExpeditionController.GetPlayerCharacter(byPlayer)
                .GetComponentInChildren<PlayerCharacter>();
            
            Pin(playerCharacter.itemHand);
            ToggleColliderRpc(false);
        }
        
        [Rpc(SendTo.Server)]
        public void DropItemRpc() {
            UnPin();
            ToggleColliderRpc(true);
        }
        
        private void Pin(Transform pin) {
            _pin = pin;
            SetPinMode(true);
        }

        private void UnPin() {
            _pin = null;
            SetPinMode(false);
        }

        private void Start() {
            enabled = false;
        }

        private void Update() {
            if (_pin) {
                transform.SetPositionAndRotation(_pin.position, _pin.rotation);
            }
        }

        private void SetPinMode(bool isPinned) {
            GetComponent<Rigidbody>().useGravity = !isPinned;
            GetComponent<Rigidbody>().constraints = isPinned ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
            GetComponent<Interactable>().enabled = !isPinned;
            enabled = isPinned;
        }

        [Rpc(SendTo.Everyone)]
        private void ToggleColliderRpc(bool canCollide) {
            GetComponent<Collider>().enabled = canCollide;
        }
    }
}