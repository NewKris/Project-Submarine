using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Hud;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public static PlayerCharacter ownedCharacter;
        
        public float maxMoveSpeed;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public GameObject thirdPersonModel;

        private bool _freeMouse;
        private bool _usingStation;
        private CharacterController _character;
        private Station _currentStation;

        public void PossessStation(Station station) {
            _currentStation = station;
            _usingStation = true;
            _character.enabled = false;
            _currentStation.Activate();
            
            SetPositionAndRotationRpc(station.stationPivot.position, station.stationPivot.rotation);
        }
        
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

                CharacterInputListener.OnToggleMouse += ToggleMouse;
                CharacterInputListener.OnInteract += interactionController.TryInteract;
                CharacterInputListener.OnExitStation += DePossessStation;

                ownedCharacter = this;
            });
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() {
            DoOnOwner(() => {
                CharacterInputListener.OnToggleMouse -= ToggleMouse;
                CharacterInputListener.OnInteract -= interactionController.TryInteract;
                CharacterInputListener.OnExitStation -= DePossessStation;
            });
        }

        private void Update() {
            if (_freeMouse) {
                return;
            }

            if (_usingStation) {
                transform.position = _currentStation.stationPivot.position;
                transform.rotation = _currentStation.stationPivot.rotation;
            }
            else {
                Look();
                Move();
            }
        }
        
        private void DePossessStation() {
            if (_currentStation) {
                _currentStation.Deactivate();
                _currentStation = null;
            }
            
            _usingStation = false;
            _character.enabled = true;
        }
        
        private void Move() {
            Vector3 velocity = transform.rotation * CharacterInputListener.Move.ProjectOnGround() * maxMoveSpeed;
            _character.SimpleMove(velocity);
        }

        private void Look() {
            playerCamera.Look(CharacterInputListener.Look);
        }

        private void ToggleMouse() {
            _freeMouse = !_freeMouse;
            Cursor.lockState = _freeMouse ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}