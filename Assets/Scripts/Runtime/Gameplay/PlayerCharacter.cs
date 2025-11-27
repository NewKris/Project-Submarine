using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using Werehorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Gameplay {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public float maxMoveSpeed;

        private CharacterController _character;
        
        private void Start() {
            DoOnNonOwners(() => {
                enabled = false;
            });
            
            DoOnOwner(() => {
                enabled = true;
                _character = GetComponent<CharacterController>();
            });
        }

        private void Update() {
            Vector3 velocity = transform.rotation * InputListener.Move.ProjectOnGround() * maxMoveSpeed;
            _character.SimpleMove(velocity);
        }
    }
}