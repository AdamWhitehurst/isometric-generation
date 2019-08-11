using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;


[RequireComponent(typeof(Collider))]
public class Attractor : MonoBehaviour
{
    #region Fields 

    private Transform target { get; set; }

    #endregion

    #region MonoBehaviour Methods

    void Update()
    {
        if (target != null)
        {
            MoveToTarget();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (target == null && other.gameObject.layer == LayerMask.NameToLayer("Attractor"))
        {
            SetTarget(other.transform);
        }
    }

    void OnTriggerLeave(Collider other)
    {
        if (target != null && other.transform == target.transform)
        {
            RemoveTarget();
        }
    }

    #endregion

    #region ItemAttractor Methods

    void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    void RemoveTarget()
    {
        target = null;
    }

    void MoveToTarget()
    {
        transform.position = Vector3.Slerp(transform.position, target.position, Time.deltaTime);
    }
    #endregion
}
