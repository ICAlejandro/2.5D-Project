using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Drag your Player object into this slot in the Inspector
    public Transform target;

    // This will now be calculated automatically based on your scene setup
    private Vector3 customOffset;

    void Start()
    {
        // Safety check to ensure Unity doesn't error out if the target isn't assigned
        if (target != null)
        {
            // Calculate the exact distance between your camera and the player right now
            customOffset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Maintain your custom hand-set view frame-for-frame instantly
        transform.position = target.position + customOffset;
    }
}