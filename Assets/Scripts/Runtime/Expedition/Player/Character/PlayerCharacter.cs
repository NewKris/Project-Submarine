using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Hud;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Player.Stations;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public static PlayerCharacter ownedCharacter;
        
        public float maxMoveSpeed;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public SkinnedMeshRenderer thirdPersonModel;
        public CharacterAnimator thirdPersonAnimator;

        private bool _usingStation;
        private CharacterController _character;
        private Station _currentStation;

        public void PossessStation(Station station) {
            _currentStation = station;
            _usingStation = true;
            _character.enabled = false;
            hud.gameObject.SetActive(false);
            CharacterInputListener.SetActive(false);
            
            _currentStation.Activate();
        }
        
        public void DePossessStation() {
            if (_currentStation) {
                _currentStation.Deactivate();
                _currentStation = null;
            }
            
            _usingStation = false;
            _character.enabled = true;
            hud.gameObject.SetActive(true);
            CharacterInputListener.SetActive(true);
        }
        
        [Rpc(SendTo.Owner)]
        public void SetPositionAndRotationRpc(Vector3 position, Quaternion rotation) {
            GetComponent<CharacterController>().enabled = false;
            transform.position = position;
            playerCamera.SetYaw(rotation.eulerAngles.y);
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
                
                thirdPersonModel.gameObject.layer = LayerMask.NameToLayer("Owner Hidden");

                CharacterInputListener.OnInteract += interactionController.TryInteract;
                PauseManager.OnPauseStateChanged += SetPauseState;

                ownedCharacter = this;
            });
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() {
            DoOnOwner(() => {
                CharacterInputListener.OnInteract -= interactionController.TryInteract;
                PauseManager.OnPauseStateChanged -= SetPauseState;
            });
        }

        private void Update() {
            if (_usingStation) {
                transform.position = _currentStation.stationPivot.position;
                transform.rotation = _currentStation.stationPivot.rotation;
            }
            else {
                Look();
                Move();
            }
        }
        
        private void Move() {
            thirdPersonAnimator.MovementInput = CharacterInputListener.Move;
            thirdPersonAnimator.Moving = CharacterInputListener.Move != Vector2.zero;
            
            Vector3 velocity = transform.rotation * CharacterInputListener.Move.ProjectOnGround() * maxMoveSpeed;
            _character.SimpleMove(velocity);
        }

        private void Look() {
            playerCamera.Look(CharacterInputListener.Look);
        }

        private void SetPauseState(bool isPaused) {
            if (!_usingStation) {
                CharacterInputListener.SetActive(!isPaused);
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}