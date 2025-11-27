using UnityEngine;

namespace Werehorse.Runtime.Utility.CommonObjects {
    public struct DampedRotation {
        private float _velocity;

        public Quaternion Current { get; set; }
        public Quaternion Target { get; set; }

        public DampedRotation(Quaternion startValue) {
            Current = startValue;
            Target = startValue;
            _velocity = 0;
        }

        public Quaternion Tick(float damping) {
            float deltaTime = Time.deltaTime;
            return Tick(damping, deltaTime);
        }
		
        public Quaternion Tick(float damping, float deltaTime) {
            float deltaAngle = Quaternion.Angle(Current, Target);
            
            if (deltaAngle > 0) {
                float t = Mathf.SmoothDampAngle(deltaAngle, 0, ref _velocity, damping, Mathf.Infinity, deltaTime);
                t = 1 - (t / deltaAngle);
                Current = Quaternion.SlerpUnclamped(Current, Target, t);
            }
            
            return Current;
        }
    }
}
