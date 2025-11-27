using UnityEngine;

namespace Werehorse.Runtime.Utility.CommonObjects {
	public struct DampedVector
	{
		private Vector3 _velocity;

		public Vector3 Current { get; private set; }
		public Vector3 Target { get; set; }

		public DampedVector(Vector3 startValue) {
			Current = startValue;
			Target = startValue;
			_velocity = Vector3.zero;
		}

		public Vector3 Tick(float damping) {
			return Tick(damping, Time.deltaTime, Mathf.Infinity);
		}
		
		public Vector3 Tick(float damping, float maxSpeed) {
			return Tick(damping, Time.deltaTime, maxSpeed);
		}
		
		public Vector3 Tick(float damping, float deltaTime, float maxSpeed) {
			Current = Vector3.SmoothDamp(Current, Target, ref _velocity, damping, maxSpeed, deltaTime);
			return Current;
		}
	}
}
