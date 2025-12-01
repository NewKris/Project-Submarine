using System;
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
        public float maxSwimSpeed;
        public float maxSwimAcceleration;
        public float waterLevel;

        [Header("References")] 
        public Transform yawPivot;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public SkinnedMeshRenderer thirdPersonModel;
        public CharacterAnimator thirdPersonAnimator;

        private bool _underWater;
        private bool _usingStation;
        private Rigidbody _rigidbody;
        private Station _currentStation;

        public void PossessStation(Station station) {
            _currentStation = station;
            _usingStation = true;
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
            hud.gameObject.SetActive(true);
            CharacterInputListener.SetActive(true);
        }
        
        [Rpc(SendTo.Owner)]
        public void SetPositionAndRotationRpc(Vector3 position, Quaternion rotation) {
            transform.position = position;
            playerCamera.SetYaw(rotation.eulerAngles.y);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        private void Start() {
            DoOnNonOwners(() => {
                enabled = false;
                playerCamera.gameObject.SetActive(false);
                interactionController.gameObject.SetActive(false);
                hud.gameObject.SetActive(false);
            });
            
            DoOnOwner(() => {
                _rigidbody = GetComponent<Rigidbody>();
                
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
                playerCamera.SetYaw(_currentStation.stationPivot.rotation.eulerAngles.y);
            }
            else {
                Look();
            }
        }

        private void FixedUpdate() {
            _underWater = transform.position.y > waterLevel;
            _rigidbody.useGravity = !_underWater;

            if (!_usingStation) {
                Move();
            }
        }

        private void Move() {
            thirdPersonAnimator.Swimming = _underWater;
            thirdPersonAnimator.MovementInput = CharacterInputListener.Move;
            thirdPersonAnimator.Moving = CharacterInputListener.Move != Vector2.zero;
            
            Vector3 targetVel = transform.rotation * CharacterInputListener.Move.ProjectOnGround();
            Vector3 vel;
            
            if (_underWater) {
                targetVel.y = CharacterInputListener.Lift;
                targetVel = targetVel.normalized * maxSwimSpeed;
                vel = Vector3.MoveTowards(
                    _rigidbody.linearVelocity, 
                    targetVel, 
                    maxSwimAcceleration * Time.fixedDeltaTime
                );
            }
            else {
                targetVel = targetVel.normalized * maxMoveSpeed;
                targetVel.y = _rigidbody.linearVelocity.y;
                vel = targetVel;
            }

            Vector3 delta = vel - _rigidbody.linearVelocity;
            _rigidbody.AddForce(delta, ForceMode.VelocityChange);
        }

        private void Look() {
            playerCamera.Look(CharacterInputListener.Look);
            transform.rotation = yawPivot.rotation;
        }

        private void SetPauseState(bool isPaused) {
            if (!_usingStation) {
                CharacterInputListener.SetActive(!isPaused);
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}