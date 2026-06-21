using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How fast the object rotates around each axis.")]
    public float rotationSpeed = 26f;

    [Tooltip("Choose which axes you want the object to spin on.")]
    public bool rotateX = false;
    public bool rotateY = true;  // Turned on by default for a classic flat spin
    public bool rotateZ = false;

    void Update()
    {
        // 1. Calculate how much to rotate this frame based on speed and time
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // 2. Determine the rotation values for each axis
        float xRotation = rotateX ? rotationAmount : 0f;
        float yRotation = rotateY ? rotationAmount : 0f;
        float zRotation = rotateZ ? rotationAmount : 0f;

        // 3. Apply the rotation to the object's transform relative to world space
        transform.Rotate(xRotation, yRotation, zRotation, Space.World);
    }
}