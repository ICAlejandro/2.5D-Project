using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 customOffset;

    void Start()
    {
        if (target != null)
        {
            customOffset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + customOffset;
    }
}