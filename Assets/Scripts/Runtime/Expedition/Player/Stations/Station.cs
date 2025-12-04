using System;
using UnityEngine;
using WereHorse.Runtime.Expedition.Player.Character;
using WereHorse.Runtime.Expedition.Player.Stations.Interface;
using WereHorse.Runtime.Utility;
using WereHorse.Runtime.Utility.Attributes;

namespace WereHorse.Runtime.Expedition.Player.Stations {
    public class Station : MonoBehaviour {
        public Transform stationPivot;
        public float cameraDirection;
        [ReadOnly] public bool occupied;

        private InterfaceHandle _grabbedHandle;
        private InterfaceControl[] _controls;

        public void Activate() {
            StationInputListener.SetActive(true);
            StationInputListener.OnExit += Exit;
            StationInputListener.OnGrab += TryGrabHandle;
            StationInputListener.OnRelease += ReleaseHandle;
            PauseManager.OnPauseStateChanged += SetPauseState;
            Cursor.lockState = CursorLockMode.None;
            enabled = true;
            occupied = true;
            
            ActivateControls();
        }

        public void Deactivate() {
            StationInputListener.SetActive(false);
            StationInputListener.OnExit -= Exit;
            StationInputListener.OnGrab -= TryGrabHandle;
            StationInputListener.OnRelease -= ReleaseHandle;
            PauseManager.OnPauseStateChanged -= SetPauseState;
            Cursor.lockState = CursorLockMode.Locked;
            enabled = false;
            occupied = false;
            
            DeactivateControls();
        }
        
        private void Awake() {
            enabled = false;
            _controls = GetComponentsInChildren<InterfaceControl>();
            DeactivateControls();
        }
        
        private void OnDestroy() {
            if (enabled) {
                StationInputListener.OnExit -= Exit;
                PauseManager.OnPauseStateChanged -= SetPauseState;
            }
        }

        private void OnDrawGizmos() {
            if (stationPivot) {
                Vector3 cameraDir = Quaternion.AngleAxis(cameraDirection, Vector3.right) * Vector3.forward;
                cameraDir = stationPivot.TransformDirection(cameraDir).normalized;
                
                HandlesProxy.DrawDisc(stationPivot.position, Vector3.up, 0.5f, true, Color.yellow);
                HandlesProxy.DrawRay(stationPivot.position + Vector3.up * 1.6f, cameraDir, 3, false, Color.red);
            }
        }
        
        private void SetPauseState(bool isPaused) {
            if (enabled) {
                StationInputListener.SetActive(!isPaused);
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
        
        private void Exit() {
            PlayerCharacter.ownedCharacter.DePossessStation();
        }

        private void ActivateControls() {
            foreach (InterfaceControl control in _controls) {
                control.Activate();
            }
        }
        
        private void DeactivateControls() {
            foreach (InterfaceControl control in _controls) {
                control.Deactivate();
            }
        }

        private void TryGrabHandle() {
            Ray ray = Camera.main.ScreenPointToRay(StationInputListener.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.TryGetComponent(out InterfaceHandle handle)) {
                _grabbedHandle = handle;
                _grabbedHandle.Grab();
            }
        }

        private void ReleaseHandle() {
            if (_grabbedHandle) {
                _grabbedHandle.Release();
                _grabbedHandle = null;
            }
        }
    }
}