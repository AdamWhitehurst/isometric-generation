using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FauxGravity {
    public class Rotator : MonoBehaviour {
        public bool rotate = false;
        public Vector3 axisOfOrbit;
        public float rotationSpeed = 80.0f;
        private void FixedUpdate() {
            if (rotate) UpdateRotation();
        }

        private void UpdateRotation() {
            transform.RotateAround(transform.position, axisOfOrbit, rotationSpeed * Time.deltaTime);
        }
    }
}