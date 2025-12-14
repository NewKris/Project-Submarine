using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Inventory {
    public class ItemObject : NetworkBehaviourExtended {
        [ReadOnly] public Transform pin;

        public void PickUp(Transform hand, ulong owner) {
            pin = hand;
            SetOwnerRpc(owner);
            SetPinMode(true);
        }

        public void Drop() {
            pin = null;
            SetOwnerRpc(0);
            SetPinMode(false);
        }

        private void Start() {
            enabled = false;
        }

        private void Update() {
            if (pin) {
                transform.SetPositionAndRotation(pin.position, pin.rotation);
            }
        }

        [Rpc(SendTo.Server)]
        private void SetOwnerRpc(ulong ownerId) {
            GetComponent<NetworkObject>().ChangeOwnership(ownerId);
        }

        private void SetPinMode(bool isPinned) {
            GetComponent<Rigidbody>().useGravity = !isPinned;
            GetComponent<Rigidbody>().constraints = isPinned ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
            GetComponent<Collider>().enabled = !isPinned;
            GetComponent<Interactable>().enabled = !isPinned;
            enabled = isPinned;
        }
    }
}