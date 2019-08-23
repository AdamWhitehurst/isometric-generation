using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FauxGravity {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Body : MonoBehaviour {

        [SerializeField] protected Attractor attractor;
        public Vector3 targetUp;
        public LayerMask terrainMask;

        public float groundedRaycastVerticalOffset = 0.1f;
        public float maxCastDistance = 0.1f;

        public float raySpacing = 0.05f;
        public Rigidbody rb { get; protected set; }
        public Collider col { get; protected set; }
        void Start() {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;

            col = GetComponent<CapsuleCollider>();
        }

        public virtual void OnTriggerEnter(Collider other) {
            if (other.tag == "Attractor Range") {
                attractor = other.GetComponent<Attractor>();
            }
        }

        public virtual void OnTriggerExit(Collider other) {
            if (attractor != null && other.gameObject.GetInstanceID() == attractor.gameObject.GetInstanceID()) {
                attractor = null;
            }
        }

        protected virtual void FixedUpdate() {
            if (attractor) {

                var newTargetUp = attractor.Attract(this);
                if (newTargetUp != transform.up) {
                    // Debug.DrawRay(transform.position, targetUp, Color.blue, 0.01f);

                    targetUp = newTargetUp;
                }

            }
        }
        public virtual void AddGravityForce(Vector3 gravity) {

            rb.AddForce(-transform.up * gravity.magnitude, ForceMode.Acceleration);

            // Debug.DrawRay(transform.position, -transform.up, Color.green, 0.01f);
        }
        public bool IsGrounded() {
            if (attractor == null) return false;

            // Vector3 originC = transform.position + (transform.up * groundedRaycastVerticalOffset);
            Vector3 originFL = transform.position + (transform.up * groundedRaycastVerticalOffset) + (transform.forward * raySpacing) + (-transform.right * raySpacing);
            Vector3 originFR = transform.position + (transform.up * groundedRaycastVerticalOffset) + (transform.forward * raySpacing) + (transform.right * raySpacing);
            Vector3 originRL = transform.position + (transform.up * groundedRaycastVerticalOffset) + (-transform.forward * raySpacing) + (-transform.right * raySpacing);
            Vector3 originRR = transform.position + (transform.up * groundedRaycastVerticalOffset) + (-transform.forward * raySpacing) + (transform.right * raySpacing);

            // RaycastHit c;
            RaycastHit fl;
            RaycastHit fr;
            RaycastHit rl;
            RaycastHit rr;

            // Physics.Raycast(originC, -targetUp, out c, maxCastDistance, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Raycast(originFL, -transform.up, out fl, maxCastDistance, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Raycast(originFR, -transform.up, out fr, maxCastDistance, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Raycast(originRL, -transform.up, out rl, maxCastDistance, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Raycast(originRR, -transform.up, out rr, maxCastDistance, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);

            if (fl.collider != null || fr.collider != null || rl.collider != null || rr.collider != null) {
                // Debug.DrawRay(originC, fl.normal, Color.white, 0.01f);
                // Debug.DrawRay(originFL, fl.normal, Color.white, 0.01f);
                // Debug.DrawRay(originFR, fr.normal, Color.white, 0.01f);
                // Debug.DrawRay(originRL, rl.normal, Color.white, 0.01f);
                // Debug.DrawRay(originRR, rr.normal, Color.white, 0.01f);
                return true;
            }

            return false;
        }
    }
}