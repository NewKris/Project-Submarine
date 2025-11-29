using UnityEngine;
using WereHorse.Runtime.Utility.CommonObjects;

namespace WereHorse.Runtime.Gameplay.Player {
    public class PlayerCamera : MonoBehaviour {
        [Range(0, 1)] public float sensitivity;
        public Transform yawPivot;
        
        [Header("Tuning")]
        public float maxRotateSpeed;
        
        [Header("Pitch")]
        [Range(0, 1)] public float pitchScale;
        public float minPitch;
        public float maxPitch;
        public float pitchDamping;
        
        [Header("Yaw")]
        [Range(0, 1)] public float yawScale;
        public float yawDamping;

        private DampedAngle _pitch;
        private DampedAngle _yaw;

        public float LookPitch => _pitch.Current;

        public void Look(Vector2 deltaMouse) {
            _pitch.Target -= deltaMouse.y * maxRotateSpeed * pitchScale * sensitivity;
            _pitch.Target = Mathf.Clamp(_pitch.Target, minPitch, maxPitch);
            
            _yaw.Target += deltaMouse.x * maxRotateSpeed * yawScale * sensitivity;
            _yaw.Target %= 360f;
        }

        private void Awake() {
            _pitch = new DampedAngle(0);
            _yaw = new DampedAngle(0);


            yawPivot.rotation = Quaternion.Euler(0, _yaw.Current, 0);
            transform.localRotation = Quaternion.Euler(_pitch.Current, 0, 0);
        }

        private void Update() {
            yawPivot.rotation = Quaternion.Euler(0, _yaw.Tick(yawDamping), 0);
            transform.localRotation = Quaternion.Euler(_pitch.Tick(pitchDamping), 0, 0);
        }
    }
}