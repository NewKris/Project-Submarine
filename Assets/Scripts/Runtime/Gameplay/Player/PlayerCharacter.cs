using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Gameplay.Hud;
using WereHorse.Runtime.Gameplay.Interaction;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Gameplay.Player {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public static PlayerCharacter OwnedCharacter;
        
        public float maxMoveSpeed;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public GameObject thirdPersonModel;

        private bool _freeMouse;
        private CharacterController _character;
        
        [Rpc(SendTo.Owner)]
        public void SetPositionAndRotationRpc(Vector3 position, Quaternion rotation) {
            GetComponent<CharacterController>().enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            GetComponent<CharacterController>().enabled = true;
        }

        
        private void Start() {
            DoOnNonOwners(() => {
                enabled = false;
                playerCamera.gameObject.SetActive(false);
                interactionController.gameObject.SetActive(false);
                hud.gameObject.SetActive(false);
            });
            
            DoOnOwner(() => {
                _character = GetComponent<CharacterController>();
                _freeMouse = false;
                
                thirdPersonModel.gameObject.SetActive(false);

                InputListener.OnToggleMouse += ToggleMouse;
                InputListener.OnInteract += interactionController.TryInteract;

                OwnedCharacter = this;
            });
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() {
            DoOnOwner(() => {
                InputListener.OnToggleMouse -= ToggleMouse;
                InputListener.OnInteract -= interactionController.TryInteract;
            });
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
            if (!_freeMouse) {
                playerCamera.Look(InputListener.Look);
            }
        }

        private void ToggleMouse() {
            _freeMouse = !_freeMouse;
            Cursor.lockState = _freeMouse ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void PositionToSpawnPoint() {
            
        }
    }
}