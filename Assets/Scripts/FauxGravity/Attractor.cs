using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FauxGravity {
    [RequireComponent(typeof(SphereCollider))]
    public class Attractor : MonoBehaviour {
        public float gravity = -9.81f;
        public float raySpacing = 0.05f;
        public float rayVerticalOffset = 0.1f;

        public Vector3 Attract(Body body) {
            Vector3 distance = body.transform.position - transform.position;
            Vector3 gravityUp = distance.normalized;
            Vector3 targetUp = body.transform.up;

            Vector3 originFL = body.transform.position + (body.transform.up * rayVerticalOffset) + (body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
            Vector3 originFR = body.transform.position + (body.transform.up * rayVerticalOffset) + (body.transform.forward * raySpacing) + (body.transform.right * raySpacing);
            Vector3 originRL = body.transform.position + (body.transform.up * rayVerticalOffset) + (-body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
            Vector3 originRR = body.transform.position + (body.transform.up * rayVerticalOffset) + (-body.transform.forward * raySpacing) + (body.transform.right * raySpacing);

            RaycastHit fl;
            RaycastHit fr;
            RaycastHit rl;
            RaycastHit rr;

            Physics.Linecast(originFL, transform.position, out fl, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Linecast(originFR, transform.position, out fr, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Linecast(originRL, transform.position, out rl, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
            Physics.Linecast(originRR, transform.position, out rr, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);

            // Debug.Log($"{fl.normal}, {fr.normal},{c.normal}, {rl.normal}, {rr.normal}");

            if (fl.normal == rr.normal
            && fr.normal == rl.normal) {
                // Debug.DrawRay(originFL, fl.normal, Color.yellow, 0.01f);
                // Debug.DrawRay(originFR, fr.normal, Color.yellow, 0.01f);
                // Debug.DrawRay(originRL, rl.normal, Color.yellow, 0.01f);
                // Debug.DrawRay(originRR, rr.normal, Color.yellow, 0.01f);

                targetUp = fr.normal;
            }

            body.AddGravityForce(gravityUp * gravity);
            return targetUp;
        }
    }
}
