using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("How long a full day takes in real-world seconds.")]
    public float dayDurationInSeconds = 60f;

    [Header("Current Time Data")]
    [Range(0f, 1f)]
    [Tooltip("0 = Sunrise, 0.25 = Noon, 0.5 = Sunset, 0.75 = Midnight")]
    public float currentTimeOfDay = 0f;

    private float rotationSpeed;

    void Update()
    {
        // Calculate how much time has passed relative to the full day duration
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;

        // Loop the time value back to 0 once it hits 1 (a full day cycle complete)
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
        }

        // Calculate the sun's rotation angle (360 degrees in a full circle)
        float sunXRotation = currentTimeOfDay * 360f;

        // Apply the rotation smoothly to the Directional Light
        // We subtract 90 to make 0f start exactly at sunrise instead of noon
        transform.rotation = Quaternion.Euler(sunXRotation - 90f, 170f, 0f);
    }
}