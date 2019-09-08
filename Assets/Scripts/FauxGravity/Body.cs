using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FauxGravity {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Body : MonoBehaviour {
        public Vector3 targetUp;

        public Vector3 targetForward;

        [SerializeField] protected IAttractor attractor;

        public bool IsGrounded => _isGrounded;
        public Rigidbody rigidBody { get; protected set; }
        public Collider bodyCollider { get; protected set; }

        [SerializeField] protected FloatReference _sphereCastRadius = new FloatReference();
        [SerializeField] protected FloatReference _maxCastDistance = new FloatReference();

        [SerializeField] protected bool _isGrounded;

        protected virtual void Start() {
            rigidBody = GetComponent<Rigidbody>();
            // rb.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.useGravity = false;

            bodyCollider = GetComponent<Collider>();
        }

        public virtual void OnTriggerEnter(Collider other) {
            if (attractor == null) {
                IAttractor att = other.transform.parent.GetComponent<IAttractor>();
                if (att != null) {
                    attractor = att;
                    this.transform.parent = att.gameObject.transform;
                }
            }
        }

        public virtual void OnTriggerExit(Collider other) {
            if (attractor != null && other.gameObject.GetInstanceID() == attractor.gameObject.GetInstanceID()) {
                attractor = null;
                this.transform.parent = null;
            }
        }

        protected virtual void FixedUpdate() {
            if (attractor != null) {

                var gravityForce = attractor.Attract(this);
                targetUp = -gravityForce.normalized;
                // Debug.DrawRay(transform.position, targetUp, Color.red, 0.01f);
                AddGravityForce(gravityForce);

            }
            UpdateGrounded();
            OrientSelf();
        }
        public virtual void AddGravityForce(Vector3 gravity) {
            rigidBody.AddForce(gravity, ForceMode.Acceleration);
        }

        protected virtual void OrientSelf() {
            var targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * rigidBody.rotation;
            rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, 5f * Time.fixedDeltaTime);
        }

        private void UpdateGrounded() {
            RaycastHit hit;
            if (Physics.SphereCast(bodyCollider.bounds.center, _sphereCastRadius, -transform.up, out hit, _maxCastDistance)) {
                _isGrounded = true;
            } else {
                _isGrounded = false;
            }
        }
    }

}