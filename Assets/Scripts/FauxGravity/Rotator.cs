using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FauxGravity {
    [RequireComponent(typeof(Rigidbody))]
    public class Rotator : MonoBehaviour {
        public bool rotate = false;
        public Vector3 axisOfOrbit;
        public float rotationSpeed = 80.0f;

        private Rigidbody m_rb;
        private void Start() {
            m_rb = GetComponent<Rigidbody>();
        }
        private void FixedUpdate() {
            if (rotate) UpdateRotation();
        }

        private void UpdateRotation() {
            // transform.RotateAround(transform.position, axisOfOrbit, rotationSpeed * Time.deltaTime);
            // var targetRotation = Quaternion.FromToRotation(transform.eulerAngles, transform.eulerAngles + axisOfOrbit);
            // m_rb.rotation.SetEulerAngles(m_rb.rotation.eulerAngles.x + axisOfOrbit.x, m_rb.rotation.eulerAngles.y + axisOfOrbit.y, m_rb.rotation.eulerAngles.z + axisOfOrbit.z);
            var targetRotation = Quaternion.Euler(axisOfOrbit) * m_rb.rotation;
            m_rb.MoveRotation(Quaternion.Slerp(m_rb.rotation, targetRotation, Time.fixedDeltaTime));
        }
    }
}