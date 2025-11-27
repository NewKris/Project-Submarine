using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using Werehorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Gameplay {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public float maxMoveSpeed;
        public PlayerCamera playerCamera;
        public GameObject thirdPersonModel;

        private CharacterController _character;
        
        private void Start() {
            DoOnNonOwners(() => {
                enabled = false;
                playerCamera.gameObject.SetActive(false);
            });
            
            DoOnOwner(() => {
                _character = GetComponent<CharacterController>();
            });
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() {
            Look();
            Move();
        }

        private void Move() {
            Vector3 velocity = transform.rotation * InputListener.Move.ProjectOnGround() * maxMoveSpeed;
            _character.SimpleMove(velocity);
        }

        private void Look() {
            playerCamera.Look(InputListener.Look);
        }
    }
}