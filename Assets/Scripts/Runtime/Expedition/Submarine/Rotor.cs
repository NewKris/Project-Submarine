using System;
using UnityEngine;

namespace WereHorse.Runtime.Expedition.Submarine {
    public class Rotor : MonoBehaviour {
        public SubmarineBody submarineBody;
        public float rotationSpeed;
        public float acceleration;
        public AnimationCurve curve;

        private float _torque;
        private float _targetTorque;
        
        private void Update() {
            _targetTorque = curve.Evaluate(submarineBody.Thrust) * rotationSpeed;
            _torque = Mathf.MoveTowards(_torque, _targetTorque, acceleration * Time.deltaTime);
            transform.Rotate(Vector3.forward * (_torque * Time.deltaTime), Space.Self);
        }
    }
}