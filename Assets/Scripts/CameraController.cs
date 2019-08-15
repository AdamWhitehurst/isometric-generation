using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Follows a target and maintains offset.
/// Able to be rotated in 90 degree increments about y-axis.

/// </summary>
[ExecuteInEditMode]
public class CameraController : MonoBehaviour {
    public Transform currentTarget;
    public Vector3 offset;
    public float moveSpeed = 5;
    public float turnSpeed = 10;
    public float smoothSpeed = 0.5f;


    Quaternion targetRotation;
    Vector3 targetPos;
    bool smoothRotating = false;

    void Update() {
        MoveWithTarget();
        LookAtTarget();

        if (Input.GetKeyDown(KeyCode.Q) && !smoothRotating) {
            StartCoroutine("RotateAroundTarget", 90);

        }

        if (Input.GetKeyDown(KeyCode.E) && !smoothRotating) {
            StartCoroutine("RotateAroundTarget", -90);
        }

    }
    /// <summary>
    /// Move camera with target, maintaining offset
    /// </summary>
    void MoveWithTarget() {
        targetPos = currentTarget.position + offset;
        if (!smoothRotating) {
            transform.position = targetPos;
        } else {
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// Use look vector (target - current) to aim camera at target
    /// </summary>
    void LookAtTarget() {
        targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    IEnumerator RotateAroundTarget(float angle) {
        Vector3 vel = Vector3.zero;
        Vector3 targetOffsetPos = Quaternion.Euler(0, angle, 0) * offset;
        smoothRotating = true;
        float dist = Vector3.Distance(offset, targetOffsetPos);

        while (dist > 0.03f) {
            offset = Vector3.SmoothDamp(offset, targetOffsetPos, ref vel, smoothSpeed);
            dist = Vector3.Distance(offset, targetOffsetPos);
            yield return null;
        }

        smoothRotating = false;
        offset = targetOffsetPos;
    }
}
