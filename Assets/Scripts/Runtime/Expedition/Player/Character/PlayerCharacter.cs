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
        
        [Header("Jumping")]
        public float jumpHeight;
        public float jumpTime;

        [Header("References")] 
        public Transform yawPivot;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public SkinnedMeshRenderer thirdPersonModel;
        public CharacterAnimator thirdPersonAnimator;
        public GroundChecker groundChecker;

        private bool _underWater;
        private bool _usingStation;
        private float _gravity;
        private float _jumpForce;
        private Vector3 _previousPosition;
        private Rigidbody _rigidbody;
        private Station _currentStation;
        private CursorLockMode _lockMode = CursorLockMode.Locked;

        public void PossessStation(Station station) {
            _currentStation = station;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _usingStation = true;
            hud.gameObject.SetActive(false);
            SetCursorLock(false);
            
            StickToStation();
        }
        
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
            _rigidbody.position = position;
            _previousPosition = position;
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

                PlayerInputListener.OnInteract += interactionController.TryInteract;
                PlayerInputListener.OnJump += Jump;
                PlayerInputListener.OnExit += DePossessStation;
                
                PauseManager.OnPauseStateChanged += SetPauseState;

                float t = jumpTime * 0.5f;
                _gravity = (-2 * jumpHeight) / (t * t);
                _jumpForce = (2 * jumpHeight) / t;

                ownedCharacter = this;
            });
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() {
            DoOnOwner(() => {
                PlayerInputListener.OnInteract -= interactionController.TryInteract;
                PlayerInputListener.OnJump -= Jump;
                PauseManager.OnPauseStateChanged -= SetPauseState;
            });
        }

        private void Update() {
            if (!_usingStation) {
                Fall();
                Look();
            }
        }

        private void FixedUpdate() {
            _underWater = transform.position.y > waterLevel;

            if (!_usingStation) {
                Move();
            }
        }

        private void StickToStation() {
            playerCamera.SetYaw(_currentStation.stationPivot.rotation.eulerAngles.y);
            playerCamera.SetPitch(_currentStation.cameraDirection);
            
            _rigidbody.position = _currentStation.stationPivot.position;
            transform.rotation = yawPivot.rotation;
        }

        private void Fall() {
            if (!_underWater) {
                _rigidbody.AddForce(Vector3.up * _gravity, ForceMode.Acceleration);
            }
        }
        
        private void Jump() {
            if (!_underWater && groundChecker.Evaluate()) {
                float delta = _jumpForce - _rigidbody.linearVelocity.y;
                _rigidbody.AddForce(Vector3.up * delta, ForceMode.VelocityChange);
            }
        }

        private void Move() {
            thirdPersonAnimator.Swimming = _underWater;
            thirdPersonAnimator.MovementInput = PlayerInputListener.Move;
            thirdPersonAnimator.Moving = PlayerInputListener.Move != Vector2.zero;

            Vector3 currentVel = (_rigidbody.position - _previousPosition) / Time.fixedDeltaTime;
            Vector3 targetVel = transform.rotation * PlayerInputListener.Move.ProjectOnGround();
            Vector3 vel;
            
            if (_underWater) {
                targetVel.y = PlayerInputListener.Lift;
                targetVel = targetVel.normalized * maxSwimSpeed;
                vel = Vector3.MoveTowards(
                    currentVel, 
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
            _previousPosition = _rigidbody.position;
            _rigidbody.AddForce(delta, ForceMode.VelocityChange);
        }

        private void Look() {
            playerCamera.Look(PlayerInputListener.Look);
            transform.rotation = yawPivot.rotation;
        }

        private void SetPauseState(bool isPaused) {
            PlayerInputListener.SetActive(!isPaused);
            Cursor.lockState = isPaused ? CursorLockMode.None : _lockMode;
        }
        
        private void DePossessStation() {
            if (_currentStation) {
                _currentStation = null;
            }
            
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _usingStation = false;
            SetCursorLock(true);
            hud.gameObject.SetActive(true);
        }

        private void SetCursorLock(bool locked) {
            _lockMode = locked ?  CursorLockMode.Locked : CursorLockMode.None;
            Cursor.lockState = _lockMode;
        }
    }
}