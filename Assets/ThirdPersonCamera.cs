using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    #region Fields
    public Transform target;

    #endregion

    #region MonoBehaviour Methods
    void LateUpdate() {

        // Vector3 targetDirection = target.position - transform.position;
        // Debug.DrawRay(transform.position, targetDirection.normalized, Color.cyan, 0.01f);

        // transform.forward = targetDirection.normalized;
        // transform.rotation = Quaternion.FromToRotation(transform.up, target.up);



        transform.LookAt(target, target.up);

        // var offset = targetToTransform.normalized;
        // offset *= targetToTransform.magnitude;
        // offset.y += currentXOffset;
        // offset.x += currentXOffset;

        // transform.position = target.position + offset;

    }
    #endregion

}
