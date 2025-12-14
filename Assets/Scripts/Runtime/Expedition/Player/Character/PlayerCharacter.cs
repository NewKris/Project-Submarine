using System;
using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Common;
using WereHorse.Runtime.Expedition.Hud;
using WereHorse.Runtime.Expedition.Interaction;
using WereHorse.Runtime.Expedition.Interaction.Interface;
using WereHorse.Runtime.Expedition.Inventory;
using WereHorse.Runtime.Expedition.Player.Stations;
using WereHorse.Runtime.Utility.Extensions;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class PlayerCharacter : NetworkBehaviourExtended {
        public static PlayerCharacter ownedCharacter;
        
        public float maxMoveSpeed;
        
        [Header("Jumping")]
        public float jumpHeight;
        public float jumpTime;

        [Header("References")] 
        public Transform itemHand;
        public Transform yawPivot;
        public PlayerCamera playerCamera;
        public InteractionController interactionController;
        public PlayerHud hud;
        public SkinnedMeshRenderer thirdPersonModel;
        public CharacterAnimator thirdPersonAnimator;
        public GroundChecker groundChecker;

        private bool _characterLocked;
        private float _gravity;
        private float _jumpForce;
        private Rigidbody _rigidbody;
        private Station _currentStation;
        private ItemObject _heldItem;
        private CursorLockMode _lockMode = CursorLockMode.Locked;

        public void PickUpItem(ItemObject item) {
            _heldItem = item;
            thirdPersonAnimator.SetIsCarrying(true);
            item.gameObject.layer = LayerMask.NameToLayer("Owner Hidden");
            // Show Item Proxy Here
            
            _heldItem.PickUpRpc(NetworkManager.LocalClientId);
        }
        
        public void PossessStation(Station station) {
            if (_heldItem) {
                return;
            }
            
            _currentStation = station;
            SetPlayerLock(true);
            StickToStation();
        }
        
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
            _rigidbody.position = position;
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
            _rigidbody.AddForce(Vector3.up * _gravity, ForceMode.Acceleration);
        }
        
        private void Jump() {
            if (groundChecker.Evaluate()) {
                float delta = _jumpForce - _rigidbody.linearVelocity.y;
                _rigidbody.AddForce(Vector3.up * delta, ForceMode.VelocityChange);
            }
        }

        private void Move() {
            thirdPersonAnimator.SetMoving(PlayerInputListener.Move != Vector2.zero);
            thirdPersonAnimator.SetMoveDirection(PlayerInputListener.Move);

            Vector3 targetVel = transform.rotation * PlayerInputListener.Move.ProjectOnGround();
            targetVel = targetVel.normalized * maxMoveSpeed;
            targetVel.y = _rigidbody.linearVelocity.y;

            Vector3 delta = targetVel - _rigidbody.linearVelocity;
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
            _currentStation = null;
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

        private void TryInteract() {
            if (!_characterLocked) {
                interactionController.TryInteract();
            }
        }

        private void TryCancel() {
            if (_currentStation) {
                DePossessStation();
            }

            if (_heldItem) {
                thirdPersonAnimator.SetIsCarrying(false);
                _heldItem.gameObject.layer = LayerMask.NameToLayer("Interaction");
                _heldItem.DropItemRpc();
                _heldItem = null;
            }
        }

        private void SubscribeListeners() {
            PlayerInputListener.OnInteract += TryInteract;
            PlayerInputListener.OnJump += Jump;
            PlayerInputListener.OnExit += TryCancel;
            PlayerInputListener.OnGrab += GrabHandle;
            PlayerInputListener.OnRelease += ReleaseHandle;
            PauseManager.OnPauseStateChanged += SetPauseState;
        }

        private void DisposeListeners() {
            PlayerInputListener.OnInteract -= TryInteract;
            PlayerInputListener.OnJump -= Jump;
            PlayerInputListener.OnExit -= TryCancel;
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