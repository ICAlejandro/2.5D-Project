using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Drag your Player object into this slot in the Inspector
    public Transform target;

    // How far away the camera should stay from the player
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    void LateUpdate()
    {
        // Safety check to ensure Unity doesn't error out if the player is missing
        if (target == null) return;

        // FIX: Removed the smooth LERP calculation.
        // The camera now snaps to the player's position frame-for-frame instantly.
        transform.position = target.position + offset;
    }
}