using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Utility.Attributes;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Expedition.Inventory {
    [RequireComponent(typeof(ItemPickup))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkRigidbody))]
    public class ItemObject : NetworkBehaviourExtended {
        public int itemId;
        public ItemShelf shelf;
        
        private Transform _pin;

        [Rpc(SendTo.Server)]
        public void PlaceOnShelfRpc(int shelfIndex) {
            shelf = ShelfManager.GetShelf(shelfIndex);
            shelf.currentItem = this;
            Pin(shelf.pin);
            ToggleColliderRpc(true);
        }
        
        [Rpc(SendTo.Server)]
        public void PickUpRpc(ulong byPlayer) {
            RemoveFromShelf();
            
            PlayerCharacter playerCharacter = ExpeditionController.GetPlayerCharacter(byPlayer)
                .GetComponentInChildren<PlayerCharacter>();
            
            Pin(playerCharacter.itemHand);
            ToggleColliderRpc(false);
        }
        
        [Rpc(SendTo.Server)]
        public void DropItemRpc() {
            RemoveFromShelf();
            UnPin();
            ToggleColliderRpc(true);
        }

        private void Reset() {
            GetComponent<NetworkRigidbody>().UseRigidBodyForMotion = true;
            
            NetworkTransform networkTransform = GetComponent<NetworkTransform>();
            networkTransform.SyncScaleX = false;
            networkTransform.SyncScaleY = false;
            networkTransform.SyncScaleZ = false;
            networkTransform.PositionInterpolationType = NetworkTransform.InterpolationTypes.Lerp;
            networkTransform.RotationInterpolationType = NetworkTransform.InterpolationTypes.Lerp;
            
            gameObject.layer = LayerMask.NameToLayer("Interaction");
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
            DoOnServer(() => {
                if (shelf) {
                    shelf.currentItem = this;
                    Pin(shelf.pin);
                }
                else {
                    UnPin();
                }
            });
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
            ToggleColliders(canCollide);
        }

        private void ToggleColliders(bool canCollide) {
            GetComponentsInChildren<Collider>().ForEach(c => c.enabled = canCollide);
        }

        private void RemoveFromShelf() {
            if (shelf) {
                shelf.currentItem = null;
                shelf = null;
            }
        }
    }
}