using Unity.Netcode;
using UnityEngine;
using WereHorse.Runtime.Utility.CommonObjects;

namespace WereHorse.Runtime.Gameplay.Player.Telescope {
    public class ExternalTelescope : MonoBehaviour {
        public float speed;
        public float damping;
        public Transform yawPivot;
        public Transform pitchPivot;

        private DampedAngle _localYaw;
        private DampedAngle _localPitch;
        private NetworkVariable<float> _yaw;
        private NetworkVariable<float> _pitch;
        
        public void Look(Vector2 look) {
            _yaw.Value += look.x * speed;
            _pitch.Value += look.y * speed;
        }

        private void Awake() {
            _yaw = new NetworkVariable<float>(0);
            _pitch = new NetworkVariable<float>(0);
        }

        private void Update() {
            _localYaw.Target = _yaw.Value;
            _localPitch.Target = _pitch.Value;
            
            yawPivot.localRotation = Quaternion.Euler(0, _localYaw.Tick(damping), 0);
            pitchPivot.localRotation = Quaternion.Euler(_localPitch.Tick(damping), 0, 0);
        }
    }
}