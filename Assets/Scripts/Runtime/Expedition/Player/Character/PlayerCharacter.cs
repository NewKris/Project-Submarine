using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Hud;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Interaction.Interface;
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
        private bool _characterLocked;
        private float _gravity;
        private float _jumpForce;
        private Vector3 _previousPosition;
        private Rigidbody _rigidbody;
        private Station _currentStation;
        private CursorLockMode _lockMode = CursorLockMode.Locked;

        public void PossessStation(Station station) {
            _currentStation = station;
            SetPlayerLock(true);
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
            DoOnNonOwners(DisableNonOwnedCharacter);
            
            DoOnOwner(() => {
                thirdPersonModel.gameObject.layer = LayerMask.NameToLayer("Owner Hidden");
                _rigidbody = GetComponent<Rigidbody>();
                ownedCharacter = this;

                SubscribeListeners();
                CalculateJumpValues();
                SetPlayerLock(false);
            });
        }

        private void OnDisable() {
            DoOnOwner(DisposeListeners);
        }

        private void Update() {
            if (!_characterLocked) {
                Look();
            }
        }

        private void FixedUpdate() {
            _underWater = transform.position.y > waterLevel;

            if (!_characterLocked) {
                Fall();
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

            if (!isPaused && !_currentStation) {
                SetPlayerLock(false);
            }
        }
        
        private void DePossessStation() {
            if (_currentStation) {
                _currentStation = null;
            }
            
            SetPlayerLock(false);
        }

        private void GrabHandle() {
            if (interactionController.TryGrabHandle(out InterfaceControl control)) {
                if (control.LockPlayer()) {
                    SetPlayerLock(true);
                }
            }
        }

        private void ReleaseHandle() {
            interactionController.TryReleaseHandle();

            if (!_currentStation) {
                SetPlayerLock(false);
            }
        }
        
        private void SetPlayerLock(bool locked) {
            _characterLocked = locked;
            hud.gameObject.SetActive(!locked);
            _rigidbody.constraints = locked ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
            
            _lockMode = locked ?  CursorLockMode.None : CursorLockMode.Locked;
            Cursor.lockState = _lockMode;
        }

        private void SubscribeListeners() {
            PlayerInputListener.OnInteract += interactionController.TryInteract;
            PlayerInputListener.OnJump += Jump;
            PlayerInputListener.OnExit += DePossessStation;
            PlayerInputListener.OnGrab += GrabHandle;
            PlayerInputListener.OnRelease += ReleaseHandle;
            PauseManager.OnPauseStateChanged += SetPauseState;
        }

        private void DisposeListeners() {
            PlayerInputListener.OnInteract -= interactionController.TryInteract;
            PlayerInputListener.OnJump -= Jump;
            PlayerInputListener.OnExit -= DePossessStation;
            PlayerInputListener.OnGrab -= GrabHandle;
            PlayerInputListener.OnRelease -= ReleaseHandle;
            PauseManager.OnPauseStateChanged -= SetPauseState;
        }
        
        private void CalculateJumpValues() {
            float t = jumpTime * 0.5f;
            _gravity = (-2 * jumpHeight) / (t * t);
            _jumpForce = (2 * jumpHeight) / t;
        }

        private void DisableNonOwnedCharacter() {
            enabled = false;
            playerCamera.gameObject.SetActive(false);
            interactionController.gameObject.SetActive(false);
            hud.gameObject.SetActive(false);
        }
    }
}